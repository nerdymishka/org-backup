using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
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
            this.CommonName = commonName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistinguishedName"/> class.
        /// </summary>
        /// <param name="subjectAlternativeName">The subject altnerative name.</param>
        /// <exception cref="ArgumentException">Throws when <paramref name="subjectAlternativeName"/> does not have one DNS name.</exception>
        public DistinguishedName(SubjectAlternativeName subjectAlternativeName)
        {
            if (subjectAlternativeName.DnsName.Count == 0)
                throw new ArgumentException("subnameAltnerativeName requires at least one DNS name.");

            this.CommonName = subjectAlternativeName.DnsName.First();
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
        ///  Gets or sets the common name (CN).
        /// </summary>
        public string CommonName { get; set; }
    }
}
