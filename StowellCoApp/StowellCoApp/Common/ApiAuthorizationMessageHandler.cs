using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace StowellCoApp.Common
{
    public class ApiAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public ApiAuthorizationMessageHandler(
            ITokenAcquisition tokenAcquisition,
            IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
        {
            var scopes = _configuration["API:Scopes"]!.Split(' ');

            var accessToken = await _tokenAcquisition
                .GetAccessTokenForUserAsync(scopes);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
