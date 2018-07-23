using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace EmailAnalyzer
{
    public class EmailAnalyzer
    {
        public Dictionary<string,string> emailParse(string emailAsString)
        {
            Dictionary<string,string> emailAsLookup = new Dictionary<string,string>();

            const string newline = "\r\n";
            const string blankLine = newline + newline;
            int emailBodyOffset = emailAsString.IndexOf(blankLine);
            string emailHeadersAsString = emailAsString.Substring(0, emailBodyOffset);
            string[] emailHeadersAsLines = emailHeadersAsString.Split(newline);
            string headerNameCurrent = null;
            string headerValueCurrent = null;

            foreach (string emailHeaderLine in emailHeadersAsLines)
            {
                const string colonSpace = ": ";
                int positionOfColonSpace = emailHeaderLine.IndexOf(colonSpace);
                if (positionOfColonSpace >= 0)
                {
                    if (headerNameCurrent != null)
                    {
                        if (emailAsLookup.ContainsKey(headerNameCurrent) == true)
                        {
                            string headerValuePrev = emailAsLookup[headerNameCurrent];
                            headerValueCurrent = headerValuePrev + "\n" + headerValueCurrent;
                            emailAsLookup[headerNameCurrent] = headerValueCurrent;
                        }
                        else
                        {
                            emailAsLookup.Add(headerNameCurrent, headerValueCurrent);
                        }
                    }
                    headerNameCurrent = emailHeaderLine.Substring(0, positionOfColonSpace);
                    headerValueCurrent = emailHeaderLine.Substring(positionOfColonSpace + colonSpace.Length);
                }
                else
                {
                    headerValueCurrent += emailHeaderLine;
                }
            }

            string emailBody = emailAsString.Substring(emailBodyOffset + blankLine.Length);
            emailAsLookup.Add("Body", emailBody);

            return emailAsLookup;
        }

        public string emailSenderHostGet(Dictionary<string,string> emailAsLookup)
        {
            string senderHostName = null;

            const string headerNameReceived = "Received";
            if (emailAsLookup.ContainsKey(headerNameReceived) == false)
            {
                string errorMessage = "Email has no 'Received' header.";
                throw new Exception(errorMessage);
            }
            else
            {
                string headerReceived = emailAsLookup[headerNameReceived];
                
                string[] headerReceivedAsTokens = headerReceived.Split(" ");
                string senderHostIPAddressBracketed = headerReceivedAsTokens[1];
                if 
                (
                    senderHostIPAddressBracketed.StartsWith("[")
                    && senderHostIPAddressBracketed.EndsWith("]")
                )
                {
                    string senderHostIPAddressAsString = 
                        senderHostIPAddressBracketed.Substring(1, senderHostIPAddressBracketed.Length - 2);
                    try 
                    {
                        IPAddress senderHostIPAddress = IPAddress.Parse(senderHostIPAddressAsString);
                        IPHostEntry senderHostHostEntry = Dns.GetHostEntry(senderHostIPAddress);
                        senderHostName = senderHostHostEntry.HostName;
                    }
                    catch (FormatException)
                    {
                        string errorMessage = 
                            "The IP from the 'Received' header did not have the expected format: " 
                            + senderHostIPAddressAsString;
                        throw new Exception(errorMessage);
                    }
                }
                else
                {
                    string errorMessage = 
                        "IP from 'Received' header did not have the expected format: " 
                        + senderHostIPAddressBracketed;
                    throw new Exception(errorMessage);
                }
            }

            return senderHostName;
        }

        public List<string> emailBodyURLsGet(Dictionary<string,string> emailAsLookup)
        {
            List<string> urlsAsStrings = new List<string>();

            string emailBody = emailAsLookup["Body"];
            // This regular expression is adapated from one found at the URL 
            // https://stackoverflow.com/questions/5717312/regular-expression-for-url
            Regex regex = new Regex("http(s)?://([\\w-]+.)+[\\w-]+(/[\\w- ./?%&=])?");
            MatchCollection matches = regex.Matches(emailBody);
            foreach (Match match in matches)
            {
                string matchAsString = match.ToString();
                urlsAsStrings.Add(matchAsString);
            }

            return urlsAsStrings;
        }
    }
}