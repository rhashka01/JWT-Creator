using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CustomExtensions;

namespace CustomCliExtensions
{
    public static class CliExtensions
    {
        public static CliArguments ParseArgs(this string[] cliArgs)
        {
            var arguments = new CliArguments();
            Regex regex = new Regex(@"^--[a-zA-Z]*");
            for (int i = 0; i < cliArgs.Length; i++)
            {
                if (regex.IsMatch(cliArgs[i]))
                {
                    var argToken = cliArgs[i].Replace("-", "");
                    switch (argToken.ToUpperInvariant())
                    {
                        case nameof(Constants.Aruments.HELP):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.HELP));
                            break;
                        case nameof(Constants.Aruments.AZURE):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.AZURE));
                            break;
                        case nameof(Constants.Aruments.PFX):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.PFX));
                            break;
                        case nameof(Constants.Aruments.PASSWORD):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.PASSWORD));
                            break;
                        case nameof(Constants.Aruments.ISSUER):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.ISSUER));
                            break;
                        case nameof(Constants.Aruments.AUDIENCE):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.AUDIENCE));
                            break;
                        case nameof(Constants.Aruments.KEYVAULT):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.KEYVAULT));
                            break;
                        case nameof(Constants.Aruments.CERTNAME):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.CERTNAME));
                            break;
                        case nameof(Constants.Aruments.CLAIMS):
                            AddArgument(arguments, cliArgs, i, nameof(Constants.Aruments.CLAIMS));
                            break;
                        default:
                            Console.Error.WriteLine($"Unknown argument: {argToken}");
                            break;
                    }
                }
            }

            return arguments;
        }


        private static void AddArgument(CliArguments args, string[] argArray, int index, string argName)
        {
            Regex regex = new Regex(@"^--[a-zA-Z]*");

            if (argArray.TryGet((index + 1), out string argValue))
            {
                if(regex.IsMatch(argValue))
                {
                    args.Add(argName);
                } else
                {
                    args.Add(argName, argValue);
                }
            }
            else
            {
                args.Add(argName);
            }
        }
    }

    public class CliArguments
    {
        private readonly Dictionary<string, string> _args;
        private readonly HashSet<string> _switches;

        public CliArguments()
        {
            _args = new Dictionary<string, string>();
            _switches = new HashSet<string>();
        }

        public Dictionary<string, string> Args
        {
            get
            {
                return _args;
            }
        }

        public HashSet<string> Switches
        {
            get
            {
                return _switches;
            }
        }

        public void Add(string switchName)
        {
            _switches.Add(switchName);
        }

        public void Add(string argName, string argValue)
        {
            _args.Add(argName, argValue);
        }
    }


    public class Constants
    {
        public enum Aruments
        {
            HELP,
            AZURE,
            PFX,
            PASSWORD,
            ISSUER,
            AUDIENCE,
            KEYVAULT,
            CERTNAME,
            CLAIMS
        };
    }
}
