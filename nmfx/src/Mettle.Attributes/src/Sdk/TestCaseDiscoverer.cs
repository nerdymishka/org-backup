using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle.Sdk
{
    public class TestCaseDiscoverer : FactDiscoverer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseDiscoverer"/> class.
        /// </summary>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages.</param>
        public TestCaseDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        [SuppressMessage("SonarQube", "RCS1096:", Justification = "By Design")]
        protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var category = factAttribute.GetNamedArgument<string?>("Category");
            var tags = factAttribute.GetNamedArgument<string[]?>("Tags");
            var ticketUri = factAttribute.GetNamedArgument<string?>("TicketUri");
            var ticketId = factAttribute.GetNamedArgument<string?>("TicketId");
            var ticketKind = factAttribute.GetNamedArgument<string?>("TicketKind");
            var documentUri = factAttribute.GetNamedArgument<string?>("DocumentUri");
            var platforms = factAttribute.GetNamedArgument<TestPlatforms>("Platforms");
            var runtimes = factAttribute.GetNamedArgument<TestRuntimes>("Runtimes");
            var frameworks = factAttribute.GetNamedArgument<DotNetFrameworks>("Frameworks");

            var configurations = factAttribute.GetNamedArgument<RuntimeConfigurations>("Configurations");

            StringBuilder? sb = null;

            var traits = new Dictionary<string, List<string>>();

            if (!platforms.HasFlag(TestPlatforms.Any))
            {
                var formatted = platforms.ToString("{0:G}");
                traits.Add("platforms", formatted);
                if (!DiscovererHelpers.TestPlatformApplies(platforms))
                {
                    sb = new StringBuilder();
                    sb.Append("Required Platforms: ").Append(formatted);
                }
            }

            if (!runtimes.HasFlag(TestRuntimes.Any))
            {
                var formatted = runtimes.ToString("{0:G}");
                traits.Add("runtimes", formatted);
                if (!DiscovererHelpers.TestRuntimeApplies(runtimes))
                {
                    sb ??= new StringBuilder();
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append("Required Runtimes: ").Append(formatted);
                }
            }

            if (!frameworks.HasFlag(DotNetFrameworks.Any))
            {
                var formatted = frameworks.ToString("{0:G}");
                traits.Add("frameworks", formatted);
                if (!DiscovererHelpers.TestFrameworkApplies(frameworks))
                {
                    sb ??= new StringBuilder();
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append("Required Frameworks: ").Append(formatted);
                }
            }

            if (!configurations.HasFlag(RuntimeConfigurations.Any))
            {
                var formatted = configurations.ToString("{0:G}");
                traits.Add("configurations", formatted);
                if (!DiscovererHelpers.RuntimeConfigurationApplies(configurations))
                {
                    sb ??= new StringBuilder();
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append("Required Configurations: ").Append(formatted);
                }
            }

            string? skipReason = null;

            if (sb?.Length > 0)
                skipReason = sb.ToString();

            var test = new SkippedTestCase(
                skipReason,
                this.DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod);

            foreach (var key in traits.Keys)
            {
                test.Traits.Add(key, traits[key]);
            }

            if (tags != null)
            {
                if (!test.Traits.TryGetValue("tags", out var list))
                {
                    list = new List<string>(tags);
                    test.Traits.Add("tags", list);
                }
                else
                {
                    list.AddRange(tags);
                }

                if (category != null)
                    list.Add(category);
            }

            if (!ticketUri.IsNullOrWhiteSpace())
                test.Traits.Add("ticket-uri", ticketUri);

            if (!ticketId.IsNullOrWhiteSpace())
                test.Traits.Add("ticket-id", ticketId);

            if (!ticketKind.IsNullOrWhiteSpace())
                test.Traits.Add("ticket-kind", ticketKind);

            if (!documentUri.IsNullOrWhiteSpace())
                test.Traits.Add("ticket-kind", documentUri);

            return test;
        }
    }
}