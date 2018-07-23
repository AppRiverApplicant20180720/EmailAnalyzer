using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace EmailAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Wrong number of arguments.");
                usageMessageShow();
            }
            else
            {
                string emailFilePath = args[0];
                if (File.Exists(emailFilePath) == false)
                {
                    Console.WriteLine("File not found: " + emailFilePath);
                    usageMessageShow();
                }
                else
                {
                    StreamReader streamReader = new StreamReader(emailFilePath);
                    string emailAsString = streamReader.ReadToEnd();

                    EmailAnalyzer emailAnalyzer = new EmailAnalyzer();
                    Dictionary<string,string> emailAsLookup = emailAnalyzer.emailParse(emailAsString);

                    try
                    {
                        string senderHostName = emailAnalyzer.emailSenderHostGet(emailAsLookup);
                        Console.WriteLine("Sender host name: " + senderHostName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    List<string> urlsInEmailBody = emailAnalyzer.emailBodyURLsGet(emailAsLookup);
                    Console.WriteLine("URLs in email body:");
                    foreach (string urlAsString in urlsInEmailBody)
                    {
                        Console.WriteLine("    " + urlAsString);
                    }
                }
            }
        }

        static void usageMessageShow()
        {
            Console.WriteLine("Usage: EmailAnalyzer <emailFilePath>");
        }

    } // end class

} // end namespace
