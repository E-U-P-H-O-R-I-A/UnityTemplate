using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Data;
using UnityEngine;

namespace Editor.Data_Editor
{
    public static class SchemeReflection
    {
        private const string COLLECTION_FIELD = "schemes";
        private const string SINGLE_FIELD = "scheme";
        private const string SINGLE_GETTER = "GetScheme";

        public static Type FindModelType(string path, Type markerInterface)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            return markerInterface.Assembly
                .GetTypes()
                .FirstOrDefault(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    markerInterface.IsAssignableFrom(type) &&
                    (ModelNaming.SanitizeFileName(type.FullName ?? type.Name) == fileName || type.Name == fileName));
        }

        public static IEnumerable<IScheme> GetSchemes(object model)
        {
            if (model == null)
                yield break;

            var collectionField = FindField(model.GetType(), COLLECTION_FIELD);
            if (collectionField?.GetValue(model) is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (item is IScheme scheme)
                        yield return scheme;
                }

                yield break;
            }

            var singleField = FindField(model.GetType(), SINGLE_FIELD);
            if (singleField == null)
                yield break;

            var singleScheme = singleField.GetValue(model) as IScheme;

            if (singleScheme == null)
            {
                var getScheme = model.GetType().GetMethod(SINGLE_GETTER, Type.EmptyTypes);
                singleScheme = getScheme?.Invoke(model, null) as IScheme;
            }

            if (singleScheme != null)
                yield return singleScheme;
        }

        public static bool IsCollection(object model) =>
            model != null && FindField(model.GetType(), COLLECTION_FIELD) != null;

        public static IEnumerable<FieldInfo> GetSerializableFields(Type type)
        {
            var levels = new List<Type>();

            while (type != null && type != typeof(object))
            {
                levels.Insert(0, type);
                type = type.BaseType;
            }

            foreach (var level in levels)
            {
                var fields = level
                    .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                               BindingFlags.DeclaredOnly)
                    .Where(IsSerializable);

                foreach (var field in fields)
                    yield return field;
            }
        }

        public static IEnumerable<Type> GetConcreteTypes(Type baseType)
        {
            return baseType.Assembly
                .GetTypes()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    baseType.IsAssignableFrom(type))
                .OrderBy(type => type.Name);
        }

        private static bool IsSerializable(FieldInfo field)
        {
            return !field.IsStatic &&
                   !field.IsInitOnly &&
                   !field.IsNotSerialized &&
                   (field.IsPublic ||
                    field.GetCustomAttribute<SerializeField>() != null ||
                    field.GetCustomAttribute<SchemeIdAttribute>() != null);
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
