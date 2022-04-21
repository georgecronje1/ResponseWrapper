using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using ResponseWrapper.DI;
using ResponseWrapper.Filters;
using ResponseWrapper.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResponseWrapper.Tests.Filters
{
    public class ApiExceptionFilterTests
    {
        private const string defaultMessage = "Something went wrong";

        ActionContext actionContext;
        ExceptionConfig exceptionConfig;
        ApiExceptionFilter filter;

        [SetUp]
        public void SetUp()
        {
            actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            exceptionConfig = new ExceptionConfig();

            exceptionConfig.SetDefaultMessage(defaultMessage);

            exceptionConfig.AddExceptionItemWithShowMessage<FriendlyException>();

            exceptionConfig.AddExceptionItemWithMessageHandler<FriendlyWithDataException>(FriendlyWithDataException.GetFriendlyMessage);
            
            exceptionConfig.AddExceptionItem<HiddenException>();

            exceptionConfig.AddExceptionItem<NotFoundException>(statusCode: 404);

            filter = new ApiExceptionFilter(exceptionConfig, new NullLogger<ApiExceptionFilter>());
        }

        [Test]
        public void GetResult_WhenUnMappedExceptionIsThrown_ThenDefaultMessageAnd500StatusCodeShouldBeReturned()
        {
            // the List<FilterMetadata> here doesn't have much relevance in the test but is required 
            // for instantiation. So we instantiate a new instance of it with no members to ensure
            // it does not effect the test.
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new Exception()
            };


            filter.OnException(exceptionContext);


            Assert.IsNotNull(exceptionContext.Result);

            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);
            
            Assert.AreEqual(500, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(exceptionConfig.DefaultMessage, response.Message);
        }

        [Test]
        public void GetResult_WhenFriendlyExceptionIsThrown_ThenShowExceptionMessageAnd500StatusCodeShouldBeReturned()
        {
            const string friendlyMessage = "This can be shown to user";

            // the List<FilterMetadata> here doesn't have much relevance in the test but is required 
            // for instantiation. So we instantiate a new instance of it with no members to ensure
            // it does not effect the test.
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new FriendlyException(friendlyMessage)
            };

            filter.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Result);

            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(500, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(friendlyMessage, response.Message);
        }

        [Test]
        public void GetResult_WhenFriendlyExceptionWithHandlerIsThrown_ThenShowExceptionMessageFromHandlerAnd500StatusCodeShouldBeReturned()
        {
            const string friendlyMessage = "This can be shown to user";

            // the List<FilterMetadata> here doesn't have much relevance in the test but is required 
            // for instantiation. So we instantiate a new instance of it with no members to ensure
            // it does not effect the test.
            const int SomeData = 42;
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new FriendlyWithDataException(friendlyMessage, SomeData)
            };
            
            filter.OnException(exceptionContext);


            Assert.IsNotNull(exceptionContext.Result);

            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(500, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);

            Assert.IsTrue(response.Message.Contains(friendlyMessage));
            Assert.IsTrue(response.Message.Contains(SomeData.ToString()));
        }

        [Test]
        public void GetResult_WhenHiddenExceptionIsThrown_ThenShowDefaultMessageAnd500StatusCodeShouldBeReturned()
        {
            // the List<FilterMetadata> here doesn't have much relevance in the test but is required 
            // for instantiation. So we instantiate a new instance of it with no members to ensure
            // it does not effect the test.
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new HiddenException("Some Technical details - dont show to user")
            };

            filter.OnException(exceptionContext);


            Assert.IsNotNull(exceptionContext.Result);

            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(500, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(defaultMessage, response.Message);
        }

        [Test]
        public void GetResult_WhenExceptionWithStatusCodeIsThrown_ThenDefaultMessageAndMappedStatusCodeShouldBeReturned()
        {
            // the List<FilterMetadata> here doesn't have much relevance in the test but is required 
            // for instantiation. So we instantiate a new instance of it with no members to ensure
            // it does not effect the test.
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new NotFoundException("Could not find thingy with Id", 42)
            };


            filter.OnException(exceptionContext);


            Assert.IsNotNull(exceptionContext.Result);

            var result = exceptionContext.Result as JsonResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(404, result.StatusCode);

            var response = result.Value as StandardResponse<object>;
            Assert.IsNotNull(response);

            Assert.AreEqual(ResponseType.ExceptionError, response.ResponseType);
            Assert.AreEqual(exceptionConfig.DefaultMessage, defaultMessage);
        }

        public class FriendlyException : Exception
        {
            public FriendlyException(string userMessage) : base(message: userMessage)
            {

            }
        }

        public class FriendlyWithDataException : Exception
        {
            public FriendlyWithDataException(string userMessage, int someData) : base(message: userMessage)
            {
                SomeData = someData;
            }

            public int SomeData { get; }

            public static string GetFriendlyMessage(Exception ex)
            {
                var custEx = ex as FriendlyWithDataException;
                return $"{custEx.Message}: {custEx.SomeData}";
            }
        }

        public class HiddenException : Exception
        {
            public HiddenException(string userMessage) : base(message: userMessage)
            {

            }
        }

        public class NotFoundException : Exception
        {
            public NotFoundException(string userMessage, int someData) : base(message: userMessage)
            {
                SomeData = someData;
            }

            public int SomeData { get; }

            public static string GetFriendlyMessage(Exception ex)
            {
                var custEx = ex as NotFoundException;
                return $"{custEx.Message}: {custEx.SomeData}";
            }
        }
    }
}
