using System;
using System.Linq;
using Xunit.Sdk;

namespace Mettle
{
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = false)]
    [XunitTestCaseDiscoverer("Mettle.Sdk.TestCaseDiscoverer", "Mettle.Attributes")]
    public class UnitAttribute : TestCaseAttribute
    {
        public UnitAttribute()
            : base("unit")
        {
        }
    }
}