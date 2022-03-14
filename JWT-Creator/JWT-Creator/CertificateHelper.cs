using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Creator
{
    public static class CertificateHelper
    {
        public static string GetPfxPublicKeyString(string pathToPfx, string password)
        {
            var cert = new X509Certificate2(pathToPfx, password);

            byte[] certificateBytes = cert.RawData;

            AsymmetricAlgorithm key = cert.GetRSAPrivateKey();

            byte[] publicKeyBytes = key.ExportSubjectPublicKeyInfo();

            char[] publicKeyPem = PemEncoding.Write("PUBLIC KEY", publicKeyBytes);

            return new string(publicKeyPem);
        }

        public static RsaSecurityKey GetPfxPublicKey(string pathToPfx, string password)
        {
            var collection = new X509Certificate2Collection();

            collection.Import(pathToPfx, password, X509KeyStorageFlags.PersistKeySet);

            var certificate = collection[0];

            var rsaPublicKey = certificate.GetRSAPublicKey();

            return new RsaSecurityKey(rsaPublicKey);
        }

        public static RsaSecurityKey GetPfxPrivateKey(string pathToPfx, string password)
        {
            var collection = new X509Certificate2Collection();

            collection.Import(pathToPfx, password, X509KeyStorageFlags.PersistKeySet);

            var certificate = collection[0];

            var rsaPrivateKey = certificate.GetRSAPrivateKey();

            return new RsaSecurityKey(rsaPrivateKey);
        }

        public static RsaSecurityKey GetPfxPublicKey(X509Certificate2 cert)
        {
            var rsaPublicKey = cert.GetRSAPublicKey();

            return new RsaSecurityKey(rsaPublicKey);
        }

        public static RsaSecurityKey GetPfxPrivateKey(X509Certificate2 cert)
        {
            var rsaPrivateKey = cert.GetRSAPrivateKey();

            return new RsaSecurityKey(rsaPrivateKey);
        }
    }
}
