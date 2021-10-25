using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    // https://github.com/dotnet/runtime/blob/8c0a1d91986f20d49ebfa07bd39ae60b9dd31394/src/libraries/System.Security.Cryptography.X509Certificates/tests/CertificateCreation/DSAX509SignatureGenerator.cs
    [SuppressMessage("SonarQube", "S101:Types should be named in PascalCase", Justification = "DSA and X509 are a standard name")]
    internal sealed class DSAX509SignatureGenerator : X509SignatureGenerator
    {
        private readonly DSA key;

        public DSAX509SignatureGenerator(DSA key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            this.key = key;
        }

        public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
        {
            if (hashAlgorithm == HashAlgorithmName.SHA1)
            {
                return "300906072A8648CE380403".HexToByteArray();
            }

            if (hashAlgorithm == HashAlgorithmName.SHA256)
            {
                return "300B0609608648016503040302".HexToByteArray();
            }

            throw new InvalidOperationException();
        }

        public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
        {
            byte[] ieeeFormat = this.key.SignData(data, hashAlgorithm);

            Debug.Assert(ieeeFormat.Length % 2 == 0, "Length should be a factor of 2");
            int segmentLength = ieeeFormat.Length / 2;

            byte[] r = this.EncodeUnsignedInteger(ieeeFormat, 0, segmentLength);
            byte[] s = this.EncodeUnsignedInteger(ieeeFormat, segmentLength, segmentLength);

            return
                new byte[] { 0x30 }.
                Concat(EncodeLength(r.Length + s.Length)).
                Concat(r).
                Concat(s).
                ToArray();
        }

        protected override PublicKey BuildPublicKey()
        {
            // DSA
            Oid oid = new Oid("1.2.840.10040.4.1");

            DSAParameters dsaParameters = this.key.ExportParameters(false);

            // Dss-Parms ::= SEQUENCE {
            //   p INTEGER,
            //   q INTEGER,
            //   g INTEGER
            // }
            byte[] p = this.EncodeUnsignedInteger(dsaParameters.P);
            byte[] q = this.EncodeUnsignedInteger(dsaParameters.Q);
            byte[] g = this.EncodeUnsignedInteger(dsaParameters.G);

            byte[] algParameters =
                new byte[] { 0x30 }.
                    Concat(EncodeLength(p.Length + q.Length + g.Length)).
                    Concat(p).
                    Concat(q).
                    Concat(g).
                    ToArray();

            byte[] keyValue = this.EncodeUnsignedInteger(dsaParameters.Y);

            return new PublicKey(
                oid,
                new AsnEncodedData(oid, algParameters),
                new AsnEncodedData(oid, keyValue));
        }

        private static byte[] EncodeLength(int length)
        {
            Debug.Assert(length >= 0, "length should not be zero");

            byte low = unchecked((byte)length);

            // If the length value fits in 7 bits, it's an answer all by itself.
            if (length < 0x80)
            {
                return new[] { low };
            }

            if (length <= 0xFF)
            {
                return new byte[] { 0x81, low };
            }

            int remainder = length >> 8;
            byte midLow = unchecked((byte)remainder);

            if (length <= 0xFFFF)
            {
                return new byte[] { 0x82, midLow, low };
            }

            remainder >>= 8;
            byte midHigh = unchecked((byte)remainder);

            if (length <= 0xFFFFFF)
            {
                return new byte[] { 0x83, midHigh, midLow, low };
            }

            remainder >>= 8;
            byte high = unchecked((byte)remainder);

            // Since we know this was a non-negative signed number, the highest
            // legal value here is 0x7F.
            Debug.Assert(remainder < 0x80, "value should not be higher than 0x7F");

            return new byte[] { 0x84, high, midHigh, midLow, low };
        }

        private byte[] EncodeUnsignedInteger(byte[]? data)
        {
            if (data == null)
                return Array.Empty<byte>();

            return this.EncodeUnsignedInteger(data, 0, data.Length);
        }

        private byte[] EncodeUnsignedInteger(byte[] data, int offset, int count)
        {
            int length = count;
            int realOffset = offset;
            bool paddingByte = false;

            if (count == 0 || data[offset] >= 0x80)
            {
                paddingByte = true;
            }
            else
            {
                while (length > 1 && data[realOffset] == 0)
                {
                    realOffset++;
                    length--;
                }
            }

            byte encodedLength = (byte)length;

            if (paddingByte)
            {
                encodedLength++;
            }

            IEnumerable<byte> bytes = new byte[] { 0x02 };
            bytes = bytes.Concat(EncodeLength(encodedLength));

            if (paddingByte)
            {
                bytes = bytes.Concat(new byte[1]);
            }

            bytes = bytes.Concat(data.Skip(realOffset).Take(length));

            return bytes.ToArray();
        }
    }
}
