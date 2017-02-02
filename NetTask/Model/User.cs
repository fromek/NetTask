using NetTask.Model.Interfaces;
using System.Collections.Generic;

namespace NetTask.Model
{
    public class User : BaseModel, IUser
    {
        public string login { get; set; }
        public string id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public string site_admin { get; set; }

        private List<Repository> _repositories;
        public List<Repository> repositories
        {
            get
            {
                return _repositories;
            }
            set
            {
                _repositories = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("repositioryCount");
            }
        }

        private System.Windows.Media.Imaging.BitmapImage _avatar;
        public System.Windows.Media.Imaging.BitmapImage Avatar
        {
            get
            {
                return _avatar;
            }
            set
            {
                _avatar = value;
                NotifyPropertyChanged();
            }
        }
        public int repositioryCount => repositories == null ? 0 : repositories.Count;
    }
}
