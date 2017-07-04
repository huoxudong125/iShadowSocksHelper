using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace iShadowSocksHelper.Utils
{
    public class ImageDownloader
    {
        private static readonly HttpClient client = new HttpClient();

        public async static Task<string> LoadImage(string url)
        {
            string fileToWriteTo = Path.GetTempFileName();

            try
            {

                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            return fileToWriteTo;
                        }

                    }

                }
                
               
            }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to load the image: {0}", ex.Message);
                }
            

            return null;
        }
    }
}
