using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Mettle.Sdk
{
    [SuppressMessage("SonarQube", "RCS1096:", Justification = "By Design")]
    internal static class DiscovererHelpers
    {
        private static readonly Lazy<bool> s_isMonoRuntime = new Lazy<bool>(() => Type.GetType("Mono.RuntimeStructs") != null);

        public static bool IsMonoRuntime => s_isMonoRuntime.Value;

        public static bool IsRunningOnNetCoreApp { get; } = Environment.Version.Major >= 5 || RuntimeInformation.FrameworkDescription.StartsWith(".NET", StringComparison.OrdinalIgnoreCase);

        public static bool IsRunningOnNetFramework { get; } = RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

        public static bool TestPlatformApplies(TestPlatforms platforms) =>
                platforms.HasFlag(TestPlatforms.Any) ||
                (platforms.HasFlag(TestPlatforms.FreeBSD) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD"))) ||
                (platforms.HasFlag(TestPlatforms.Linux) && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ||
                (platforms.HasFlag(TestPlatforms.NetBSD) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD"))) ||
                (platforms.HasFlag(TestPlatforms.OSX) && RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) ||
                (platforms.HasFlag(TestPlatforms.Illumos) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("ILLUMOS"))) ||
                (platforms.HasFlag(TestPlatforms.Solaris) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("SOLARIS"))) ||
                (platforms.HasFlag(TestPlatforms.IOS) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")) && !RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST"))) ||
                (platforms.HasFlag(TestPlatforms.TVOS) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS"))) ||
                (platforms.HasFlag(TestPlatforms.MacCatalyst) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST"))) ||
                (platforms.HasFlag(TestPlatforms.Android) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"))) ||
                (platforms.HasFlag(TestPlatforms.Browser) && RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"))) ||
                (platforms.HasFlag(TestPlatforms.Windows) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

        public static bool TestRuntimeApplies(TestRuntimes runtimes) =>
                (runtimes.HasFlag(TestRuntimes.Mono) && IsMonoRuntime) ||
                (runtimes.HasFlag(TestRuntimes.CoreCLR) && !IsMonoRuntime); // assume CoreCLR if it's not Mono

        public static bool TestFrameworkApplies(DotNetFrameworks frameworks) =>
                (frameworks.HasFlag(DotNetFrameworks.NetCore) && IsRunningOnNetCoreApp) ||
                (frameworks.HasFlag(DotNetFrameworks.NetFramework) && IsRunningOnNetFramework);

        public static bool RuntimeConfigurationApplies(RuntimeConfigurations configurations) =>
            configurations.HasFlag(RuntimeConfigurations.Any) ||
            (configurations.HasFlag(RuntimeConfigurations.Release) && IsReleaseRuntime()) ||
            (configurations.HasFlag(RuntimeConfigurations.Debug) && IsDebugRuntime()) ||
            (configurations.HasFlag(RuntimeConfigurations.Checked) && IsCheckedRuntime());

        private static bool IsCheckedRuntime() => AssemblyConfigurationEquals("Checked");

        private static bool IsReleaseRuntime() => AssemblyConfigurationEquals("Release");

        private static bool IsDebugRuntime() => AssemblyConfigurationEquals("Debug");

        private static bool AssemblyConfigurationEquals(string configuration)
        {
            var assemblyConfigurationAttribute = typeof(string).Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

            return assemblyConfigurationAttribute != null &&
                string.Equals(assemblyConfigurationAttribute.Configuration, configuration, StringComparison.InvariantCulture);
        }
    }
}