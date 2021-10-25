using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class SubjectAlternativeName
    {
        public SubjectAlternativeName()
        {
            this.DnsNames = new List<string>();
        }

        public SubjectAlternativeName(params string[] dnsNames)
        {
            this.DnsNames = new List<string>();
            if (dnsNames != null && dnsNames.Length > 0)
                this.DnsNames.AddRange(dnsNames);
        }

        public SubjectAlternativeName(IEnumerable<string> dnsNames)
        {
            this.DnsNames = new List<string>(dnsNames);
        }

        /// <summary>
        /// Gets the Dns Names. At least 1 dns name is required.
        /// </summary>
        public List<string> DnsNames { get; }

        public List<string> Emails { get; } = new List<string>();

        public List<System.Net.IPAddress> IpAddress { get; } = new List<System.Net.IPAddress>();

        public List<string> Upns { get; } = new List<string>();

        public List<Uri> Uris { get; } = new List<Uri>();
    }
}
