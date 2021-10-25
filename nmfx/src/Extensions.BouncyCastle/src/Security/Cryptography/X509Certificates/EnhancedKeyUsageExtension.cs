using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class EnhancedKeyUsageExtension : IX509Extension
    {
        private ExtendedKeyUsage extendedKeyUsage;

        public EnhancedKeyUsageExtension(IEnumerable<Oid> enhancedKeyUsage)
        {
            var purposes = new List<DerObjectIdentifier>();
            foreach (var oid in enhancedKeyUsage)
            {
                switch (oid.Value)
                {
                    case Oids.ClientAuthentication:
                        purposes.Add(KeyPurposeID.IdKPClientAuth);
                        break;

                    case Oids.ServerAuthentication:
                        purposes.Add(KeyPurposeID.IdKPServerAuth);
                        break;

                    case Oids.CodeSigning:
                        purposes.Add(KeyPurposeID.IdKPCodeSigning);
                        break;

                    case Oids.TimeStamping:
                        purposes.Add(KeyPurposeID.IdKPTimeStamping);
                        break;

                    case Oids.SecureEmail:
                        purposes.Add(KeyPurposeID.IdKPEmailProtection);
                        break;

                    case Oids.IPsecurityTunnelTermination:
                        purposes.Add(KeyPurposeID.IdKPIpsecTunnel);
                        break;

                    case Oids.IPsecurityUser:
                        purposes.Add(KeyPurposeID.IdKPIpsecUser);
                        break;

                    case Oids.IPsecurityEndSystem:
                        purposes.Add(KeyPurposeID.IdKPIpsecEndSystem);
                        break;

                    case Oids.OCSPSigning:
                        purposes.Add(KeyPurposeID.IdKPOcspSigning);
                        break;

                    case Oids.SmartCardLogon:
                        purposes.Add(KeyPurposeID.IdKPSmartCardLogon);
                        break;

                    case Oids.MacAddress:
                        purposes.Add(KeyPurposeID.IdKPMacAddress);
                        break;

                    default:
                        purposes.Add(new DerObjectIdentifier(oid.Value));
                        break;
                }
            }

            this.extendedKeyUsage = new ExtendedKeyUsage(purposes);
        }

        public bool IsCritical => throw new NotImplementedException();

        public string ObjectIdentifier => throw new NotImplementedException();

        public byte[] Data => this.extendedKeyUsage.GetDerEncoded();
    }
}
