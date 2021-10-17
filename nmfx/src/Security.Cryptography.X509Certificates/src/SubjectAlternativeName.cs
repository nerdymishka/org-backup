using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
{
    public class SubjectAlternativeName
    {
        /// <summary>
        /// Gets or sets the Dns Name. At least 1 dns name is required.
        /// </summary>
        public List<string> DnsName { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the email address. Optional.
        /// </summary>
        public string? Email { get; set; }
    }
}
