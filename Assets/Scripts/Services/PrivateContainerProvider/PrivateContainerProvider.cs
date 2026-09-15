using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Services.LogService;
using UnityEngine;

namespace Services.PrivateContainerProvider
{
    public class PrivateContainerProvider : IPrivateContainerProvider
    {
        private const string FOLDER_NAME = "PrivateData";
        private const string TEMP_EXTENSION = ".tmp";
        private const string CORRUPT_EXTENSION = ".corrupt";
        private const string SYNC_TEMP_EXTENSION = ".sync.tmp";

        private readonly Dictionary<Type, IPrivateContainer> containers = new();
        private readonly Dictionary<Type, string> queuedJson = new();
        private readonly HashSet<Type> writingTypes = new();
        private readonly ILogService logService;

        public PrivateContainerProvider(ILogService logService)
        {
            this.logService = logService;
        }
        
        public async UniTask Initialize(CancellationToken ct = default)
        {
            containers.Clear();
            SubscribeToApplicationEvents();

            var containerTypes = FindPrivateContainerTypes();

            foreach (var t in containerTypes)
            {
                ct.ThrowIfCancellationRequested();
                
                try
                {
                    var instance = (IPrivateContainer)Activator.CreateInstance(t);
                    containers[t] = instance;

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
                    logService.LogError($"[PrivateContainerProvider] Failed to init container {t.FullName}: {e}", LogCategory.PrivateContainer);
                }
                
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public TContainer GetContainer<TContainer>() where TContainer : IPrivateContainer
        {
            if (containers.TryGetValue(typeof(TContainer), out var temp) && temp is TContainer typed)
                return typed;

            throw new InvalidOperationException($"[PrivateContainerProvider] Container {typeof(TContainer).FullName} is not registered");
        }

        public async UniTask SaveAll(CancellationToken ct = default)
        {
            foreach (var kv in containers.ToArray())
            {
                ct.ThrowIfCancellationRequested();

                await SaveTyped(kv.Key, kv.Value, ct);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        public async UniTask SaveContainer<TContainer>(CancellationToken ct = default) where TContainer : IPrivateContainer
        {
            var container = GetContainer<TContainer>();

            await SaveTyped(typeof(TContainer), container, ct);
        }

        #region Helpers

        private void SubscribeToApplicationEvents()
        {
            Application.focusChanged -= OnFocusChanged;
            Application.focusChanged += OnFocusChanged;
            Application.quitting -= SaveAllSync;
            Application.quitting += SaveAllSync;
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
                SaveAllSync();
        }

        private void SaveAllSync()
        {
            foreach (var kv in containers)
            {
                try
                {
                    EnsureFolder();
                    WriteAtomicSync(GetPathForType(kv.Key), kv.Value.ExportToJson());
                }
                catch (Exception e)
                {
                    logService.LogError($"[PrivateContainerProvider] Failed to save container {kv.Key.FullName} synchronously: {e}", LogCategory.PrivateContainer);
                }
            }
        }

        private static void WriteAtomicSync(string path, string json)
        {
            var temporary = path + SYNC_TEMP_EXTENSION;

            File.WriteAllText(temporary, json);

            if (File.Exists(path))
                File.Delete(path);

            File.Move(temporary, path);
        }

        private async UniTask SaveTyped(Type type, IPrivateContainer container, CancellationToken ct)
        {
            try
            {
                queuedJson[type] = container.ExportToJson();

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
                logService.LogError($"[PrivateContainerProvider] Failed to save container {type.FullName}: {e}", LogCategory.PrivateContainer);
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

        private async UniTask LoadInto(IPrivateContainer container, string path, CancellationToken ct)
        {
            var json = await File.ReadAllTextAsync(path, ct);

            try
            {
                container.ImportFromJson(json);
            }
            catch (Exception e)
            {
                var kept = PreserveCorruptFile(path);

                logService.LogError($"[PrivateContainerProvider] Could not read {Path.GetFileName(path)}, it was kept as {Path.GetFileName(kept)}: {e}", LogCategory.PrivateContainer);
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

        private static List<Type> FindPrivateContainerTypes()
        {
            var assembly = typeof(IPrivateContainer).Assembly;

            return assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(IPrivateContainer).IsAssignableFrom(t) &&
                    t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();
        }
    
        #endregion
    }
}
