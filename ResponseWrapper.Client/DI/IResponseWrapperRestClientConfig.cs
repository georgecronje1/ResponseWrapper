using ResponseWrapper.Core.Exceptions;
using System.Net;

namespace ResponseWrapper.Client.DI
{
    public interface IResponseWrapperRestClientConfig
    {
        /// <summary>
        /// Base URL of the REST API service to call
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Indicates whether to append Authorisation headers
        /// using the Token Provider set in <see cref="ResponseWrapperConfig"/>
        /// </summary>
        bool UseAuthHeader { get; }

        /// <summary>
        /// Indicates whether to throw the Standard Exceptions when parsing the REST
        /// response. Based on Status Code, the following exceptions will be thrown:
        /// <list type="bullet">
        /// <item>[404] -> <see cref="EntityNotFoundException"/></item>
        /// <item>[400] -> <see cref="BadRequestException"/></item>
        /// <item>[409] -> <see cref="BusinessRuleException"/></item>
        /// <item>[401] -> <see cref="UnauthorisedException"/></item>
        /// <item>[Default] -> <see cref="RestException"/></item>
        /// </list>
        /// </summary>
        bool ShouldThrowStandardExceptions { get; }
    }
}
