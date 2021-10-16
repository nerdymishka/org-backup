using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Util.Strings.Inflection
{
    public class InflectorRuleSet : IInflectorRuleSet
    {
        private readonly List<RegexRule> pluralRules = new List<RegexRule>();

        private readonly List<RegexRule> singleRules = new List<RegexRule>();

        private readonly HashSet<string> uncountables = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
             // Singular words with no plurals.
             "adulthood",
             "advice",
             "agenda",
             "aid",
             "aircraft",
             "alcohol",
             "ammo",
             "anime",
             "athletics",
             "audio",
             "bison",
             "blood",
             "bream",
             "buffalo",
             "butter",
             "carp",
             "cash",
             "chassis",
             "chess",
             "clothing",
             "cod",
             "commerce",
             "cooperation",
             "corps",
             "debris",
             "diabetes",
             "digestion",
             "elk",
             "energy",
             "equipment",
             "excretion",
             "expertise",
             "firmware",
             "flounder",
             "fun",
             "gallows",
             "garbage",
             "graffiti",
             "headquarters",
             "health",
             "herpes",
             "highjinks",
             "homework",
             "housework",
             "information",
             "jeans",
             "justice",
             "kudos",
             "labour",
             "literature",
             "machinery",
             "mackerel",
             "mail",
             "media",
             "mews",
             "moose",
             "music",
             "mud",
             "manga",
             "news",
             "only",
             "personnel",
             "pike",
             "plankton",
             "pliers",
             "police",
             "pollution",
             "premises",
             "rain",
             "research",
             "rice",
             "salmon",
             "scissors",
             "series",
             "sewage",
             "shambles",
             "shrimp",
             "software",
             "species",
             "staff",
             "swine",
             "tennis",
             "traffic",
             "transportation",
             "trout",
             "tuna",
             "wealth",
             "welfare",
             "whiting",
             "wildebeest",
             "wildlife",
             "you",
        };

        public InflectorRuleSet()
        {
             this.AddPluralRule("$", "s");
             this.AddPluralRule("s$", "s");
             this.AddPluralRule("(ax|test)is$", "$1es");
             this.AddPluralRule("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)us$", "$1i");
             this.AddPluralRule("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)$", "$1es");
             this.AddPluralRule("(buffal|tomat|volcan|ech|embarg|her|mosquit|potat|torped|vet)o$", "$1oes");
             this.AddPluralRule("([dti])um$", "$1a");
             this.AddPluralRule("sis$", "ses");
             this.AddPluralRule("(?:([^f])fe|([lr])f)$", "$1$2ves");
             this.AddPluralRule("(hive)$", "$1s");
             this.AddPluralRule("([^aeiouy]|qu)y$", "$1ies");
             this.AddPluralRule("(x|ch|ss|sh)$", "$1es");
             this.AddPluralRule("(matr|vert|ind|d)(ix|ex)$", "$1ices");
             this.AddPluralRule("(^[m|l])ouse$", "$1ice");
             this.AddPluralRule("^(ox)$", "$1en");
             this.AddPluralRule("(quiz)$", "$1zes");
             this.AddPluralRule("(buz|blit|walt)z$", "$1zes");
             this.AddPluralRule("(hoo|lea|loa|thie)f$", "$1ves");
             this.AddPluralRule("(alumn|alg|larv|vertebr)a$", "$1ae");
             this.AddPluralRule("(criteri|phenomen)on$", "$1a");

             this.AddSingularRule("s$", string.Empty);
             this.AddSingularRule("(n)ews$", "$1ews");
             this.AddSingularRule("([dti])a$", "$1um");
             this.AddSingularRule("(analy|ba|diagno|parenthe|progno|synop|the|ellip|empha|neuro|oa|paraly)ses$", "$1sis");
             this.AddSingularRule("([^f])ves$", "$1fe");
             this.AddSingularRule("(hive)s$", "$1");
             this.AddSingularRule("(tive)s$", "$1");
             this.AddSingularRule("([lr]|hoo|lea|loa|thie)ves$", "$1f");
             this.AddSingularRule("(^zomb)?([^aeiouy]|qu)ies$", "$2y");
             this.AddSingularRule("(s)eries$", "$1eries");
             this.AddSingularRule("(m)ovies$", "$1ovie");
             this.AddSingularRule("(x|ch|ss|sh)es$", "$1");
             this.AddSingularRule("(^[m|l])ice$", "$1ouse");
             this.AddSingularRule("(o)es$", "$1");
             this.AddSingularRule("(shoe)s$", "$1");
             this.AddSingularRule("(cris|ax|test)es$", "$1is");
             this.AddSingularRule("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)i$", "$1us");
             this.AddSingularRule("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)es$", "$1");
             this.AddSingularRule("^(ox)en", "$1");
             this.AddSingularRule("(matr|d)ices$", "$1ix");
             this.AddSingularRule("(vert|ind)ices$", "$1ex");
             this.AddSingularRule("(quiz)zes$", "$1");
             this.AddSingularRule("(buz|blit|walt)zes$", "$1z");
             this.AddSingularRule("(alumn|alg|larv|vertebr)ae$", "$1a");
             this.AddSingularRule("(criteri|phenomen)a$", "$1on");
             this.AddSingularRule("([b|r|c]ook|room|smooth)ies$", "$1ie");

             this.AddIrregularRule("person", "people");
             this.AddIrregularRule("man", "men");
             this.AddIrregularRule("human", "humans");
             this.AddIrregularRule("child", "children");
             this.AddIrregularRule("sex", "sexes");
             this.AddIrregularRule("glove", "gloves");
             this.AddIrregularRule("move", "moves");
             this.AddIrregularRule("goose", "geese");
             this.AddIrregularRule("wave", "waves");
             this.AddIrregularRule("foot", "feet");
             this.AddIrregularRule("tooth", "teeth");
             this.AddIrregularRule("curriculum", "curricula");
             this.AddIrregularRule("database", "databases");
             this.AddIrregularRule("zombie", "zombies");
             this.AddIrregularRule("personnel", "personnel");
             this.AddIrregularRule("cache", "caches");
             this.AddIrregularRule("ex", "exes", matchEnding: false);
             this.AddIrregularRule("is", "are", matchEnding: false);
             this.AddIrregularRule("that", "those", matchEnding: false);
             this.AddIrregularRule("this", "these", matchEnding: false);
             this.AddIrregularRule("bus", "buses", matchEnding: false);
             this.AddIrregularRule("die", "dice", matchEnding: false);
        }

        public void AddIrregularRule(string singular, string plural, bool matchEnding = true)
        {
             if (matchEnding)
             {
                 this.AddPluralRule("(" + singular[0] + ")" + singular.Substring(1) + "$", "$1" + plural.Substring(1));
                 this.AddSingularRule("(" + plural[0] + ")" + plural.Substring(1) + "$", "$1" + singular.Substring(1));
                 return;
             }

             this.AddPluralRule($"^{singular}$", plural);
             this.AddSingularRule($"^{plural}$", singular);
        }

        public void AddPluralRule(RegexRule rule)
             => this.pluralRules.Add(rule);

        public void AddPluralRule(string pattern, string replacement)
             => this.pluralRules.Add(new RegexRule(pattern, replacement));

        public void AddSingularRule(RegexRule rule)
             => this.singleRules.Add(rule);

        public void AddSingularRule(string pattern, string replacement)
             => this.singleRules.Add(new RegexRule(pattern, replacement));

        public void AddUncountable(string value)
        {
             if (string.IsNullOrWhiteSpace(value))
                 throw new ArgumentNullException(nameof(value));

             value = value.ToLowerInvariant();
             if (!this.uncountables.Contains(value))
                 this.uncountables.Add(value);
        }

        public string Pluralize(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length == 1)
                return input;

    #if NETSTANDARD2_0
            var span = input.AsSpan();
            if (IsUncountable(span, this.uncountables))
                return input;

            var value = input;

            for (var i = this.pluralRules.Count - 1; i > -1; i--)
            {
                var rule = this.pluralRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);
                    if (char.IsUpper(input[0]) && char.IsLower(result[0]))
                        return input[0] + result.Substring(1);

                    return result;
                }
            }
    #elif NETSTANDARD2_1_OR_GREATER
            if (IsUncountable(input, this.uncountables))
                return input;

            var value = input;

            for (var i = this.pluralRules.Count - 1; i > -1; i--)
            {
                var rule = this.pluralRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);
                    if (char.IsUpper(input[0]) && char.IsLower(result[0]))
                        return input[0] + result.Substring(1);

                    return result;
                }
            }
    #endif
            return input;
        }

        public ReadOnlySpan<char> PluralizeSpan(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty || span.Length == 1)
                return span;

#if NETSTANDARD2_0

            if (IsUncountable(span, this.uncountables))
                return span;

            var value = new string(span.ToArray());

            for (var i = this.pluralRules.Count - 1; i > -1; i--)
            {
                var rule = this.pluralRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);
                    if (char.IsUpper(span[0]) && char.IsLower(result[0]))
                        return (span[0] + result.Substring(1)).AsSpan();

                    return result.AsSpan();
                }
            }

#else
            var value = new string(span);

            for (var i = this.pluralRules.Count - 1; i > -1; i--)
            {
                var rule = this.pluralRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);
                    if (char.IsUpper(span[0]) && char.IsLower(result[0]))
                        return span[0] + result.Substring(1);

                    return result;
                }
            }
#endif
            return span;
        }

        public ReadOnlySpan<char> SingularizeSpan(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty || span.Length == 1)
                return span;

            if (IsUncountable(span, this.uncountables))
                return span;

            var value = new string(span.ToArray());
#if NETSTANDARD2_0
            for (var i = this.singleRules.Count - 1; i > -1; i--)
            {
                var rule = this.singleRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);

                    if (char.IsUpper(span[0]) && char.IsLower(result[0]))
                        return (span[0] + result.Substring(1)).AsSpan();

                    return result.AsSpan();
                }
            }
#else
            for (var i = this.singleRules.Count - 1; i > -1; i--)
            {
                var rule = this.singleRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);
                    if (char.IsUpper(span[0]) && char.IsLower(result[0]))
                         return new string(span[0] + result.Substring(1));

                    return result;
                }
            }
#endif

            return span;
        }

        public string Singularize(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length == 1)
                return input;

#if NETSTANDARD2_0
            if (IsUncountable(input.AsSpan(), this.uncountables))
                return input;

            var value = input;
#else
            if (IsUncountable(input, this.uncountables))
                return input;

            var value = new string(input);

#endif

            for (var i = this.singleRules.Count - 1; i > -1; i--)
            {
                var rule = this.singleRules[i];
                if (rule.Expression.IsMatch(value))
                {
                    var result = rule.Expression.Replace(value, rule.Replacement);
                    if (char.IsUpper(input[0]) && char.IsLower(result[0]))
                         return input[0] + result.Substring(1);

                    return result;
                }
            }

            return input;
        }

        private static bool IsUncountable(ReadOnlySpan<char> input, HashSet<string> uncountables)
        {
             var start = char.ToLowerInvariant(input[0]);

             foreach (var item in uncountables)
             {
                 var c = item[0];
                 if (c != start)
                     continue;

                 if (item.Length != input.Length)
                     continue;

                 var match = true;
                 for (var i = 1; i < input.Length; i++)
                 {
                     var left = input[i];
                     var right = char.ToLowerInvariant(item[i]);

                     if (left != right)
                     {
                          match = false;
                          break;
                     }
                 }

                 if (match)
                     return true;
             }

             return false;
        }
    }
}
