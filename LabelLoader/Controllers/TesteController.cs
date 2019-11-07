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

        public TesteController(IConfiguration configuration, LabelContext labelContext)
        {
            //_Configuration = configuration;
            //_visionServices = new VisionServices(configuration,labelContext);
            
        }
        public IActionResult Get() {
            return null;
        }
    }
}