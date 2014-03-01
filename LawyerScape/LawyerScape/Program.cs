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
            htmlDoc.OptionFixNestedTags = true;
            WebClient client = new WebClient();

            List<Lawyer> lawyers = new List<Lawyer>();

            bool isMore = true;
            int linksPerPage = 20;
            int loop = 0;
            do
            {

                string detailUrl = "http://www.wyomingbar.org/directory/index.html?-Token.name=&-Token.firm=&-Token.city=&-Token.county=&-Token.sort=nameLast&-Token.lawyer_search_submitted=1&-Token.skip=" + (loop * linksPerPage);
                string downloadString = client.DownloadString(detailUrl); ;
                htmlDoc.LoadHtml(downloadString);
                List<int> lawyerIDs = new List<int>();

                for (int i = 0; i < 20; i++)
                {
                    var searchLink = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table[2]/tr[" + (i + 2) + "]/td[1]/a");
                    if (searchLink != null)
                    {
                        foreach (var a in searchLink.Attributes)
                        {
                            lawyerIDs.Add(int.Parse(a.Value.Substring(a.Value.IndexOf('=') + 1)));
                        }
                    }
                    else
                    {
                        isMore = false;
                        break;
                    }
                }


                //filePath is a path to a file containing the html
                foreach (int i in lawyerIDs)
                {
                    Lawyer lawyer = ProcessIndividual(i);
                    lawyers.Add(lawyer);
                }
                loop++;
            }
            while (isMore);

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Temp\Lawyers.csv"))
            {
                foreach (Lawyer lawyer in lawyers)
                {
                    writer.WriteLine(lawyer.Name + "," +
                        lawyer.Address.Replace(',', ' ').Replace('\n', ' ') + "," +
                        lawyer.Website + "," +
                        lawyer.Email + "," +
                        lawyer.District + "," +
                        lawyer.County + "," +
                        lawyer.AdmissionDate + "," +
                        lawyer.Status + "," +
                        lawyer.Discipline);
                }
            }


            Console.ReadLine();
        }

        private static Lawyer ProcessIndividual(int i)
        {
            Lawyer lawyer = new Lawyer();

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            WebClient client = new WebClient();
            string detailUrl = "https://www.wyomingbar.org/directory/index.html?-Token.vid=" + i;
            string downloadString = client.DownloadString(detailUrl); ;
            htmlDoc.LoadHtml(downloadString);

            if (htmlDoc.DocumentNode != null)
            {
                var test = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]");
                if (!test.InnerHtml.Contains("Record is missing"))
                {
                    Console.WriteLine(i);

                    try
                    {
                        var nameNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/p[2]");
                        lawyer.Name = nameNode.InnerHtml;

                        var addressAndPhoneNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/p[3]");
                        lawyer.Address = addressAndPhoneNode.InnerHtml.Replace("<br>", " ");

                        var website = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr/td/a");
                        bool isMissingRow = false;
                        if (website != null)
                        {
                            if (string.IsNullOrEmpty(website.InnerHtml))
                            {
                                isMissingRow = true;
                            }
                            else
                            {
                                var checkForNowWebsite = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[1]/td[3]");
                                if (checkForNowWebsite != null && checkForNowWebsite.InnerHtml.Contains("Judicial District"))
                                {
                                    isMissingRow = true;
                                }
                                else
                                {
                                    lawyer.Website = website.InnerHtml;
                                }
                            }

                            int firstRowIndex = isMissingRow ? 1 : 2;
                            var email = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[" + firstRowIndex + "]/td/a");
                            if (email != null)
                            {
                                isMissingRow = true;
                                lawyer.Email = email.InnerHtml;
                            }


                            string districtLoc = "/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[" + firstRowIndex + "]/td[3]";
                            var district = htmlDoc.DocumentNode.SelectSingleNode(districtLoc);
                            lawyer.District = district.InnerHtml.Replace("Judicial District: ", "");

                            var county = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[" + (firstRowIndex + 1) + "]/td");
                            lawyer.County = county.InnerHtml.Replace("County: ", "");

                            var admissionDate = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[" + (firstRowIndex + 1) + "]/td[3]");
                            lawyer.AdmissionDate = admissionDate.InnerHtml.Replace("Admission Date: ", "");

                            var status = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[" + (firstRowIndex + 2) + "]/td");
                            lawyer.Status = status.InnerHtml.Replace("Status: ", "");

                            var discipline = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table[4]/tr/td[3]/table/tr[1]/td[2]/table/tr[" + (firstRowIndex + 3) + "]/td");
                            lawyer.Discipline = discipline.InnerHtml.Replace("Public Discipline: ", "");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("--------------------------------------------------\n");
                        Console.WriteLine("Cannot Parse:" + i);
                        Console.WriteLine("--------------------------------------------------\n");
                    }
                }
            }
            return lawyer;
        }

        private class Lawyer
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Website { get; set; }
            public string Email { get; set; }
            public string District { get; set; }
            public string County { get; set; }
            public string AdmissionDate { get; set; }
            public string Status { get; set; }
            public string Discipline { get; set; }

        }
    }
}
