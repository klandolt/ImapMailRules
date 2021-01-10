using ImapMailRules.Core.Exceptions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using log4net;
using MimeKit;

namespace ImapMailRules.Core
{
    public sealed class MailConfig
    {
        static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string CONFIGFILE = "config.xml";
        private ImapClient _client = new ImapClient();
        #region XML Attribute Config
        public string ImapServer;
        public int ImapPort;
        public bool ImapSSL;
        public string ImapUser;
        public string ImapPassword;

        public List<MailRule> MailRules;

        public string LogMail;
        #endregion

        #region ReadConfigFile
        public MailConfig ReadConfigFile()
        {
            return ReadConfigFile(CONFIGFILE);
        }
        public MailConfig ReadConfigFile(string ConfigFile)
        {
            MailConfig config;
            if(File.Exists(ConfigFile))
            {
                _log.Debug("DeSerialize ConfigFile");
                XmlSerializer myDeSerializer = new XmlSerializer(typeof(MailConfig));
                //Filestream herstellen
                FileStream myFileStream = new FileStream(ConfigFile, FileMode.Open);
                //Objekt erzeugen
                config = (MailConfig)myDeSerializer.Deserialize(myFileStream);
                //Filestream schliessen
                myFileStream.Close();

                if(!config.CheckConfig())
                {
                    throw new ConfigErrorException(String.Format("Config content Error!! ConfigFile: {0}", ConfigFile));
                }
                else
                {
                    _log.Info("ConfigFile found & checked successful!");
                }
            }
            else
            {
                throw new ConfigFileNotFoundException(String.Format("No ConfigFile found!! ConfigFile: {0}",ConfigFile));
            }
            return config;
        }
        #endregion

        #region Check ConfigFile
        public bool CheckConfig()
        {
            _log.Debug("Beginn CheckConfig File");
            bool check = true;

            // Check Not Empty an existig Config Parameteter
            if(ImapServer.Length < 1)
            {
                _log.Error("Config: ImapServer is Empty!");
                check = false;    
            }
            if(ImapPort < 1 )
            {
                _log.Error("Config: ImapPort is Empty!");
                check = false;
            }
            if(ImapUser.Length < 1)
            {
                _log.Error("Config: ImapUser is Empty!");
                check = false;
            }
            if(ImapPassword.Length <1)
            {
                _log.Error("Config: ImapPassword is Empty!");
                check = false;
            }

            if(!CheckMailRules())
            {
                _log.Error("CheckMailRules Fails!");
                check = false;
            }
            _log.Debug("End CheckConfig File");
            return check;
        }

        private bool CheckMailRules()
        {
            _log.Debug("Beginn CheckMailRules File");
            bool check = true;

            if(MailRules.Count == 0)
            {
                _log.Error("Config: MailRules is Empty!");
                return false;
            }

            foreach(MailRule rule in MailRules)
            {
                if(!rule.CheckMailRule())
                {
                    check = rule.CheckMailRule();
                }
            }
            _log.Debug("End CheckMailRules File");
            return check;
        }
        #endregion

        #region Connection
        private void Connect2Server()
        {
            _log.Debug("Beginn Connect2Server");
            try
            {
                _client.Connect(ImapServer, ImapPort, ImapSSL);
                _client.Authenticate(ImapUser, ImapPassword);
            }catch(Exception ex)
            {
                throw new NotConnectedException("Could not connect to Server! Look at inner Exception:", ex);
            }
            

            if(!_client.IsConnected)
            {
                throw new NotConnectedException("No Connection to the Server after Connect & Authenticate!");
            }
            _log.Debug("End Connect2Server");
        }
        
        private void DisConnect2Server()
        {
            _log.Debug("Beginn DisConnect2Server");
            _client.Disconnect(true);
            _log.Debug("End DisConnect2Server");
        }
        #endregion

        #region Check Mail with Rules
        public void CheckMails()
        {
            _log.Debug("Beginn CheckMails");
            Connect2Server();
            // The Inbox folder is always available on all IMAP servers...
            IMailFolder Inbox = _client.Inbox;
            Inbox.Open(FolderAccess.ReadWrite);
            // Infos Flags
            int CountMessages = 0;
            int CountActions = 0;

            foreach (UniqueId UId in Inbox.Search(SearchQuery.NotSeen))
            {
                bool RuleMatch = false;
                MimeMessage message = Inbox.GetMessage(UId);
                _log.Debug(String.Format("Mail to Check: {0} : {1}", message.Date, message.Subject));
                CountMessages++;

                foreach (MailRule rule in MailRules)
                {
                    int FilterCounter = rule.CheckRuleOnMessage(message);
                    
                    //Action Region
                    if (FilterCounter == 0)
                    {
                        _log.Info(String.Format("Mail reach ActionCounter: {0} : {1}", message.Date, message.Subject));
                        // Check MarkRead
                        if (rule.MarkRead)
                        {
                            //Mark the Message as seen
                            Inbox.SetFlags(UId, MessageFlags.Seen, true);
                            _log.Debug(String.Format("Mail mark Seen: {0} : {1}", message.Date, message.Subject));
                        }

                        // Do Action
                        switch (rule.Action)
                        {
                            case MailAction.Move:
                                _log.Info("Execute: MailAction.Move");
                                IMailFolder target = _client.GetFolder(rule.ActionParam);

                                Inbox.MoveTo(UId, target);
                                break;
                            case MailAction.Delete:
                                _log.Info("Execute: MailAction.Delete");
                                Inbox.SetFlags(UId, MessageFlags.Deleted, true);
                                Inbox.Expunge();
                                break;
                            case MailAction.MarkRead:
                                _log.Info("Execute: MailAction.MarkRead");
                                // Nothing to do
                                break;
                            default:
                                _log.Fatal("Default case Not Implementet!!");
                                throw new NotImplementedException("Action Not Implementet!");
                        }

                        RuleMatch = true;
                    }

                    // One Match > Next Mail
                    if (RuleMatch)
                    {
                        CountActions++;
                        break;
                    }
                }
            }
            DisConnect2Server();
            _log.Info(String.Format("{0} mails were checked and {1} actions actions!", CountMessages, CountActions));
            _log.Debug("End CheckMails");
        }
        #endregion
    }
}
