using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using OpenCvSharp;

namespace TestForms
{
    
    

    class AWSImageRekognition : IFaceRekognition
    {
        AmazonRekognitionClient rekognitionClient;
        String collectionId = "MyCollection";


        public void initialize()
        {
            rekognitionClient = new AmazonRekognitionClient();
            
            
        }

        public void release()
        {
            
        }

        public void requestRekognition(Bitmap bitmap, MyDelegate my)
        {
            //List<Mat> matList = detectFace(bitmap);

            //if (matList != null)
            //{
            //    foreach (Mat matFace in matList)
            //    {
            //        searchFacesMatch(matFace, my);
            //    }
            //}

            searchFacesMatch(bitmap, my);
            
            
        }

        public void getRekognitionResult()
        {
           
        }

        public void setOnResultListener()
        {
            
        }

        private void searchFacesMatch(Bitmap bitmap, MyDelegate my)
        {
            Amazon.Rekognition.Model.Image image = Utils.bitmapToAWSImage(bitmap);

            SearchFacesByImageRequest request = new SearchFacesByImageRequest()
            {
                CollectionId = collectionId,
                Image = image
            };

            SearchFacesByImageResponse response = null;
            try
            {
                response = rekognitionClient.SearchFacesByImage(request);
            }
            catch (Exception e)
            {
                my(e.Message);
                //Console.WriteLine("cannot recognize human face");
                return;
            }



            if (response.FaceMatches.Count != 0)
            {
                String name = response.FaceMatches[0].Face.FaceId;
                String similarity = response.FaceMatches[0].Similarity+"";
                my(similarity);
            }
            else
            {
                my("nobody nobody but you");
            }


        }

        private List<Mat> detectFace(Bitmap bitmap)
        {
            Mat src = null;
            try
            {
                src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
            }catch(Exception e)
            {

            }
            Amazon.Rekognition.Model.Image image = Utils.bitmapToAWSImage(bitmap);

            DetectFacesRequest request = new DetectFacesRequest()
            {
                Image = image
            };

            try
            {
                DetectFacesResponse detectFacesResponse = rekognitionClient.DetectFaces(request);

                float bitmapWidth = (float) bitmap.Width;
                float bitmapHeight = (float) bitmap.Height;

                List<Mat> matList = new List<Mat>();

                foreach (FaceDetail face in detectFacesResponse.FaceDetails)
                {
                    int faceLeft = (int)(face.BoundingBox.Left * bitmapWidth);
                    int faceTop = (int) (face.BoundingBox.Top * bitmapHeight);
                    int faceWidth = (int) (face.BoundingBox.Width * bitmapWidth);
                    int faceHeight = (int) (face.BoundingBox.Height * bitmapHeight);

                    Rect rectCrop = new Rect(faceLeft, faceTop, faceWidth, faceHeight);
                    //Console.WriteLine("Confidence : {0}\nAge :" + face.Confidence + ", " + face.BoundingBox.Top + ", " + face.BoundingBox.Left + ", " +
                    //    face.BoundingBox.Height + ", " + face.BoundingBox.Width);

                    Mat img = new Mat(src, rectCrop);
                    matList.Add(img);
                }

                return matList;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

      
    }
}
