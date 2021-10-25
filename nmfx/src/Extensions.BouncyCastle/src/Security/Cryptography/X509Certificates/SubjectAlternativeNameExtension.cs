using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class SubjectAlternativeNameExtension : IX509Extension
    {
        private readonly GeneralNames names;

        public SubjectAlternativeNameExtension(SubjectAlternativeName subjectAlternativeName)
        {
            var set = new List<GeneralName>();
            foreach (var dns in subjectAlternativeName.DnsNames)
                set.Add(new GeneralName(GeneralName.DnsName, dns));

            foreach (var email in subjectAlternativeName.Emails)
                set.Add(new GeneralName(GeneralName.Rfc822Name, email));

            foreach (var uri in subjectAlternativeName.Uris)
                set.Add(new GeneralName(GeneralName.UniformResourceIdentifier, uri.ToString()));

            foreach (var ip in subjectAlternativeName.IpAddress)
                set.Add(new GeneralName(GeneralName.IPAddress, ip.ToString()));

            foreach (var upn in subjectAlternativeName.Upns)
            {
                var other = new OtherName(new DerObjectIdentifier(Oids.UserPrincipalName), new DerUtf8String(upn));
                set.Add(new GeneralName(GeneralName.OtherName, other));
            }

            this.names = new GeneralNames(set.ToArray());
        }

        public bool IsCritical => true;

        public string ObjectIdentifier => X509Extensions.SubjectAlternativeName.Id;

        public byte[] Data => this.names.GetDerEncoded();
    }
}
