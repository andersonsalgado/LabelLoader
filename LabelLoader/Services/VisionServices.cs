using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Migrations;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class VisionServices
    {
        private readonly IConfiguration _Configuration;
        private readonly LabelContext _labelContext;
        private readonly ComputerVisionClient _client;

        public VisionServices(IConfiguration configuration, LabelContext labelContext)
        {
            _Configuration = configuration;
            _labelContext = labelContext;

            // Create a client
            _client = Authenticate(
                                    _Configuration.GetSection("Vision").GetValue<string>("endpoint"),
                _Configuration.GetSection("Vision").GetValue<string>("subscriptionKey"));



        }

        public async Task<LabelImageAdded> ObterIngredientes(string pathFile)
        {

            List<string> ingredientes = new List<string>();
            StringBuilder concat = new StringBuilder();
            bool entrar = false;
            using (var imgStream = new FileStream(pathFile, FileMode.Open))
            {


                RecognizeTextInStreamHeaders results = await _client.RecognizeTextInStreamAsync(imgStream, TextRecognitionMode.Printed);

                string idImagem = results.OperationLocation.Split('/').Last();

                var resultText = await _client.GetTextOperationResultAsync(idImagem);

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
                        if (line.Text.IndexOf("INGREDIENTE")>= 0 || entrar)
                        {
                            entrar = true;
                            concat.Append(line.Text);
                    }
                        //texto += line.Text + "\n";
                }

                    var resultado = Regex.Replace(concat.ToString(), "[^A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ, -]", "");

                    LabelImageAdded labelImageAdded = new LabelImageAdded();
                    labelImageAdded.ItemName = pathFile;
                    labelImageAdded.Ingredients = resultado.Split(',');

                    return labelImageAdded;
            } 

            }
            
            //AnalyzeImageUrl(_client, Path.Combine(Environment.CurrentDirectory,"images", Path.GetFileName("rotulo.jpg"))).Wait();
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


       
    }

}
