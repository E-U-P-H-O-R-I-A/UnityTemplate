using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEditor;
using UnityEngine;

namespace EditorTools.DataEditor
{
    public static class PublicContainerCodeGenerator
    {
        private const string REGION_NAME = "Generated";
        private const string ENUM_NAME = "Id";
        private const string EMPTY_MEMBER = "None";
        private const string MEMBER_INDENT = "            ";
        private const string BODY_INDENT = "        ";
        private const string CLASS_END = "    }";

        [MenuItem("Tools/Regenerate Public Ids")]
        public static void GenerateAll()
        {
            bool changed = false;

            foreach (var containerType in RecordReflection.GetConcreteTypes(typeof(IPublicContainer)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(PublicDataService.GetAssetPath(containerType));

                if (asset == null)
                    continue;

                var ids = RecordReflection.GetRecords(asset).Select(record => record.Id).ToList();

                changed |= Generate(containerType, ids);
            }

            if (changed)
                AssetDatabase.Refresh();
        }

        public static bool Generate(Type containerType, IReadOnlyList<string> ids)
        {
            if (containerType == null)
                return false;

            if (!IsCollection(containerType))
                return false;

            string path = FindSourcePath(containerType);

            if (path == null)
            {
                Debug.LogWarning($"[PublicContainerCodeGenerator] No source file found for {containerType.Name}, ids are not generated.");
                return false;
            }

            string source = File.ReadAllText(path);
            string updated = WriteRegion(source, BuildRegion(GetMemberNames(containerType, ids)));

            if (updated == null)
            {
                Debug.LogWarning($"[PublicContainerCodeGenerator] Could not place the {REGION_NAME} region in {path}.");
                return false;
            }

            if (updated == source)
                return false;

            File.WriteAllText(path, updated);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            return true;
        }

        private static List<string> BuildRegion(IReadOnlyList<string> members)
        {
            var lines = new List<string>
            {
                $"{BODY_INDENT}#region {REGION_NAME}",
                string.Empty,
                $"{BODY_INDENT}public enum {ENUM_NAME}",
                $"{BODY_INDENT}{{"
            };

            if (members.Count == 0)
                lines.Add($"{MEMBER_INDENT}{EMPTY_MEMBER} = 0,");

            for (int index = 0; index < members.Count; index++)
                lines.Add($"{MEMBER_INDENT}{members[index]} = {index},");

            lines.AddRange(new[]
            {
                $"{BODY_INDENT}}}",
                string.Empty,
                $"{BODY_INDENT}#endregion"
            });

            return lines;
        }

        private static string WriteRegion(string source, List<string> region)
        {
            string newLine = source.Contains("\r\n") ? "\r\n" : "\n";
            var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            bool endsWithNewLine = lines.Count > 0 && lines[^1].Length == 0;
            if (endsWithNewLine)
                lines.RemoveAt(lines.Count - 1);

            int start = lines.FindIndex(line => line.Trim() == $"#region {REGION_NAME}");

            if (start >= 0)
            {
                int end = lines.FindIndex(start, line => line.Trim() == "#endregion");

                if (end < 0)
                    return null;

                lines.RemoveRange(start, end - start + 1);
                lines.InsertRange(start, region);
            }
            else
            {
                int classEnd = lines.FindLastIndex(line => line == CLASS_END);

                if (classEnd < 0)
                    return null;

                var block = new List<string>();
                string previous = classEnd > 0 ? lines[classEnd - 1].Trim() : string.Empty;

                if (previous.Length > 0 && previous != "{")
                    block.Add(string.Empty);

                block.AddRange(region);
                lines.InsertRange(classEnd, block);
            }

            return string.Join(newLine, lines) + (endsWithNewLine ? newLine : string.Empty);
        }

        private static string FindSourcePath(Type containerType)
        {
            foreach (var guid in AssetDatabase.FindAssets($"{containerType.Name} t:MonoScript"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (Path.GetFileNameWithoutExtension(path) == containerType.Name)
                    return path;
            }

            return null;
        }

        private static List<string> GetMemberNames(Type containerType, IReadOnlyList<string> ids)
        {
            var members = new List<string>(ids.Count);
            var skipped = new List<string>();

            foreach (var id in ids)
            {
                if (IsValidMemberName(id) && !members.Contains(id))
                {
                    members.Add(id);
                    continue;
                }

                skipped.Add(string.IsNullOrEmpty(id) ? "(empty)" : id);
            }

            if (skipped.Count > 0)
            {
                Debug.LogWarning($"[PublicContainerCodeGenerator] {containerType.Name}: ids left out of {ENUM_NAME} " +
                                 $"because they are not unique valid C# names: {string.Join(", ", skipped)}");
            }

            return members;
        }

        private static bool IsValidMemberName(string id)
        {
            if (string.IsNullOrEmpty(id) || id == ENUM_NAME)
                return false;

            if (!char.IsLetter(id[0]) && id[0] != '_')
                return false;

            foreach (var symbol in id)
            {
                if (!char.IsLetterOrDigit(symbol) && symbol != '_')
                    return false;
            }

            return true;
        }

        private static bool IsCollection(Type containerType)
        {
            var type = containerType;

            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PublicContainer.Collection<>))
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}
