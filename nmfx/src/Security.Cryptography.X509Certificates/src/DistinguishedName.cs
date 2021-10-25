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
        /// <param name="commonName">The common name.</param>
        public DistinguishedName(string commonName)
        {
            if (string.IsNullOrWhiteSpace(commonName))
                throw new ArgumentNullException(nameof(commonName));

            this.CommonName = commonName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinguishedName"/> class.
        /// </summary>
        /// <param name="subjectAlternativeName">The subject altnerative name.</param>
        /// <exception cref="ArgumentException">Throws when <paramref name="subjectAlternativeName"/> does not have one DNS name.</exception>
        public DistinguishedName(SubjectAlternativeName subjectAlternativeName)
        {
            if (subjectAlternativeName.DnsNames.Count == 0)
                throw new ArgumentException("subnameAltnerativeName requires at least one DNS name.");

            this.CommonName = subjectAlternativeName.DnsNames.First();
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
        public string? Organisation { get; set; }

        /// <summary>
        /// Gets or sets the organization unit (OU).
        /// </summary>
        public string? OrganisationUnit { get; set; }

        /// <summary>
        ///  Gets the common name (CN).
        /// </summary>
        public string CommonName { get; private set; }

        public string? FriendlyName { get; set; }

        public X500DistinguishedName ToX500DistinguishedName()
        {
            return new X500DistinguishedName(this.ToString());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var sb = new StringBuilder($"CN=").Append(this.CommonName);

            if (!string.IsNullOrWhiteSpace(this.Country))
            {
                sb.Append(", C=")
                    .Append(this.Country);
            }

            if (!string.IsNullOrWhiteSpace(this.Organisation))
            {
                sb.Append(", O=")
                    .Append(this.Organisation);
            }

            if (!string.IsNullOrWhiteSpace(this.OrganisationUnit))
            {
                sb.Append(", OU=")
                    .Append(this.OrganisationUnit);
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

            return sb.ToString();
        }
    }
}
