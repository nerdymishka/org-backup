using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    /// <summary>
    /// Represents the certificate's distinquished name. A common name (CN) is required.
    /// </summary>
    public class DistinguishedName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistinguishedName"/> class.
        /// </summary>
        public DistinguishedName(string commonName)
        {
        }

        /// <summary>
        ///  Gets or sets the country (C).
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the state or province (ST).
        /// </summary>
        public string? StateProvince { get; set; }

        /// <summary>
        /// Gets or sets the locality (L).
        /// </summary>
        public string? Locality { get; set; }

        /// <summary>
        /// Gets or sets the organization (O).
        /// </summary>
        public string? Organization { get; set; }

        /// <summary>
        /// Gets or the organization units (OU).
        /// </summary>
        public List<string> OrganizationUnits { get; } = new List<string>();

        public List<string> DomainComponents { get; } = new List<string>();

        /// <summary>
        ///  Gets the common name (CN).
        /// </summary>
        public List<string> CommonNames { get; } = new List<string>();

        public string PostalCode { get; set; }

        public string SerialNumber { get; set; }

        public string Title { get; set; }

        public string Qualifier { get; set; }

        public string UserIdentifier { get; set; }

        public string Street { get; set; }

        public X500DistinguishedName ToX500DistinguishedName()
        {
            return new X500DistinguishedName(this.ToString());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var cn in this.CommonNames)
            {
                if (string.IsNullOrWhiteSpace(cn))
                    continue;

                if (sb.Length > 0)
                    sb.Append(',');

                sb.Append(" CN=").Append(cn);
            }

            if (!string.IsNullOrWhiteSpace(this.Qualifier))
            {
                sb.Append(", DNQ=")
                    .Append(this.Qualifier);
            }

            if (!string.IsNullOrWhiteSpace(this.Organization))
            {
                sb.Append(", O=")
                    .Append(this.Organization);
            }

            foreach (var ou in this.OrganizationUnits)
            {
                if (string.IsNullOrWhiteSpace(ou))
                    continue;

                sb.Append(", OU=").Append(ou);
            }

            foreach (var dc in this.DomainComponents)
            {
                if (string.IsNullOrWhiteSpace(dc))
                    continue;

                sb.Append(", DC=").Append(dc);
            }

            if (!string.IsNullOrWhiteSpace(this.Street))
            {
                sb.Append(", STREET=").Append(this.Street);
            }

            if (!string.IsNullOrEmpty(this.Locality))
            {
                sb.Append(", L=")
                    .Append(this.Locality);
            }

            if (!string.IsNullOrEmpty(this.StateProvince))
            {
                sb.Append(", ST=")
                    .Append(this.StateProvince);
            }

            if (!string.IsNullOrEmpty(this.PostalCode))
            {
                sb.Append(", PC=").Append(this.PostalCode);
            }

            if (!string.IsNullOrWhiteSpace(this.Country))
            {
                sb.Append(", C=")
                    .Append(this.Country);
            }

            return sb.ToString();
        }
    }
}
