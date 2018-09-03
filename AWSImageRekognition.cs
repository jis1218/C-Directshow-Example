using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace TestForms
{
    class AWSImageRekognition : IFaceRekognition
    {
        AmazonRekognitionClient rekognitionClient;
        String collectionId = "MyCollection";

        public void initialize()
        {
            rekognitionClient = new AmazonRekognitionClient();
            throw new NotImplementedException();
        }

        public void release()
        {
            throw new NotImplementedException();
        }

        public void requestRekognition(Bitmap bitmap)
        {
            searchFacesMatch(bitmap);
            throw new NotImplementedException();
        }

        public void getRekognitionResult()
        {
            throw new NotImplementedException();
        }

        public void setOnResultListener()
        {
            throw new NotImplementedException();
        }

        private void searchFacesMatch(Bitmap bitmap)
        {
            String collectionId = "MyCollection";

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
                Console.WriteLine("cannot recognize human face");
                return;
            }

            foreach (FaceMatch face in response.FaceMatches)
            {
                Console.WriteLine("FaceId: " + face.Face.FaceId + ", Similarity: " + face.Similarity);
            }
        }

        private void detectFace(Amazon.Rekognition.Model.Image image)
        {
            DetectFacesRequest request = new DetectFacesRequest()
            {
                Image = image
            };

            try
            {
                DetectFacesResponse detectFacesResponse = rekognitionClient.DetectFaces(request);

                foreach (FaceDetail face in detectFacesResponse.FaceDetails)
                {
                    Console.WriteLine("Confidence : {0}\nAge :" + face.Confidence + ", " + face.BoundingBox.Top + ", " + face.BoundingBox.Left + ", " +
                        face.BoundingBox.Height + ", " + face.BoundingBox.Width);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
