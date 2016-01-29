using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace Unblock163MusicClient
{
    public static class Processor
    {
        static String SearchURL = "http://music.163.com/eapi/batch";
        static String PlayURL = "http://music.163.com/eapi/song/enhance/player/url";
        public static void Process(object sender, SessionEventArgs e)
        {
            //Console.WriteLine("Proxied URL: " + e.RequestUrl);
            if (e.RequestUrl.Contains(SearchURL))
            {
                //modify search api
                Console.WriteLine("PC Search API: " + e.RequestUrl);
                String modified = e.GetResponseBodyAsString();
                modified = ModifySearchAPI(modified);
                e.SetResponseBodyString(modified);
            }
            if (e.RequestUrl.Contains(PlayURL))
            {
                Console.WriteLine("Play API: " + e.RequestUrl);
                e.SetResponseBodyString(ModifyPlayAPI(e.GetResponseBodyAsString()));
                foreach (HttpHeader header in e.ResponseHeaders)
                {
                    Console.WriteLine(header.ToString());
                    if (header.Name == "Content-Type")
                    {
                        header.Value = "audio/mpeg;charset=UTF-8";
                        Console.WriteLine(header.ToString());
                    }
                }
            }
            
            //if (e.RequestUrl.Contains("/eapi/v3/song/detail/") || e.RequestUrl.Contains("/eapi/v1/album/") || e.RequestUrl.Contains("/eapi/v3/playlist/detail"))
            //{
            //    string modified = ModifyDetailApi(e.GetResponseBodyAsString());
            //    e.SetResponseBodyString(modified);
            //}
            //else if (e.RequestUrl.Contains("/eapi/song/enhance/player/url"))
            //{
            //    SetPlaybackMusicQuality(e.GetResponseBodyAsString());
            //    string modified = ModifyPlayerApi(e.GetResponseBodyAsString(), _playbackBitrate, _playbackQuality);
            //    e.SetResponseBodyString(modified);
            //}
            //else if (e.RequestUrl.Contains("/eapi/song/download/limit"))
            //{
            //    string modified = ModifyDownloadLimitApi();
            //    e.SetResponseBodyString(modified);
            //}
            //else if (e.RequestUrl.Contains("/eapi/song/enhance/download/url"))
            //{
            //    string modified = e.GetResponseBodyAsString();
            //    SetDownloadMusicQuality(modified);
            //    modified = ModifyDownloadApi(modified, _downloadBitrate, _downloadQuality);
            //    e.SetResponseBodyString(modified);
            //}

        }
        static String ModifySearchAPI(String content)
        {
            JObject root = JObject.Parse(content);
            if (root["/api/cloudsearch/pc"] == null)
            {
                return content;
            }
            foreach (JObject song in root["/api/cloudsearch/pc"]["result"]["songs"])
            {
                int maxBR = 0;
                if (song["h"].HasValues)
                {
                    maxBR = song["h"]["br"].Value<int>();
                }else if (song["m"].HasValues)
                {
                    maxBR = song["m"]["br"].Value<int>();
                }
                else if (song["l"].HasValues)
                {
                    maxBR = song["l"]["br"].Value<int>();
                }
                else if (song["a"].HasValues)
                {
                    maxBR = song["a"]["br"].Value<int>();
                }
                if (song["privilege"]["fee"].Value<int>()!=0 | song["privilege"]["st"].Value<int>() != 0)
                {
                    song["privilege"]["fee"] = 0;
                    song["privilege"]["st"] = 0;
                    song["privilege"]["pl"] = maxBR;
                    song["privilege"]["dl"] = maxBR;
                    //Console.WriteLine(song["name"].Value<String>() + song["privilege"].ToString() + maxBR);
                }
            }
            return root.ToString();
        }

        public static String ModifyPlayAPI(String content)
        {
            JObject root = JObject.Parse(content);
            Console.WriteLine(root["data"][0]["url"]);
            if (root["data"][0]["url"].Value<String>() == null)
            {
                root["data"][0]["url"] = NeteaseAPI.GetUrl(root["data"][0]["id"].Value<String>(), "hMusic");
                root["data"][0]["code"] = 200;
                Console.WriteLine("\tSong ID:" + root["data"][0]["id"].Value<String>() + ", URL:" + root["data"][0]["url"].Value<String>());
            }
            return root.ToString();
        }
    }
}
