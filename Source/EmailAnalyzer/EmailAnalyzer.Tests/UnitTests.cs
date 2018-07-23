using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace EmailAnalyzer.Tests
{
    public class UnitTests
    {
        private Dictionary<string,string> emailLoadAndParse()
        {
            EmailAnalyzer analyzer = new EmailAnalyzer();
            string emailFilePath = "testemail2.txt";
            string emailAsString = new StreamReader(emailFilePath).ReadToEnd();
            Dictionary<string,string> emailParsed = analyzer.emailParse(emailAsString);
            return emailParsed;
        }

        [Fact]
        public void EmailBodyURLsGet()
        {
            Dictionary<string,string> email = this.emailLoadAndParse();
            EmailAnalyzer analyzer = new EmailAnalyzer();
            List<string> urlsFromEmailBody = analyzer.emailBodyURLsGet(email);
            string urlsAsStringActual = String.Join(",", urlsFromEmailBody);
            Console.WriteLine(urlsAsStringActual);
            string urlsAsStringExpected = "http://pandaresearch.com/Unsubscribe.jsp,http://www.pandaresearch.com/images/mailImages/mail12_img01.png,http://www.pandaresearchmails.com/mx.jsp?sendTo=mx&a=dpm&uid=9694197&t=av&offid=11729&p=xKAvcj%2BqEhr9yCbzaaLLZA%3D%3D&mt=2,http://www.pandaresearchmails.com/mx.jsp?sendTo=mx&a=dpm&uid=9694197&t=av&offid=11729&p=xKAvcj%2BqEhr9yCbzaaLLZA%3D%3D&mt=2,http://www.pandaresearch.com/privacy,http://www.pandaresearch.com/uc?a=st&amp;pt=terms,http://pandaresearch.com/Unsubscribe.jsp";
            Assert.Equal(urlsAsStringExpected, urlsAsStringActual);
        }

        [Fact]
        public void EmailSenderGet()
        {
            Dictionary<string,string> email = this.emailLoadAndParse();
            EmailAnalyzer analyzer = new EmailAnalyzer();
            string senderHostActual = analyzer.emailSenderHostGet(email);
            string senderHostExpected = "mail76.pandaresearch.com";
            Assert.Equal(senderHostExpected, senderHostActual);
        }
    }
}
