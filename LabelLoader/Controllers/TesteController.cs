using GeekBurger.LabelLoader.Migrations;
using GeekBurger.LabelLoader.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace GeekBurger.LabelLoader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly VisionServices _visionServices;

        public TesteController(IConfiguration configuration, LabelContext labelContext)
        {
            //_Configuration = configuration;
            //_visionServices = new VisionServices(configuration,labelContext);
            
        }
        public IActionResult Get() {
            var retorno = _visionServices.ObterIngredientes(Path.Combine(Environment.CurrentDirectory, "images", Path.GetFileName("rotulo.png")));
            return new ObjectResult(retorno);
        }
    }
}