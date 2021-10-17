using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
{
    public struct CertificateLifetime
    {
        public CertificateLifetime(DateTimeOffset? from = null, DateTimeOffset? to = null)
        {
            from ??= DateTimeOffset.UtcNow;
            to ??= from.Value.AddDays(90);

            this.From = from.Value;
            this.To = to.Value;
        }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }
    }
}
