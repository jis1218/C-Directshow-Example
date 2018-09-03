using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForms
{
    class Utils
    {
        public static Amazon.Rekognition.Model.Image bitmapToAWSImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            byte[] data = new byte[ms.Length];
            ms.Read(data, 0, (int)ms.Length);
            Amazon.Rekognition.Model.Image image = new Amazon.Rekognition.Model.Image();
            image.Bytes = ms;
            return image;
        }
    }
}
