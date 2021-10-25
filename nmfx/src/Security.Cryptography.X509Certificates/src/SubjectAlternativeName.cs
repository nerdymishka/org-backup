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

        public SubjectAlternativeName(IEnumerable<string> dnsNames, string email)
        {
            this.DnsNames = new List<string>(dnsNames);
            this.Email = email;
        }

        /// <summary>
        /// Gets the Dns Names. At least 1 dns name is required.
        /// </summary>
        public List<string> DnsNames { get; private set; }

        /// <summary>
        /// Gets or sets the email address. Optional.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Converts the model to a SAN <see cref="X509Extension"/>.
        /// </summary>
        /// <returns>The <see cref="X509Extension"/> representation of a Subject Altnerative Name.</returns>
        public X509Extension ToX509Extension()
        {
            var sanBuilder = new SubjectAlternativeNameBuilder();
            foreach (var dnsName in this.DnsNames)
            {
                sanBuilder.AddDnsName(dnsName);
            }

            if (!string.IsNullOrEmpty(this.Email))
            {
                sanBuilder.AddEmailAddress(this.Email);
            }

            return sanBuilder.Build();
        }
    }
}
