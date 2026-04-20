using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data.Model;
using Services.LogService;
using UnityEngine;

namespace Services.PrivateModelProvider
{
    public class PrivateModelProvider : IPrivateModelProvider
    {
        private const string FOLDER_NAME = "PrivateData";

        private readonly Dictionary<Type, IPrivateModel> models = new();
        private readonly ILogService logService;

        public PrivateModelProvider(ILogService logService)
        {
            this.logService = logService;
        }
        
        public async UniTask Init(CancellationToken ct = default)
        {
            models.Clear();

            var modelTypes = FindPrivateModelTypes();

            foreach (var t in modelTypes)
            {
                ct.ThrowIfCancellationRequested();
                
                try
                {
                    var instance = (IPrivateModel)Activator.CreateInstance(t);
                    models[t] = instance;

                    var path = GetPathForType(t);

                    if (File.Exists(path))
                    {
                        var json = await File.ReadAllTextAsync(path, ct);
                        instance.ImportFromJson(json);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    logService.LogError($"[PrivateModelProvider] Failed to init model {t.FullName}: {e}", LogCategory.PrivateModel);
                }
                
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public async UniTask SaveAll(CancellationToken ct = default)
        {
            EnsureFolder();

            foreach (var kv in models.ToArray())
            {
                ct.ThrowIfCancellationRequested();
                
                try
                {
                    var type = kv.Key;
                    var model = kv.Value;

                    var json = model.ExportToJson();
                    var path = GetPathForType(type);

                    await File.WriteAllTextAsync(path, json, ct);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    logService.LogError($"[PrivateModelProvider] Failed to save model {kv.Key.FullName}: {e}", LogCategory.PrivateModel);
                }
                
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public async UniTask SaveModel<TModel>(CancellationToken ct = default) where TModel : IPrivateModel
        {
            var m = GetModel<TModel>();
            if (m == null)
                return;

            EnsureFolder();

            try
            {
                var type = typeof(TModel);
                var json = m.ExportToJson();
                var path = GetPathForType(type);

                await File.WriteAllTextAsync(path, json, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                logService.LogError($"[PrivateModelProvider] Failed to save model {typeof(TModel).FullName}: {e}", LogCategory.PrivateModel);
            }
        }

        public TModel GetModel<TModel>() where TModel : IPrivateModel
        {
            if (models.TryGetValue(typeof(TModel), out var temp) && temp is TModel typed)
                return typed;

            return default;
        }

        #region Helpers
    
        private static void EnsureFolder()
        {
            var folder = GetBaseFolder();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        private static string GetBaseFolder()
        {
            return Path.Combine(Application.persistentDataPath, FOLDER_NAME);
        }

        private static string GetPathForType(Type t)
        {
            var safeName = SanitizeFileName(t.FullName ?? t.Name);
            return Path.Combine(GetBaseFolder(), safeName + ".json");
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
        
            return name.Replace(' ', '_');
        }

        private static List<Type> FindPrivateModelTypes()
        {
            var assembly = typeof(IPrivateModel).Assembly;

            return assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(IPrivateModel).IsAssignableFrom(t) &&
                    t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();
        }
    
        #endregion
    }
}
