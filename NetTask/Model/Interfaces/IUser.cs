using System.Collections.Generic;

namespace NetTask.Model.Interfaces
{
    interface IUser
    {
        string login { get; set; }
        string id { get; set; }
        string avatar_url { get; set; }
        string gravatar_id { get; set; }
        string url { get; set; }
        string html_url { get; set; }
        string followers_url { get; set; }
        string following_url { get; set; }
        string gists_url { get; set; }
        string starred_url { get; set; }
        string subscriptions_url { get; set; }
        string organizations_url { get; set; }
        string repos_url { get; set; }
        string events_url { get; set; }
        string received_events_url { get; set; }
        string type { get; set; }
        string site_admin { get; set; }
        List<Repository> repositories { get; set; }
        System.Windows.Media.Imaging.BitmapImage Avatar { get; set; }
        int repositioryCount { get; }
    }
}
