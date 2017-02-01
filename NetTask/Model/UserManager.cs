using NetTask.Helpers;
using NetTask.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NetTask.Model
{
    public class UserManager
    {
        public UserManager() { }

        private Queue<CustomBackgroundWorker> workerQueue = new Queue<CustomBackgroundWorker>();
        private GitHubAPI _api;

        private GitHubAPI GitHubClient
        {
            get
            {
                if (_api == null) _api = new GitHubAPI();
                return _api;
            }
        }

        public async Task GetAllUserByPage(string url, ObservableCollection<User> userList)
        {
            while (!string.IsNullOrEmpty(url))
            {
                try
                {
                    var tokenHeader = await GitHubClient.GetAsync(url);
                    if (tokenHeader != null)
                    {
                        var us = await GitHubClient.GetFromResponse(tokenHeader, typeof(List<User>));
                        foreach (User u in us)
                        {
                            userList.Add(u);
                            u.repositories = await GetUserRepositories($"{u.repos_url}?&per_page=100");
                            GetUserAvatarInBackgroundWorker(u);
                        }

                        IEnumerable<string> linkParam;
                        tokenHeader.Headers.TryGetValues("Link", out linkParam);
                        url = GetLinkUrl(linkParam, "next");
                    }
                }
                catch (CustomException cEx)
                {
                    Debug.Write($"{cEx.CustomMessage} \t {cEx.StackTrace}");
                    throw cEx;
                }
                catch (HttpRequestException hrEx)
                {
                    Debug.Write($"{hrEx.Message} \t {hrEx.StackTrace}");
                    if (hrEx.InnerException.GetType() == typeof(WebException))
                    {
                        throw new CustomException("There are problems with the network connection. Download was canceled.", hrEx);
                    }
                    else
                    {
                        throw new CustomException(hrEx.Message, hrEx);
                    }
                }catch(TaskCanceledException tcEx)
                {
                    throw new CustomException("Task was canceled", tcEx);
                }catch(Exception ex)
                {
                    throw new CustomException(ex.Message , ex);
                }
            }
        }

        public async Task<List<Repository>> GetUserRepositories(string url)
        {
            List<Repository> rp = new List<Repository>();
            while (!string.IsNullOrEmpty(url))
            {
                var tokenHeader = await GitHubClient.GetAsync(url);
                if (tokenHeader != null)
                {
                    IEnumerable<string> linkParam;
                    tokenHeader.Headers.TryGetValues("Link", out linkParam);

                    url = GetLinkUrl(linkParam, "next");

                    rp.AddRange(await GitHubClient.GetFromResponse(tokenHeader, typeof(List<Repository>)));
                }
            }

            return rp;
        }

        private void GetUserAvatarInBackgroundWorker(User sender)
        {
            CustomBackgroundWorker.QueueWorker(
                                this.workerQueue,
                                sender,
                                (x, e) => { e.Result = GetUserAvatar(sender); },
                                (x, e) => { SetAvatar((Task<Stream>)e.Result, sender); });
        }


        private async Task<Stream> GetUserAvatar(User u)
        {
            try
            {
                if (!string.IsNullOrEmpty(u.avatar_url))
                {
                    var tokenHeader = await GitHubClient.GetAsync(u.avatar_url);
                    if (tokenHeader != null)
                    {
                        return await tokenHeader.Content.ReadAsStreamAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write($"{ex.Message} \t {ex.StackTrace}");
            }

            return null;
        }

        private void SetAvatar(Task<Stream> st, User u)
        {
            BitmapImage bitmapimage = null;
            try
            {
                var str = st.Result;
                if(str != null)
                {
                    bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = str;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                }
            }
            catch (Exception ex)
            {
                Debug.Write($"{ex.Message} \t {ex.StackTrace}");
            }

            u.Avatar = bitmapimage;
        }


        private string GetLinkUrl(IEnumerable<string> linkParam, string linkType)
        {
            string url = null;
            try
            {
                if (linkParam != null)
                {
                    var links = linkParam.FirstOrDefault().Split(new char[] { ',' }).Select(t => new { linkType = t.Split(new char[] { ';' })[1], link = t.Split(new char[] { ';' })[0] });
                    url = links.Where(l => l.linkType.Contains(linkType)).FirstOrDefault() != null ? links.Where(l => l.linkType.Contains(linkType)).Select(l1 => l1.link).FirstOrDefault() : null;
                    url = string.IsNullOrEmpty(url) ? null : url.Replace('<', ' ').Replace('>', ' ').Trim();
                }
            }
            catch (Exception ex)
            {
                //todo write to log
                Debug.Write($"An exception occurred while retrieving the address of another page of data. \t Exception detail: {ex.Message}\t{ex.StackTrace} ");
            }
            return url;
        }


    }
}
