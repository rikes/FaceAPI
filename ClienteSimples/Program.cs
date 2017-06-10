using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;

namespace ClienteSimples
{
    class Program
    {
        

        static void Main()
        {
            string emotionKey = "53349a89ad7148cba0590fc965a39d96";
            string faceKey = "f3cc67c6452240b58d000c1854b1ff98";


            Console.Write("Enter the path to the JPEG image with faces to identify:");
            string imageFilePath = @"C:\Users\henri\Downloads\expressoes.jpg";
            
            Console.WriteLine("Caminho {0}", imageFilePath);
            Console.WriteLine("Arquivo {0}",Path.GetFileName(imageFilePath));

            MakeDetectRequest(imageFilePath, emotionKey);

            Console.WriteLine("\n\n\nWait for the result below, then hit ENTER to exit...\n\n\n");
            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        static async void MakeDetectRequest(string imageFilePath, string key)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            // Request parameters for FAce API
            //string queryString = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,smile,facialHair,glasses";

            // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            //string uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect?" + queryString;

            //Url APi Emotion
            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";


            HttpResponseMessage response;
            string responseContent;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            //A peak at the JSON response.
            Console.WriteLine(responseContent);
        }
    }
}