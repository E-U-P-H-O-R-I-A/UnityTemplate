using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Data;
using UnityEngine;

namespace Editor.DataEditor
{
    /// <summary>
    /// Reflection access to private models: type lookup, schemes and serialized fields.
    /// </summary>
    public static class SchemeReflection
    {
        public static Type FindModelType(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            return typeof(IPrivateModel).Assembly
                .GetTypes()
                .FirstOrDefault(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(IPrivateModel).IsAssignableFrom(type) &&
                    ModelNaming.SanitizeFileName(type.FullName ?? type.Name) == fileName);
        }

        public static IEnumerable<PrivateScheme> GetSchemes(IPrivateModel model)
        {
            var collectionField = FindField(model.GetType(), "schemes");
            if (collectionField?.GetValue(model) is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (item is PrivateScheme scheme)
                        yield return scheme;
                }

                yield break;
            }

            var singleField = FindField(model.GetType(), "scheme");
            if (singleField == null)
                yield break;

            var singleScheme = singleField.GetValue(model) as PrivateScheme;
            if (singleScheme == null)
            {
                var getScheme = model.GetType().GetMethod(
                    nameof(PrivateModel.Single<PrivateScheme>.GetScheme),
                    Type.EmptyTypes);

                singleScheme = getScheme?.Invoke(model, null) as PrivateScheme;
            }

            if (singleScheme != null)
                yield return singleScheme;
        }

        public static IEnumerable<FieldInfo> GetSerializableFields(Type type)
        {
            return type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field =>
                    !field.IsStatic &&
                    !field.IsInitOnly &&
                    !field.IsNotSerialized &&
                    (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null));
        }

        private static FieldInfo FindField(Type type, string fieldName)
        {
            while (type != null)
            {
                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    return field;

                type = type.BaseType;
            }

            return null;
        }
    }
}
