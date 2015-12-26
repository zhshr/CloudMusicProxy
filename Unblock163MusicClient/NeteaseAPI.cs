using FSLib.Network.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Unblock163MusicClient
{
    /// <summary>
    /// original author: 
    /// Get song url from song id.
    /// It works as this flow: extract song id from original page -> get dfs ID -> call another API to get URL -> replace the original result with the new URL.
    /// </summary>
    public static class NeteaseAPI
    {        
        /// <summary>
        /// Get song url from original song ID.
        /// </summary>
        /// <param name="songId">Song ID.</param>
        /// <param name="type">Bitrate. Accepts: bMusic, lMusic, mMusic, hMusic.</param>
        /// <returns>Song URL.</returns>
        public static string GetUrl(string songId, string type)
        {
            string dfsId = GetDfsId(Utility.DownloadPage(HttpMethod.Get, $"http://music.163.com/api/song/detail?id={songId}&ids=[{songId}]"), type);
            return GenerateUrl(dfsId);
        }

        /// <summary>
        /// Calculate enc ID from dfs ID.
        /// </summary>
        /// <param name="dfsId">dfs ID.</param>
        /// <returns>enc ID.</returns>
        private static string GetEncId(string dfsId)
        {
            byte[] magicBytes = new ASCIIEncoding().GetBytes("3go8&$8*3*3h0k(2)2");
            byte[] songId = new ASCIIEncoding().GetBytes(dfsId);
            for (int i = 0; i < songId.Length; i++)
            {
                songId[i] = (byte)(songId[i] ^ magicBytes[i % magicBytes.Length]);
            }
            byte[] hash = MD5.Create().ComputeHash(songId);
            return Convert.ToBase64String(hash).Replace('/', '_').Replace('+', '-');
        }

        /// <summary>
        /// Generate final URL with dfsId.
        /// </summary>
        /// <param name="dfsId"></param>
        /// <returns></returns>
        private static string GenerateUrl(string dfsId)
        {
            return $"http://p{DateTime.Now.Second % 2 + 1}.music.126.net/{GetEncId(dfsId)}/{dfsId}.mp3";
        }

        /// <summary>
        /// Extract dfs ID from the original API return value.
        /// </summary>
        /// <param name="pageContent">The original API return value.</param>
        /// <param name="type">Bitrate. Accepts: bMusic, lMusic, mMusic, hMusic.</param>
        /// <returns>dfs ID.</returns>
        private static string GetDfsId(string pageContent, string type)
        {
            JObject root = JObject.Parse(pageContent);

            // Downgrade if we don't have higher quality...

            if (type == "hMusic" && !root["songs"][0]["hMusic"].HasValues)
            {
                Console.WriteLine("Downgrade to medium quality.");
                type = "mMusic";
            }
            if (type == "mMusic" && !root["songs"][0]["mMusic"].HasValues)
            {
                Console.WriteLine("Downgrade to low quality.");
                type = "lMusic";
            }
            if (type == "lMusic" && !root["songs"][0]["lMusic"].HasValues)
            {
                Console.WriteLine("Downgrade to can't be lower quality.");
                type = "bMusic";
            }

            if (type == "bMusic" && !root["songs"][0]["bMusic"].HasValues)
            {
                // Don't ask me what to do if there's even no lowest quality...
            }

            return root["songs"][0][type]["dfsId"].Value<string>();
        }
    }
}
