using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;
using CustomExtensions;
using System.Collections.Generic;
using CustomCliExtensions;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace JWT_Creator
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Starting....");
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

            var arguments = args.ParseArgs();

            UpdateSettings(settings, arguments);

            if (arguments.Switches.Contains(nameof(Constants.Aruments.HELP)))
            {
                PrintHelp();
                return;
            }

            CreateAndValidateJWT(settings);
            
            Console.ReadLine();
        }

        private static void CreateAndValidateJWT(Settings settings, bool azureCert = false)
        {
            RsaSecurityKey privateKey;
            RsaSecurityKey publicKey;

            if (!azureCert)
            {
                privateKey = CertificateHelper.GetPfxPrivateKey(settings.PfxPath, settings.PfxPassword);
                publicKey = CertificateHelper.GetPfxPublicKey(settings.PfxPath, settings.PfxPassword);
            } else
            {
                // call to get certificate from Azure KeyVault would go here.
                // would need to have this app registered with Azure Active Directory
                // to use this feature.
                privateKey = null;
                publicKey = null;
                Console.WriteLine($"Getting Certificates from Azure Key Vault has not been implemented\n");
                return;
            }

            var token = JwtHelper.GenerateJwtTokenWithAsymmetricKey_RSA256(
                    privateKey,
                    settings.Issuer,
                    settings.Audience,
                    settings.GetClaims()
                );

            var validated = JwtHelper.ValidateJwtTokenWithAsymmetricKey_RSA256(
                token,
                publicKey,
                settings.Issuer,
                settings.Audience,
                out string message);

            if (validated)
            {
                Console.WriteLine($"Valid Token:\nBearer {token}\n");
            } else
            {
                Console.WriteLine($"Token could not be validated [ {message} ].");
            }
            
        }

        /// <summary>
        /// Applies any overrides from CLI arguments.
        /// </summary>
        /// <param name="settings">Settings object loaded from appsettings.json</param>
        /// <param name="arguments">Object contianing arguments read from the commandline</param>
        private static void UpdateSettings(Settings settings, CliArguments arguments)
        {
            if (arguments == null)
                return;

            if (arguments.Switches.Contains(nameof(Constants.Aruments.AZURE)))
            {
                if(arguments.Args.ContainsKey(nameof(Constants.Aruments.KEYVAULT)))
                    settings.AzureKeyVaultName = 
                        arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.KEYVAULT));

                if (arguments.Args.ContainsKey(nameof(Constants.Aruments.CERTNAME)))
                    settings.CertificateName =
                        arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.CERTNAME));
            }

            if (arguments.Args.ContainsKey(nameof(Constants.Aruments.ISSUER)))
                settings.Issuer =
                    arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.ISSUER));

            if (arguments.Args.ContainsKey(nameof(Constants.Aruments.AUDIENCE)))
                settings.Audience =
                    arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.AUDIENCE));

            if (arguments.Args.ContainsKey(nameof(Constants.Aruments.PFX)))
                settings.PfxPath =
                    arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.PFX));

            if (arguments.Args.ContainsKey(nameof(Constants.Aruments.PASSWORD)))
                settings.PfxPassword =
                    arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.PASSWORD));

            if (arguments.Args.ContainsKey(nameof(Constants.Aruments.CLAIMS)))
            {
                string claimsJsonString =
                    arguments.Args.GetValueOrDefault(nameof(Constants.Aruments.CLAIMS));

                // perhaps some JSON validation here.
                List<ClaimSetting> claims = new List<ClaimSetting>();
                
                claims = JsonConvert.DeserializeObject<List<ClaimSetting>>(claimsJsonString);

                settings.Claims = claims;
            }
                

        }

        private static void PrintHelp()
        {
            Console.WriteLine("===== App Help Information =====\n");
            Console.WriteLine("This app supports CLI arguments which are used to override values in appsettings.json.\n=====");
            foreach (string argName in Enum.GetNames(typeof(Constants.Aruments)))
            {
                string message = string.Empty;
                switch (argName) {
                    case nameof(Constants.Aruments.HELP):
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: Displays HELP information");
                        break;
                    case nameof(Constants.Aruments.AZURE):
                        message = $"Switch instructing app to extract Certificate from Azure Key Vault. [NOT IMPLEMENTED]";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.PFX):
                        message = "the pathname where a Windows .pfx certificate bundle can be found.";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.PASSWORD):
                        message = "the password used to access a Windows .pfx certificate bundle.";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.ISSUER):
                        message = "The JWT issuer.";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.AUDIENCE):
                        message = "The JWT audience";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.KEYVAULT):
                        message = "Aure Key Vault name used with the --azure switch. [NOT IMPLEMENTED]";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.CERTNAME):
                        message = "Azure Key Vault Certificate Name used with the --azure switch. [NOT IMPLEMENTED]";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    case nameof(Constants.Aruments.CLAIMS):
                        message = "JSON string containing security claims. Format: [ { \"ClaimType\" : \"aClaimType\", \"ClaimValue\" : \"aClaimValue\" } ]";
                        Console.WriteLine($"--{argName.ToLowerInvariant()}: {message}");
                        break;
                    default:
                        break;
                }

            }
        }

        
    }

    
}


