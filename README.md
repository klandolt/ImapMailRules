# ImapMailRules
___

## Description

The program "ImapMailRules" can apply rules to the mail messages of the INBOX similar to various mail clients.
The following actions are available:
- MarkRead: Mark the matching mail as Read
- Move: Move the matching Mail in a subfolder of this mailbox
- Delete: Delete the matching mail permanently

All settings are configured in an configuration file.
There is an example configuration file.

The "ImapMailRules.ConsoleApp" execute this program in command line mode.

## Thirdparty

- Mailkit by Jeffrey Stedfast http://www.mimekit.net/
- log4net by The Apache Software Foundation https://logging.apache.org/log4net/
- log4net.xad Schema by Roger Knapp http://csharptest.net/downloads/schema/log4net.xsd
- NETStandard.Library by Microsoft https://dotnet.microsoft.com/

## License
The MIT License (MIT)

Copyright (c) 2021 Kevin Landolt
Detailed in the LICENSE File.

## Exitcodes

0 = all Fine

11 = No Connection to the Server after Connect & Authenticate!
12 = Could not connect to Server! Look at inner Exception:

77 = Config content Error!! Set Log Level to Debug and search the error in your ConfigFile.

99 = No ConfigFile found!!

## Questions

- For any questions and information. Ask me ;-)

