using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Migrations;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class VisionServices
    {
        private readonly IConfiguration _Configuration;
        private readonly LabelContext _labelContext;
        private readonly ILogger<VisionServices> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ComputerVisionClient _client;
        private readonly StringBuilder _logs;

        public VisionServices(
                                IConfiguration configuration, 
                                ILogger<VisionServices> logger, 
                                LabelContext labelContext)
        {
            _logs = new StringBuilder();

            _Configuration = configuration;
            _labelContext = labelContext;
            _logger = logger;

            _logs.AppendLine("Autenticando o serviço Vision no azure");

            _client = Authenticate(
                                    _Configuration.GetSection("Vision").GetValue<string>("endpoint"),
                                    _Configuration.GetSection("Vision").GetValue<string>("subscriptionKey"));

            

        }

        public async Task<LabelImageAdded> ObterIngredientes(string pathFile)
        {
            _logs.AppendLine("Iniciando método de ObterIngredientes");

            try
            {
                List<string> ingredientes = new List<string>();
                StringBuilder concat = new StringBuilder();
                bool entrar = false;

                _logs.AppendLine($"Lendo o arquivo: {pathFile} ");
                using (var imgStream = new FileStream(pathFile, FileMode.Open))
                {
                    RecognizeTextInStreamHeaders results = await _client.RecognizeTextInStreamAsync(imgStream, TextRecognitionMode.Printed);
                    Thread.Sleep(2000);
                    string idImagem = results.OperationLocation.Split('/').Last();

                    var resultText = await _client.GetTextOperationResultAsync(idImagem);

                    var lines = resultText.RecognitionResult.Lines;

                    _logs.AppendLine($"Número de linhas encontradas: {lines.Count} ");

                    if (lines.Count > 0)
                    {
                        foreach (Line line in lines)
                        {
                            if (line.Text.IndexOf("INGREDIENTE") >= 0 || entrar)
                            {
                                entrar = true;
                                concat.Append(line.Text);
                            }
                        }

                        if (concat.ToString().Length > 0)
                        {
                            var resultado = Regex.Replace(concat.ToString(), "[^A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ, -]", "");
                            resultado = resultado.Replace("INGREDIENTES", "");
                            resultado = resultado.Replace("INGREDIENTE", "");

                            _logs.AppendLine($"Retorno dos ingredientes: {resultado}");

                            LabelImageAdded labelImageAdded = new LabelImageAdded();
                            labelImageAdded.ItemName = pathFile;
                            labelImageAdded.Ingredients = resultado.Split(',');

                            _logs.AppendLine($"LabelImageAdded serializado: {JsonConvert.SerializeObject(labelImageAdded)}");
                            _logger.LogInformation(_logs.ToString());
                            return labelImageAdded;
                        } else
                        {
                            _logs.AppendLine("Não foi encontrado ingredientes");
                        }
                    } else
                    {
                        _logs.AppendLine("Não foi encontrado ingredientes");
                    }
                }
            }
            catch (Exception ex)
            {
                _logs.AppendLine($"Ocorreu um erro: {ex.Message}");
                _logger.LogError(ex,_logs.ToString());
            }

            _logger.LogInformation(_logs.ToString());
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
