using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public interface IX509Extension
    {
        public bool IsCritical { get; }

        public string ObjectIdentifier { get; }

        public byte[] Data { get; }
    }
}
