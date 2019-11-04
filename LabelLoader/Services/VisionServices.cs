using GeekBurger.LabelLoader.Contract;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class VisionServices
    {
        private readonly IConfiguration _Configuration;

        public VisionServices(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public LabelImageAdded ObterIngredientes()
        {
            // Create a client
            ComputerVisionClient client = Authenticate(_Configuration.GetSection("Vision").GetValue<string>("endpoint"), 
                _Configuration.GetSection("Vision").GetValue<string>("subscriptionKey"));

            AnalyzeImageUrl(client, Path.Combine(Environment.CurrentDirectory,"images", Path.GetFileName("rotulo.jpg"))).Wait();
            return null;
        }

        /*
        * AUTHENTICATE
        * Creates a Computer Vision client used by each example.
        */
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                { Endpoint = endpoint };
            return client;
        }


        /* 
* ANALYZE IMAGE - URL IMAGE
* Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult content,
* brands, celebrities, landmarks, color scheme, and image types.
*/
        public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };


            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            //ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, features);

            using (var imgStream = new FileStream(imageUrl, FileMode.Open))
            {
                RecognizeTextInStreamHeaders results = await client.RecognizeTextInStreamAsync(imgStream, TextRecognitionMode.Printed);

                string idImagem = results.OperationLocation.Split('/').Last();

                var resultText = await client.GetTextOperationResultAsync(idImagem);

                Console.WriteLine("Teste");

                var lines = resultText.RecognitionResult.Lines;
                string texto = "";
                if (lines.Count == 0)
                {
                    texto = "None";
                }
                else
                {
                    foreach (Line line in lines)
                    {
                        texto += line.Text + "\n";
                    }
                }

            } 

            
            //results.

        }
    }

}
