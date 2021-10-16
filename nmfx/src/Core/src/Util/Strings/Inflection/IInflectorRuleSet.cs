using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Util.Strings.Inflection
{
    public interface IInflectorRuleSet
    {
        void AddIrregularRule(string singular, string plural, bool matchEnding = true);

        void AddPluralRule(RegexRule rule);

        void AddPluralRule(string pattern, string replacement);

        void AddSingularRule(RegexRule rule);

        void AddSingularRule(string pattern, string replacement);

        void AddUncountable(string value);

        ReadOnlySpan<char> SingularizeSpan(ReadOnlySpan<char> span);

        ReadOnlySpan<char> PluralizeSpan(ReadOnlySpan<char> span);

        string Singularize(string input);

        string Pluralize(string input);
    }
}
