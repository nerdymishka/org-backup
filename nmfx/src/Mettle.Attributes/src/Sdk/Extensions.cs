using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mettle
{
    public static class Extensions
    {
        public static void Add(this IDictionary<string, List<string>> traits, string key, string value)
        {
            if (!traits.TryGetValue(key, out var values))
            {
                values = new List<string>();
                traits.Add(key, values);
            }

            if (!values.Contains(value))
                values.Add(value);
        }

        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? data)
        {
            return string.IsNullOrEmpty(data);
        }

        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? data)
        {
            return string.IsNullOrEmpty(data);
        }
    }
}