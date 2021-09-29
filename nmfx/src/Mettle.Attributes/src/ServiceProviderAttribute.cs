using System;

namespace Mettle
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false,
        AllowMultiple = true)]
    public sealed class ServiceProviderAttribute : System.Attribute
    {
        public ServiceProviderAttribute(string name)
        {
            this.ServiceProviderName = name;
        }

        public ServiceProviderAttribute(Type factoryType)
        {
            this.ServiceProviderFactoryType = factoryType;
        }

        public Type? ServiceProviderFactoryType { get; set; }

        public string? ServiceProviderName { get; set; }
    }
}