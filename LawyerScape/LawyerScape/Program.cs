using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LawyerScape
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            WebClient client = new WebClient();
            string downloadString = client.DownloadString("https://www.wyomingbar.org/directory/index.html?-Token.vid=3030");
            htmlDoc.LoadHtml(downloadString);

            // Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                foreach (var error in htmlDoc.ParseErrors)
                {
                    Console.WriteLine(error.Reason);

                }
            }


            if (htmlDoc.DocumentNode != null)
            {
                var addressAndPhoneNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/p[2]");
                Console.WriteLine(addressAndPhoneNode.InnerHtml);

                var nameNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/p[3]");
                Console.WriteLine(nameNode.InnerHtml);

                var website = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr/td/a");
                Console.WriteLine(website.InnerHtml);


                var email = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[2]/td/a");
                Console.WriteLine(email.InnerHtml);

                var district = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[2]/td[3]");
                Console.WriteLine(district.InnerHtml);

                var county = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[3]/td");
                Console.WriteLine(county.InnerHtml);

                var admissionDate = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[3]/td[3]");
                Console.WriteLine(admissionDate.InnerHtml);

                var status = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[4]/td");
                Console.WriteLine(status.InnerHtml);

                var discipline = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[5]/td");
                Console.WriteLine(discipline.InnerHtml);

                Console.ReadLine();
            }
        }   
    }
}
