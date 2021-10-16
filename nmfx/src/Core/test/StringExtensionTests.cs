using Mettle;
using NerdyMishka.Util.Strings;
using Xunit;

namespace NerdyMishka.Core.Tests;

public class StringExtensionTests
{
    [Unit]
    public void Underscore()
    {
        Assert.Equal("under_score", "UnderScore".Underscore());
        Assert.Equal("under_score_test_test", "under-score testTest".Underscore());
    }

    [Unit]
    public void Puralize()
    {
        Assert.Equal("cats", "cat".Pluralize());
        Assert.Equal("geese", "goose".Pluralize());
    }

    [Unit]
    public void Singularize()
    {
        Assert.Equal("cat", "cats".Singularize());
        Assert.Equal("goose", "geese".Singularize());
    }
}