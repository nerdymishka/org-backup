using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public enum X509CertificateAlgorithm
    {
        Unsupported = 0,
        Dsa = 1,
        Ec = 2,
        Rsa = 3,
    }
}
