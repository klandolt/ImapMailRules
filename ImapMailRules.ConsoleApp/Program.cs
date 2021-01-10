using System;
using ImapMailRules.Core;

namespace ImapMailRules.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Argument 
            string ParamConfigFile = String.Empty;
            bool ParamSilent = false;
            
            if(args.Length >0)
            {
                int ArgCount = args.Length;
                

                if(ArgCount == 1)
                {
                    if (args[0].ToLower().Contains("silent"))
                    {
                        ParamSilent = true;
                    }
                    else
                    {
                        // Get First Argument:
                        ParamConfigFile = args[0];
                    }
                }else if (ArgCount == 2)
                {
                    // Check First Argument
                    if (args[0].ToLower().Contains("silent"))
                    {
                        ParamSilent = true;
                    }
                    else
                    {
                        // Get First Argument:
                        ParamConfigFile = args[0];
                    }
                    // Check Second Argument
                    if (args[1].ToLower().Contains("silent"))
                    {
                        ParamSilent = true;
                    }
                    else
                    {
                        // Get Second Argument:
                        ParamConfigFile = args[1];
                    }
                }
            }

            ConsoleOutput("Hello to ImapMailRule Tool!", ParamSilent);

            if (!ParamSilent)
            {
                int ArgCount = 0;
                foreach (string arg in args)
                {
                    ArgCount++;
                    Console.WriteLine(String.Format("Argument {0} Value: {1}", ArgCount, arg));
                }
            }
                        

            MailConfig config = new MailConfig();

            if (ParamConfigFile.Length>0)
            {
                config = config.ReadConfigFile(ParamConfigFile);
            }
            else
            {
                config = config.ReadConfigFile();
            }
            
            config.CheckMails();

            ConsoleOutput("End of ImapMailRule Tool!", ParamSilent);

        }

        public static void ConsoleOutput(string Message, bool Silent)
        {
            if (!Silent)
            {
                Console.WriteLine(Message);
            }
        }
    }
}
