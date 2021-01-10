using log4net;
using MimeKit;
using System;

namespace ImapMailRules.Core
{
    public sealed class MailRule
    {
        static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region XML Attribute MailRule
        public string RuleName;

        public string FilterFrom;
        public string FilterTo;
        public string FilterSubject;
        public string FilterBody;

        public bool MarkRead;
        public MailAction Action;
        public string ActionParam;

        #endregion

        #region CheckRule

        internal bool CheckMailRule()
        {
            bool Check = true;
            _log.Debug(String.Format("Check Mailrulle: {0}", RuleName));
            if (RuleName.Length < 1)
            {
                _log.Error("Config MailRule: RuleName is Empty!");
                Check = false;
            }

            if (FilterFrom == null && FilterTo == null && FilterSubject == null && FilterBody == null)
            {
                _log.Error("Config MailRule: all Filter* is Null!");
                Check = false;
            }
            if (FilterFrom.Length < 1 && FilterSubject.Length < 1 && FilterBody.Length < 1)
            {
                _log.Error("Config MailRule: all Filter* is Empty!");
                Check = false;
            }
            if (Action == MailAction.Move && ActionParam == null && ActionParam.Length < 1)
            {
                _log.Error("Config MailRule: MailAction.Move has no ActionParam!");
                Check = false;
            }
            if (Action == MailAction.Delete && ActionParam != null)
            {
                _log.Error("Config MailRule: MailAction.Delete ActionParam is not Empty!");
                Check = false;
            }
            if (Action == MailAction.MarkRead && ActionParam != null)
            {
                _log.Error("Config MailRule: MailAction.MarkRead ActionParam is not Empty!");
                Check = false;
            }

            return Check;
        }

        #endregion

        #region CheckRuleOnMessage
        
        internal int CheckRuleOnMessage(MimeMessage message)
        {
            // Set to number of Filters
            int FilterCounter = 4;

            if (FilterFrom == "*" || message.From.ToString().Contains(FilterFrom))
            {
                _log.Debug(String.Format("Check Filter: FilterFrom Match Filter: {0} Value: {1}", FilterFrom, message.From));
                FilterCounter--;
            }
            if (FilterTo == "*" || message.To.ToString().Contains(FilterTo))
            {
                _log.Debug(String.Format("Check Filter: FilterTo Match Filter: {0} Value: {1}", FilterTo, message.To));
                FilterCounter--;
            }
            if (FilterSubject == "*" || message.Subject.ToString().Contains(FilterSubject))
            {
                _log.Debug(String.Format("Check Filter: FilterSubject Match Filter: {0} Value: {1}", FilterSubject, message.Subject));
                FilterCounter--;
            }
            if (FilterBody == "*" || message.Body.ToString().Contains(FilterBody))
            {
                _log.Debug(String.Format("Check Filter: FilterBody Match Filter: {0} Value: {1}", FilterBody, message.Body));
                FilterCounter--;
            }

            return FilterCounter;
        }
        #endregion
    }
}