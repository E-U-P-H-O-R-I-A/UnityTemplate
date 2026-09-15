using System;
using System.IO;
using System.Text;

namespace Editor.Data_Editor
{
    public static class ContainerNaming
    {
        private const string NAMESPACE_PREFIX = "Data.";

        private static readonly string[] CONTAINER_SUFFIXES = { "PrivateContainer", "PublicContainer" };

        public static string NormalizePath(string path) =>
            string.IsNullOrEmpty(path) ? path : path.Replace('\\', '/');

        public static string GetDisplayName(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);

            return FormatTypeName(fileName.StartsWith(NAMESPACE_PREFIX, StringComparison.Ordinal)
                ? fileName.Substring(NAMESPACE_PREFIX.Length)
                : fileName);
        }

        public static string FormatTypeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            foreach (var suffix in CONTAINER_SUFFIXES)
            {
                if (name.Length <= suffix.Length || !name.EndsWith(suffix, StringComparison.Ordinal))
                    continue;

                name = name.Substring(0, name.Length - suffix.Length);
                break;
            }

            var builder = new StringBuilder(name.Length + 4);
            builder.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                char symbol = name[i];

                if (IsWordStart(name, i))
                    builder.Append(' ');

                builder.Append(char.IsUpper(symbol) ? char.ToLowerInvariant(symbol) : symbol);
            }

            return builder.ToString();
        }

        public static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name.Replace(' ', '_');
        }

        public static string FormatSize(long bytes)
        {
            if (bytes < 1024L)
                return $"{bytes} B";

            if (bytes < 1024L * 1024L)
                return $"{bytes / 1024f:0.#} KB";

            return $"{bytes / (1024f * 1024f):0.#} MB";
        }

        private static bool IsWordStart(string name, int index)
        {
            if (!char.IsUpper(name[index]))
                return false;

            if (!char.IsUpper(name[index - 1]))
                return true;

            return index + 1 < name.Length && char.IsLower(name[index + 1]);
        }
    }
}
