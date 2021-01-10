using System;
using log4net;

namespace ImapMailRules.Core.Exceptions
{
    sealed class ConfigErrorException : FormatException
    {
        static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ConfigErrorException(string message): base(message)
        {
            _log.Fatal(message);
            Environment.Exit(77);
        }
    }
}
