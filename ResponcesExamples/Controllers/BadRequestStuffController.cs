using Microsoft.AspNetCore.Mvc;
using ResponseWrapper.Models;
using System.ComponentModel.DataAnnotations;

namespace ResponcesExamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadRequestStuffController : ControllerBase
    {
        [HttpGet]
        [Route("BadRequestEmpty")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult BadRequestEmpty()
        {
            return BadRequest();
        }

        [HttpGet]
        [Route("BadRequestString")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult BadRequestEmpty([Required][MinLength(2)] string message)
        {
            return BadRequest(message);
        }

        [HttpGet]
        [Route("BadRequestObj")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult BadRequestObj([Required][MinLength(2)] string message)
        {
            return BadRequest(new { message });
        }
    }
}
