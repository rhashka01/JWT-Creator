using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Creator
{
    public class Settings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }    
        public string PfxPath { get; set; } 
        public string PfxPassword { get; set; }
        public string AzureKeyVaultName { get; set; }   
        public string CertificateName { get; set; }
        public IEnumerable<ClaimSetting> Claims { get; set; }

        public IEnumerable<Claim> GetClaims()
        {
            var claims = new List<Claim>();
            foreach (var claim in Claims)
            {
                claims.Add(new Claim(claim.ClaimType, claim.ClaimValue));
            }
            return claims;
        }
    }

    public class ClaimSetting
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
