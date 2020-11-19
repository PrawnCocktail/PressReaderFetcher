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
            public bool EnableHighlihtPaidIssues { get; set; }
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
            public bool ConfirmationRequired { get; set; }
            public string Key { get; set; }
            public long PageNumber { get; set; }
        }
    }

    public class PubInfo
    {
        public class Root
        {
            public AggregatedContent AggregatedContent { get; set; }
            public bool AggregateTocByPage { get; set; }
            public object Bookmark { get; set; }
            public Category Category { get; set; }
            public bool CouldPotentiallyBeRead { get; set; }
            public object Sponsor { get; set; }
            public bool EnableCalendarCache { get; set; }
            public List<long> Heights { get; set; }
            public bool IsAllPagesAvailable { get; set; }
            public bool IsMyNewspaper { get; set; }
            public Issue Issue { get; set; }
            public string LastIssue { get; set; }
            public Layout Layout { get; set; }
            public List<PageSize> MagnifierPageSizes { get; set; }
            public long MaxBackIssues { get; set; }
            public PageSize MaxUnrestrictedFirstPageSize { get; set; }
            public PageSize MaxUnrestrictedPageSize { get; set; }
            public Newspaper Newspaper { get; set; }
            public long Pages { get; set; }
            public List<PagesInfo> PagesInfo { get; set; }
            public List<PageSize> PageSizes { get; set; }
            public SubscriptionsInfo SubscriptionsInfo { get; set; }
            public long Zooms { get; set; }
            public Slugs Slugs { get; set; }
        }

        public class AggregatedContent
        {
            public bool AggregateCalendar { get; set; }
            public object AggregateCid { get; set; }
            public object AggregateIssue { get; set; }
            public object AggregateLastIssue { get; set; }
            public bool AggregateToc { get; set; }
            public long TocKey { get; set; }
        }

        public class Category
        {
            public string CategoryId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class Issue
        {
            public string Cid { get; set; }
            public object ExpungeVersion { get; set; }
            public long ImagesEngineVersion { get; set; }
            public string IssueIssue { get; set; }
            public string IssueDateDisplayName { get; set; }
            public DateTimeOffset IssueDate { get; set; }
            public long ProfileId { get; set; }
        }

        public class Layout
        {
            public bool LayoutAvailable { get; set; }
            public long LayoutVersion { get; set; }
            public bool PartialLayoutAvailable { get; set; }
            public bool ValidForSmartFlow { get; set; }
        }

        public class PageSize
        {
            public int W { get; set; }
            public int H { get; set; }
        }

        public class Newspaper
        {
            public string Culture { get; set; }
            public bool EnableSmart { get; set; }
            public object ImageBackgroundColor { get; set; }
            public string IsoLanguage { get; set; }
            public bool IsRightToLeft { get; set; }
            public string Name { get; set; }
            public Mastheads Mastheads { get; set; }
        }

        public class Mastheads
        {
            public string ColorImageId { get; set; }
            public string WhiteImageId { get; set; }
            public long Width { get; set; }
            public long Height { get; set; }
        }

        public class PagesInfo
        {
            public object ExpungeVersion { get; set; }
            public long MaxUnrestrictedScale { get; set; }
            public long PageNumber { get; set; }
        }

        public class Slugs
        {
            public string Country { get; set; }
            public string Newspaper { get; set; }
        }

        public class SubscriptionsInfo
        {
            public string BasicPrice { get; set; }
            public bool IsFreeContent { get; set; }
            public bool RequiredAuthenticatedUser { get; set; }
            public string SingleIssuePrice { get; set; }
        }
    }
}
