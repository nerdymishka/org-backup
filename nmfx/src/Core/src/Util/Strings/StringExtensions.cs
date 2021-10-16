using System;
using System.Collections.Generic;
using System.Text;
using NerdyMishka.Util.Strings.Inflection;
using NerdyMishka.Util.Text;

namespace NerdyMishka.Util.Strings
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        private static IInflectorRuleSet s_inflectorRuleSet = new InflectorRuleSet();
        private static IStringBuilderPool s_stringBuilderPool = new DefaultStringBuliderPool();

        /// <summary>
        /// Gets or sets the inflector rule set.
        /// </summary>
        // TODO: replace with implementation that can handle ReadOnlySpan<char>
        public static IInflectorRuleSet InflectorRuleSet
        {
            get
            {
                s_inflectorRuleSet ??= new InflectorRuleSet();
                return s_inflectorRuleSet;
            }
            set => s_inflectorRuleSet = value;
        }

        /// <summary>
        /// Gets or sets the pool of <see cref="StringBuilder"/> instances.
        /// </summary>
        public static IStringBuilderPool StringBuilderPool
        {
            get
            {
                s_stringBuilderPool ??= new DefaultStringBuliderPool();
                return s_stringBuilderPool;
            }
            set => s_stringBuilderPool = value;
        }

        /// <summary>
        /// Converts a plural word into it's single form.
        /// </summary>
        /// <param name="word">The word to transform.</param>
        /// <returns>A singularized word.</returns>
        public static string Singularize(this string word)
        {
            return InflectorRuleSet.Singularize(word);
        }

        /// <summary>
        /// Converts a word into it's plural form.
        /// </summary>
        /// <param name="input">The word to transofrm.</param>
        /// <returns>A pluralized word.</returns>
        public static string Pluralize(this string input)
        {
            return InflectorRuleSet.Pluralize(input);
        }

        /// <summary>
        /// Underscores the case of the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="convertSpace">if set to <c>true</c> [convert space].</param>
        /// <param name="pool">The string builder pool.</param>
        /// <returns>System.String.</returns>
        public static string Underscore(
             this string value,
             bool convertSpace = true,
             IStringBuilderPool? pool = null)
        {
            pool ??= StringBuilderPool;
#if NETSTANDARD2_0
            var sb = ConvertToUnderscore(value.AsSpan(), convertSpace, pool);
#else
            var sb = ConvertToUnderscore(value, convertSpace, pool);
#endif
            var result = sb.ToString();
            pool.Return(sb);
            return result;
        }

        internal static StringBuilder ConvertToUnderscore(
             ReadOnlySpan<char> value,
             bool convertSpace = true,
             IStringBuilderPool? pool = null)
        {
            pool ??= StringBuilderPool;
            var sb = pool.Get();
            var i = 0;
            foreach (var c in value)
            {
                if (i == 0 && char.IsUpper(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    i++;
                    continue;
                }

                if (c == '-' || (convertSpace && c == ' '))
                {
                    sb.Append('_');
                    i++;
                    continue;
                }

                if (char.IsUpper(c))
                {
                    if (char.IsLetterOrDigit(value[i - 1]))
                    {
                        sb.Append('_');
                    }

                    sb.Append(char.ToLowerInvariant(c));
                    i++;
                    continue;
                }

                sb.Append(c);
                i++;
            }

            return sb;
        }
    }
}
