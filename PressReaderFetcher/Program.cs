using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace PressReaderFetcher
{
    class Program
    {
        public static string type = "img";
        public static string accesstoken = string.Empty;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine();
                Console.WriteLine("====================== No arguments found! ======================");
                Console.WriteLine("Valid arguments are below, but it MUST start with your access token!");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();
                Console.WriteLine("=============================================================================");
                Console.WriteLine("-a=TOKEN - This is your auth token. you need a valid subscription.");
                Console.WriteLine("Whilst on PressReader.com press Ctrl+Shift+i & get your access token like below");
                Console.WriteLine("https://i.imgur.com/0ibQyoU.png");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();
                Console.WriteLine("=============================================================================");
                //Console.WriteLine("-i=ISSUEID - This is for the ID of a specific issue.");
                Console.WriteLine("-p=PUBLICATIONNAME - This is for the publication and all issues.");
                Console.WriteLine("Publication name can be found in the the url of the mag, for example, ");
                Console.WriteLine("https://www.pressreader.com/uk/retro-gamer/");
                Console.WriteLine("retro-gamer is the publication name");

                Console.WriteLine("-type=img - This is for the output type.");
                Console.WriteLine("Valid types are img or pdf. Default is img");
                Console.WriteLine("PDF output is made from the same images as img output");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();
                Console.WriteLine("Press return/enter to exit.");
                Console.ReadLine();
                Environment.Exit(0);
                //Console.WriteLine("Please enter your accesstoken");
                //Console.WriteLine("This can be found by opening your browser dev tools on pressreader.com and");
                //Console.WriteLine("searching for \"accesstoken\" in the html. The accesstoken will probably end in !!");
                //accesstoken = Console.ReadLine();


                //Console.WriteLine("Please enter a publication name (get it from the URL)");
                //Console.WriteLine("Example: retro-gamer or the-wall-street-journal");
                //pubname = Console.ReadLine();
            }
            else 
            {
                if (args[0].StartsWith("-a="))
                {
                    accesstoken = args[0].Split('=')[1];

                    List<string> pubnames = new List<string>();

                    foreach (var argument in args)
                    {
                        if (argument.StartsWith("-p="))
                        {
                            pubnames.Add(argument.Split('=')[1]);
                        }
                        else if (argument.StartsWith("-t="))
                        {
                            //string date = argument.Split('=')[1];
                            //List<string> dates = new List<string>();
                            //dates.Add(date);
                            //getIssues(dates);
                            type = argument.Split('=')[1];
                        }
                    }
                    Console.WriteLine("Fetching Data...");

                    foreach (var pubname in pubnames)
                    {
                        getPub(pubname);
                    }
                }
            }
        }

        static void getPub(string pubname)
        {
            List<string> pubids = new List<string>();
            using (var client = new WebClient())
            {
                string cidjson = client.DownloadString("https://ingress.pressreader.com/services/catalog/v1/routes/publication?accessToken=" + accesstoken + "&publication=" + pubname);
                CidInfo cid = JsonConvert.DeserializeObject<CidInfo>(cidjson);

                string pubjson = client.DownloadString("https://ingress.pressreader.com/services/calendar/get?accessToken=" + accesstoken + "&cid=" + cid.cid);
                PubDates.Root pubdates = JsonConvert.DeserializeObject<PubDates.Root>(pubjson);

                foreach (var year in pubdates.Years)
                {
                    int pubyear = year.Key;
                    foreach (var month in year.Value)
                    {
                        int pubmonth = month.Key;
                        foreach (var day in month.Value)
                        {
                            int pubday = day.Key;
                            pubids.Add(cid.cid + pubyear + pubmonth.ToString("00") + pubday.ToString("00") + "00000000001001");
                        }
                    }
                }
            }
            getIssues(pubids);
        }

        static void getIssues(List<string> pubids)
        {
            using (var client = new WebClient())
            {
                foreach (var pubid in pubids)
                {
                    string pubinfojson = client.DownloadString("https://ingress.pressreader.com/services/IssueInfo/GetIssueInfo?accessToken=" + accesstoken + "&issue=" + pubid);
                    PubInfo.Root pubinfo = JsonConvert.DeserializeObject<PubInfo.Root>(pubinfojson);

                    string pagekeysjson = client.DownloadString("https://ingress.pressreader.com/services/IssueInfo/GetPageKeys?accessToken=" + accesstoken + "&issue=" + pubid + "&pageNumber=0");
                    PageKeys.Root pagekeys = JsonConvert.DeserializeObject<PageKeys.Root>(pagekeysjson);

                    int height = 0;
                    int width = 0;
                    using (MemoryStream stream = new MemoryStream(client.DownloadData("https://i.prcdn.co/img?file=" + pubid + "&page=1")))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream, false, false);
                        height = image.Height;
                        width = image.Width;
                    }

                    string title = MakeValidFileName(pubinfo.Newspaper.Name);
                    Directory.CreateDirectory(title);

                    int scale = (int)Math.Floor((double)Math.Min(100 * pubinfo.MagnifierPageSizes[pubinfo.MagnifierPageSizes.Count - 1].W / width, 100 * pubinfo.MagnifierPageSizes[pubinfo.MagnifierPageSizes.Count - 1].H / height));
                    string pubdate = pubinfo.Issue.IssueDate.ToString(@"yyyy-MM-dd");

                    if (type == "pdf")
                    {
                        getPdf(pubid, title, scale, pubdate, pagekeys);
                    }
                    else if(type == "img")
                    {
                        getImg(pubid, title, scale, pubdate, pagekeys);
                    }
                }
            }
        }

        public static void getPdf(string pubid, string title, int scale, string pubdate, PageKeys.Root pagekeys)
        {
            string filename = title + " - " + pubdate + ".pdf";

            if (!File.Exists(title + "\\" + filename))
            {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(title + "\\" + filename));

                Console.WriteLine("=============================================================================");
                Console.WriteLine("Fetching: " + filename);

                foreach (var page in pagekeys.PageKeys)
                {
                    using (var client = new WebClient())
                    {
                        Console.Write("\rPage: " + (page.PageNumber));

                        byte[] data = client.DownloadData("https://i.prcdn.co/img?file=" + pubid + "&page=" + page.PageNumber + "&scale=" + scale + "&ticket=" + HttpUtility.UrlEncode(page.Key));

                        Image image = new Image(ImageDataFactory.Create(data));
                        Document doc = new Document(pdfDoc, new PageSize(image.GetImageWidth(), image.GetImageHeight()));

                        image = new Image(ImageDataFactory.Create(data));
                        pdfDoc.AddNewPage(new PageSize(image.GetImageWidth(), image.GetImageHeight()));
                        image.SetFixedPosition(page.PageNumber, 0, 0);
                        doc.Add(image);
                    }
                }
                pdfDoc.Close();
            }
            else
            {
                Console.WriteLine(title + " - " + title + " Already Exists, Skipping.");
            }
        }
        public static void getImg(string pubid, string title, int scale, string pubdate, PageKeys.Root pagekeys)
        {
            foreach (var page in pagekeys.PageKeys)
            {
                using (var client = new WebClient())
                {
                    string filename = title + " - " + pubdate + " - " + page.PageNumber.ToString("000") + ".png";

                    if (!File.Exists(title + "\\" + filename))
                    {
                        Console.WriteLine("=============================================================================");
                        Console.WriteLine("Fetching: " + filename);

                        client.DownloadFile("https://i.prcdn.co/img?file=" + pubid + "&page=" + page.PageNumber + "&scale=" + scale + "&ticket=" + HttpUtility.UrlEncode(page.Key), title + "\\" + filename);
                    }
                    else
                    {
                        Console.WriteLine(filename + " Already Exists, Skipping.");
                    }
                }
            }                
        }
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "");
        }
    }
}
