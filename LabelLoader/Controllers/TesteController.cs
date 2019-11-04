using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeekBurger.LabelLoader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        private readonly IConfiguration _Configuration;
        private readonly VisionServices _visionServices;

        public TesteController(IConfiguration configuration)
        {
            _Configuration = configuration;
            _visionServices = new VisionServices(configuration);
            
        }
        public IActionResult Get() {
            _visionServices.ObterIngredientes();
            return Ok();
        }
    }
}