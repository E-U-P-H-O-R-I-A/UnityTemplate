using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Data;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public static class RecordReflection
    {
        private const string COLLECTION_FIELD = "records";
        private const string SINGLE_FIELD = "record";
        private const string SINGLE_GETTER = "GetRecord";

        public static Type FindContainerType(string path, Type markerInterface)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            return markerInterface.Assembly
                .GetTypes()
                .FirstOrDefault(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    markerInterface.IsAssignableFrom(type) &&
                    (ContainerNaming.SanitizeFileName(type.FullName ?? type.Name) == fileName || type.Name == fileName));
        }

        public static IEnumerable<IRecord> GetRecords(object container)
        {
            if (container == null)
                yield break;

            var collectionField = FindField(container.GetType(), COLLECTION_FIELD);
            if (collectionField?.GetValue(container) is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (item is IRecord record)
                        yield return record;
                }

                yield break;
            }

            var singleField = FindField(container.GetType(), SINGLE_FIELD);
            if (singleField == null)
                yield break;

            var singleRecord = singleField.GetValue(container) as IRecord;

            if (singleRecord == null)
            {
                var getRecord = container.GetType().GetMethod(SINGLE_GETTER, Type.EmptyTypes);
                singleRecord = getRecord?.Invoke(container, null) as IRecord;
            }

            if (singleRecord != null)
                yield return singleRecord;
        }

        public static bool IsCollection(object container) =>
            container != null && FindField(container.GetType(), COLLECTION_FIELD) != null;

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
                    field.GetCustomAttribute<RecordIdAttribute>() != null);
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
