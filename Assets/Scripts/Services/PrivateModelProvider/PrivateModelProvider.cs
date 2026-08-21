using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Services.LogService;
using UnityEngine;

namespace Services.PrivateModelProvider
{
    public class PrivateModelProvider : IPrivateModelProvider
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string TEMP_EXTENSION = ".tmp";
        private const string CORRUPT_EXTENSION = ".corrupt";

        private readonly Dictionary<Type, IPrivateModel> models = new();
        private readonly Dictionary<Type, string> queuedJson = new();
        private readonly HashSet<Type> writingTypes = new();
        private readonly ILogService logService;

        public PrivateModelProvider(ILogService logService)
        {
            this.logService = logService;
        }
        
        public async UniTask Initizele(CancellationToken ct = default)
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
                        await LoadInto(instance, path, ct);
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

        public TModel GetModel<TModel>() where TModel : IPrivateModel
        {
            if (models.TryGetValue(typeof(TModel), out var temp) && temp is TModel typed)
                return typed;

            logService.LogError($"[PrivateModelProvider] Model {typeof(TModel).FullName} is not registered", LogCategory.PrivateModel);

            return default;
        }

        public async UniTask SaveAll(CancellationToken ct = default)
        {
            foreach (var kv in models.ToArray())
            {
                ct.ThrowIfCancellationRequested();

                await SaveTyped(kv.Key, kv.Value, ct);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public async UniTask SaveModel<TModel>(CancellationToken ct = default) where TModel : IPrivateModel
        {
            var model = GetModel<TModel>();

            if (model == null)
            {
                logService.LogError($"[PrivateModelProvider] Nothing was saved, model {typeof(TModel).FullName} is not registered", LogCategory.PrivateModel);
                return;
            }

            await SaveTyped(typeof(TModel), model, ct);
        }

        #region Helpers

        private async UniTask SaveTyped(Type type, IPrivateModel model, CancellationToken ct)
        {
            try
            {
                queuedJson[type] = model.ExportToJson();

                if (writingTypes.Contains(type))
                    return;

                writingTypes.Add(type);

                try
                {
                    EnsureFolder();

                    while (queuedJson.Remove(type, out var json))
                        await WriteAtomic(GetPathForType(type), json, ct);
                }
                finally
                {
                    writingTypes.Remove(type);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                logService.LogError($"[PrivateModelProvider] Failed to save model {type.FullName}: {e}", LogCategory.PrivateModel);
            }
        }

        private static async UniTask WriteAtomic(string path, string json, CancellationToken ct)
        {
            var temporary = path + TEMP_EXTENSION;

            await File.WriteAllTextAsync(temporary, json, ct);

            if (File.Exists(path))
                File.Delete(path);

            File.Move(temporary, path);
        }

        private async UniTask LoadInto(IPrivateModel model, string path, CancellationToken ct)
        {
            var json = await File.ReadAllTextAsync(path, ct);

            try
            {
                model.ImportFromJson(json);
            }
            catch (Exception e)
            {
                var kept = PreserveCorruptFile(path);

                logService.LogError($"[PrivateModelProvider] Could not read {Path.GetFileName(path)}, it was kept as {Path.GetFileName(kept)}: {e}", LogCategory.PrivateModel);
            }
        }

        private static string PreserveCorruptFile(string path)
        {
            var target = path + CORRUPT_EXTENSION;

            if (File.Exists(target))
                File.Delete(target);

            File.Move(path, target);

            return target;
        }

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
