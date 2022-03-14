using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
