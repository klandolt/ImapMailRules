using System;
using log4net;

namespace ImapMailRules.Core.Exceptions
{
    sealed class NotConnectedException : System.IO.IOException
    {
        static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public NotConnectedException(string message) : base(message)
        {
            _log.Fatal(message);
            Environment.Exit(11);
        }
        public NotConnectedException(string message, Exception innerException) : base(message,innerException)
        {
            _log.Fatal(message + innerException.Message);
            Environment.Exit(12);
        }
    }
}
