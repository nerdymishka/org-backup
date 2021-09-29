using System;
using Xunit.Sdk;

namespace Mettle
{
    [AttributeUsage(
         AttributeTargets.Method,
         Inherited = false)]
    [XunitTestCaseDiscoverer("Mettle.Sdk.TestCaseDiscoverer", "Mettle.Attributes")]
    public class IntegrationAttribute : TestCaseAttribute
    {
        public IntegrationAttribute()
            : base("integration")
        {
        }
    }
}