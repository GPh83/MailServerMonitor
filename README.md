![release](https://img.shields.io/github/v/release/GPh83/MailServerMonitor?include_prereleases)
![commit](https://img.shields.io/github/last-commit/GPh83/MailServerMonitor) 
![issues](https://img.shields.io/github/issues/GPh83/MailServerMonitor) 
![downloads](https://img.shields.io/github/downloads/GPh83/MailServerMonitor/total)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
![platform](https://img.shields.io/badge/platform-Windows%20Linux-blue)

# MailServerMonitor

Simple mail server monitoring with historic in CSV format.  
The goal of this tool is to ask a SMTP and IMAP server and store result in a CSV file for later analysis.  
You can launch it under Windows with schedule task, one time or endless.

After you can use LibreOffice Calc or Excel to analyze the data. (Sort/Filter/Graph ...)

## Features
- Ask SMTP and IMAP server
- Store result in CSV file 
- Check connection, authentication and response time
- Multi mailboxes
- Run every X minutes or single time

# Usage

Download release and extract package.  
Then run from command line.  
First time, fill in a mailbox credential and server.  
Next time run it for one time or endless.

For informations : 
> MailServerMonitor.exe -h

For adding a new mailbox : 
> MailServerMonitor.exe -a

For listing mailboxes stored setup : 
> MailServerMonitor.exe -l

For running only one time : 
> MailServerMonitor.exe -u

For delete mailbox (Use -l for obtain Num) : 
> MailServerMonitor.exe -d [Num]

For running endless : 
> MailServerMonitor.exe

**Other settings in config.json :** You can edit with your favorite editor (vi or VSCode for me)
- AskMin : Ask every X minutes. Default=0=One time
- CSVName : Output CSV name. Default="MailServerMonitor.csv"
- Key : Random key for securing password. Change it loose all stored password.
- For each mailbox :
  - Name : Name of the mailbox 
  - For each SMTP & IMAP :
    - eMail : email to test 
    - TimeOutMs : Time out in milliseconde. Default=30000 
    - Server : Server name (FQDN or IP address)
    - Login : Mailbox Username   
    - EncryptedPassword : Use -a command for setting this
    - Port : Imap port. Default=993
    - UseSSL : For using SSL/TLS. Default=true
 

# Contributing

**MailServerMonitor** is written in C# with .NET 6  
Using MailKit : https://dotnetfoundation.org/projects/mailkit  
Made with VS2022 Community.


# Licence 

MailServerMonitor Copyright(C) 2022 Philippe GRAILLE.  
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, **software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND**, either express or implied.
See the License for the specific language governing permissions and limitations under the License.

