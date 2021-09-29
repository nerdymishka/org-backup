using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    /// <summary>Wraps another test case that should be skipped.</summary>
    internal sealed class SkippedTestCase : XunitTestCase
    {
        private string? skipReason;

        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public SkippedTestCase()
            : base()
        {
        }

        public SkippedTestCase(
            string? skipReason,
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            object[]? testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            this.skipReason = skipReason;
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            this.skipReason = data.GetValue<string>(nameof(this.skipReason));
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            data.AddValue(nameof(this.skipReason), this.skipReason);
        }

        protected override string GetSkipReason(IAttributeInfo factAttribute)
           => this.skipReason ?? base.GetSkipReason(factAttribute);
    }
}