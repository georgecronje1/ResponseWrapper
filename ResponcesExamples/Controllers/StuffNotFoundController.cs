using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResponseWrapper.Models;

namespace ResponcesExamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StuffNotFoundController : ControllerBase
    {
        [HttpGet]
        [Route("NotFoundEmpty")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult NotFoundEmpty()
        {
            return NotFound();
        }

        [HttpGet]
        [Route("NotFoundString")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult NotFoundEmpty([Required][MinLength(2)]string message)
        {
            return NotFound(message);
        }

        [HttpGet]
        [Route("NotFoundObj")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult NotFoundObj([Required][MinLength(2)] string message)
        {
            return NotFound(new { message });
        }
    }
}
