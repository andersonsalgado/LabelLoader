using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GeekBurger.LabelLoader.Controllers
{
    [Route("api/[controller]")]
    public class SendFilesController : ControllerBase
    {
        private readonly ILogger _logger;

        public SendFilesController(ILogger logger)
        {
            _logger = logger;
        }
        // POST api/<controller>
        [HttpPost]
        [Route("enviar")]
        public IActionResult Post([FromForm] List<IFormFile> arquivos)
        {
            try
            {
                foreach (var arquivo in arquivos)
                {
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro: {ex.Message}");
                return BadRequest();
            }
            return Ok();
        }

        
    }
}
