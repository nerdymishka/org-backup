using System.Text.RegularExpressions;

namespace NerdyMishka.Util.Strings.Inflection
{
    public class RegexRule
    {
        private readonly Regex expression;
        private readonly string replacement;

        public RegexRule(string pattern, string replacement)
        {
             this.expression = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
             this.replacement = replacement;
        }

        public Regex Expression => this.expression;

        public string Replacement => this.replacement;
    }
}
