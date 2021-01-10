using System;
using System.IO;
using log4net;

namespace ImapMailRules.Core.Exceptions
{
    sealed class ConfigFileNotFoundException : FileNotFoundException
    {
        static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ConfigFileNotFoundException(string message):base(message)
        {
            _log.Fatal(message);
            Environment.Exit(99);
        }
    }
}
