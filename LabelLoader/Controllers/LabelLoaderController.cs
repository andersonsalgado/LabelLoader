using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabelLoader.Repository;
using Microsoft.AspNetCore.Mvc;

namespace LabelLoader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelLoaderController : ControllerBase
    {
        LabelLoaderRepository _labelLoaderRepository = new LabelLoaderRepository();

        [HttpPost]
        public async Task<IActionResult> Post(string[] multipleImages)
        {
            var resultados = string.Empty;
            foreach (var item in multipleImages)
            {
                resultados += await _labelLoaderRepository.MakeAnalysisRequest(item);
            }
            if (resultados == string.Empty)
                return new StatusCodeResult(406);
            else
                return Content(resultados);
        }
    }
}