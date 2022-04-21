using Microsoft.AspNetCore.Mvc;
using ResponcesExamples.Exceptions;
using ResponseWrapper.Models;
using System.ComponentModel.DataAnnotations;

namespace ResponcesExamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        [HttpGet]
        [Route("FriendlyException")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult FriendlyException()
        {
            throw new FriendlyException("The end user can see this");
        }

        [HttpGet]
        [Route("FriendlyExceptionWithData")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult FriendlyExceptionWithData([Required][MinLength(2)] string message)
        {
            throw new FriendlyExceptionWithData(message);
        }

        [HttpGet]
        [Route("NotFoundStatusCode")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult NotFoundStatusCode([Required][MinLength(2)] string message)
        {
            throw new SpecialStuffNotFoundException(message);
        }

        [HttpGet]
        [Route("HiddenException")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult HiddenException()
        {
            throw new HiddenSystemException("42");
        }

        [HttpGet]
        [Route("UnMappedException")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse))]
        public IActionResult UnMappedException()
        {
            throw new System.Exception("Random Stuff");
        }
    }
}
