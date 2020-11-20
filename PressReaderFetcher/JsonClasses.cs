using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressReaderFetcher
{
    public class CidInfo
    {
        public string cid { get; set; }
    }

    public class PubDates
    {
        public class Root
        {
            public Dictionary<int, Dictionary<int, Dictionary<int, Year>>> Years { get; set; }
        }

        public class Year
        {
            public long Dis { get; set; }
            public object P { get; set; }
            public string V { get; set; }
        }
    }

    public class PageKeys
    {
        public class Root
        {
            public List<PageKey> PageKeys { get; set; }
        }

        public class PageKey
        {
            public string Key { get; set; }
            public long PageNumber { get; set; }
        }
    }

    public class PubInfo
    {
        public class Root
        {
            public Issue Issue { get; set; }
            public List<PageSize> MagnifierPageSizes { get; set; }
            public Newspaper Newspaper { get; set; }
        }

        public class Issue
        {
            public DateTimeOffset IssueDate { get; set; }
        }

        public class PageSize
        {
            public int W { get; set; }
            public int H { get; set; }
        }

        public class Newspaper
        {
            public string Name { get; set; }
        }
    }

    public class PubStr
    {
        public class Root
        {
            public Issue Issue { get; set; }
            public Newspaper Newspaper { get; set; }

        }

        public class Newspaper
        {
            public string Name { get; set; }
        }

        public class Issue
        {
            [JsonProperty("Issue")]
            public string IssueIssue { get; set; }
        }
    }

    public class ImgTest
    {
        public string CID { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
    }
}
