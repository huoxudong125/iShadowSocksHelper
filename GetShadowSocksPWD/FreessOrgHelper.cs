using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iShadowSocksHelper.Domain;
using iShadowSocksHelper.Utils;
using ZXing;

namespace iShadowSocksHelper
{
    public class FreessOrgHelper
    {
        private static List<string> _imageUrls;

        static FreessOrgHelper()
        {
            _imageUrls = new List<string> {
            "http://freess.org/images/servers/jp01.png",
            "http://freess.org/images/servers/jp02.png",
            "http://freess.org/images/servers/jp03.png"};
        }


        public static  List<Config> GetIshadowsocksServers()
        {
            List<Config> serverConfigs = new List<Config>();
            foreach (var imgUrl in _imageUrls)
            {
               var imageFilePath=  ImageDownloader.LoadImage(imgUrl).Result;

                if (!string.IsNullOrEmpty(imageFilePath))
                {

                    // create a barcode reader instance
                    IBarcodeReader reader = new BarcodeReader();
                    // load a bitmap
                    var barcodeBitmap = new Bitmap(imageFilePath);
                    // detect and decode the barcode inside the bitmap
                    var result = reader.Decode(barcodeBitmap);
                    // do something with the result
                    if (result != null)
                    {
                        Debug.WriteLine(result.BarcodeFormat.ToString());
                        Debug.WriteLine(result.Text);
                    }
                }
               
            }

            return serverConfigs;


        }
    }
}
