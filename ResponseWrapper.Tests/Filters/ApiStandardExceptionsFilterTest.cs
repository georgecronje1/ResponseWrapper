using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ResponseWrapper.Core.Exceptions;
using ResponseWrapper.DI;
using ResponseWrapper.Filters;
using ResponseWrapper.Models;

namespace ResponseWrapper.Tests.Filters
{
    /// <summary>
    /// This class will test the use of <see cref="ExceptionConfig.UseStandardExceptions"/>
    /// and that it returns the correct Standard Responses and status codes
    /// </summary>
    public class ApiStandardExceptionsFilterTest
    {
        ActionContext _actionContext;
        ExceptionConfig _exceptionConfig;
        ApiExceptionFilter _filter;

        [SetUp]
        public void SetUp()
        {
            _actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            _exceptionConfig = new ExceptionConfig();
            _exceptionConfig.UseStandardExceptions();
            _filter = new ApiExceptionFilter(_exceptionConfig, new NullLogger<ApiExceptionFilter>());
        }

        [Test]
        public void ShouldReturnNotFound_WhenEntityNotFoundException()
        {

            var entityTypeName = "SomeType";
            var entityId = "SomeTypeId";
            var expectedMessage = $"{entityTypeName} with id:{entityId} was not found";
            var expectedStatusCode = (int)HttpStatusCode.NotFound;

            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new EntityNotFoundException(entityTypeName, entityId)
            };
            _filter.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Result);
            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void ShouldReturnBadRequest_WhenBadRequestException()
        {
            var expectedMessage = "A parameter is missing";
            var expectedStatusCode = (int)HttpStatusCode.BadRequest;

            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new BadRequestException(expectedMessage)
            };
            _filter.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Result);
            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void ShouldReturnConflict_WhenBusinessRuleException()
        {
            var expectedMessage = "Some nasty Business Rule that only makes sense to Business";
            var expectedStatusCode = (int)HttpStatusCode.Conflict;

            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new BusinessRuleException(expectedMessage)
            };
            _filter.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Result);
            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void ShouldReturnUnauthorised_WhenUnauthorisedException()
        {
            var expectedMessage = "You shall not pass";
            var expectedStatusCode = (int)HttpStatusCode.Unauthorized;

            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new UnauthorisedException(expectedMessage)
            };
            _filter.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Result);
            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(expectedMessage, response.Message);
        }

        [Test]
        public void ShouldReturnInternalError_WhenRestExceptionException()
        {
            var expectedMessage = "Just some rando exception";
            var expectedStatusCode = (int)HttpStatusCode.InternalServerError;

            var exceptionContext = new ExceptionContext(_actionContext, new List<IFilterMetadata>())
            {
                Exception = new RestException((HttpStatusCode)expectedStatusCode, expectedMessage)
            };
            _filter.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Result);
            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(expectedMessage, response.Message);
        }
    }
}
