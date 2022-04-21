using AdfsAuthenticationHandler.Services.Funds;
using Microsoft.AspNetCore.Mvc;
using ResponcesExamples.Models;
using ResponcesExamples.Services;
using ResponseWrapper.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ResponcesExamples.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IFakeRestClientService _fakeRestClientService;
        private readonly IFundsAuthorizationService _fundsAuthorizationService;

        public DataController(IFakeRestClientService fakeRestClientService, IFundsAuthorizationService fundsAuthorizationService)
        {
            _fakeRestClientService = fakeRestClientService;
            _fundsAuthorizationService = fundsAuthorizationService;
        }

        [HttpPost]
        [Route("CheckUserRoleAccess")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse<DataResponse>))]
        public async Task<IActionResult> CheckUserRoleAccessAsync([Required] DataRequest request)
        {
            var hasAccess = await _fundsAuthorizationService.DoesUserHaveAccessToPoliciesAsync("1002000259000727");

            var response = new DataResponse { DataItem1 = request.DataItem1, DataItem2 = request.DataItem2 };

            return Ok(response);
        }


        [HttpPost]
        [Route("SendData")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse<DataResponse>))]
        public IActionResult FriendlyExceptionWithData([Required] DataRequest request)
        {
            var response = new DataResponse { DataItem1 = request.DataItem1, DataItem2 = request.DataItem2 };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetSomeData")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse<DataResponse>))]
        public IActionResult GetSomeData()
        {


            var response = new DataResponse { DataItem1 = "Some", DataItem2 = "Data" };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetSomeDataAsClient")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse<DataResponse>))]
        public IActionResult GetSomeDataAsClient()
        {
            var response = _fakeRestClientService.GetMessage();

            return Ok(response);
        }

        [HttpGet]
        [Route("GetRestrictedData")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse<DataResponse>))]
        public IActionResult GetRestrictedData()
        {
            var response = new DataResponse { DataItem1 = "Some", DataItem2 = "Data" };

            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateRestrictedData")]
        [ProducesResponseType(statusCode: 200, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 404, type: typeof(StandardResponse<DataResponse>))]
        [ProducesResponseType(statusCode: 500, type: typeof(StandardResponse<DataResponse>))]
        public IActionResult UpdateRestrictedData([Required] DataRequest request)
        {
            var response = new DataResponse { DataItem1 = request.DataItem1, DataItem2 = request.DataItem2 };

            return Ok(response);
        }
    }
}
