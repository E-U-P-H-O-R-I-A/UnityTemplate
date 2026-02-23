using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Model;
using UnityEngine;

namespace Services.PrivateModelProvider
{
    public class PrivateModelProvider : IPrivateModelProvider
    {
        private const string FOLDER_NAME = "PrivateData";
    
        private readonly Dictionary<Type, IPrivateModel> models = new();
    
        public void Init()
        {
            models.Clear();
        
            var modelTypes = FindPrivateModelTypes();
        
            foreach (var t in modelTypes)
            {
                try
                {
                    var instance = (IPrivateModel)Activator.CreateInstance(t);
                    models[t] = instance;

                    var path = GetPathForType(t);
                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        instance.ImportFromJson(json);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PrivateModelProvider] Failed to init model {t.FullName}: {e}");
                }
            }
        }

        public void SaveAll()
        {
            EnsureFolder();

            foreach (var kv in models)
            {
                try
                {
                    var type = kv.Key;
                    var model = kv.Value;

                    var json = model.ExportToJson();
                    var path = GetPathForType(type);

                    File.WriteAllText(path, json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PrivateModelProvider] Failed to save model {kv.Key.FullName}: {e}");
                }
            }
        }

        public void SaveModel<TModel>() where TModel : IPrivateModel
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

                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[PrivateModelProvider] Failed to save model {typeof(TModel).FullName}: {e}");
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