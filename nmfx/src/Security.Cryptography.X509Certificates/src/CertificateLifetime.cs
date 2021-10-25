using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public struct CertificateLifetime
    {
        public CertificateLifetime(DateTimeOffset? notBefore = null, DateTimeOffset? notAfter = null)
        {
            notBefore ??= DateTimeOffset.UtcNow;
            notAfter ??= notBefore.Value.AddDays(90);

            this.NotBefore = notBefore.Value;
            this.NotAfter = notAfter.Value;
        }

        public CertificateLifetime(TimeSpan span)
        {
            var now = DateTimeOffset.UtcNow;
            this.NotBefore = now;
            this.NotAfter = now.Add(span);
        }

        [SuppressMessage("Readability Rules", "SA1129:Do not use default type constructor", Justification = "Constuctor has defaults")]
        public static CertificateLifetime Default { get; } = new CertificateLifetime();

        public DateTimeOffset NotBefore { get; set; }

        public DateTimeOffset NotAfter { get; set; }

        public static implicit operator CertificateLifetime(TimeSpan span)
        {
            return new CertificateLifetime(span);
        }

        public static implicit operator CertificateLifetime(long ticks)
        {
            return new CertificateLifetime(new TimeSpan(ticks));
        }

        public static implicit operator CertificateLifetime(DateTimeOffset notAfter)
        {
            return new CertificateLifetime(DateTimeOffset.UtcNow, notAfter);
        }

        public static implicit operator CertificateLifetime(DateTime notAfter)
        {
            return new CertificateLifetime(DateTimeOffset.UtcNow, notAfter);
        }

        public static CertificateLifetime AsOneYear()
        {
            return new CertificateLifetime(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));
        }

        public static CertificateLifetime AsFiveYears()
        {
            return new CertificateLifetime(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5));
        }

        public static CertificateLifetime AsTenYears()
        {
            return new CertificateLifetime(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(10));
        }
    }
}
