using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.Server.Interceptors
{
    //Spa > API > API
    public class AuthorizationBearerProxyHttpHandler : DelegatingHandler
    {
        private readonly string bearerToken;

        public AuthorizationBearerProxyHttpHandler(IHttpContextAccessor httpContextAccessor)
        {
            bearerToken = httpContextAccessor.HttpContext?.Request
                      .Headers["Authorization"]
                      .FirstOrDefault(h => h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase));
        }

        public AuthorizationBearerProxyHttpHandler(HttpMessageHandler innerHandler, IHttpContextAccessor httpContextAccessor)
            : base(innerHandler)
        {
            bearerToken = httpContextAccessor.HttpContext?.Request
                      .Headers["Authorization"]
                      .FirstOrDefault(h => h.StartsWith("bearer ", StringComparison.InvariantCultureIgnoreCase));
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (bearerToken != null)
                request.Headers.Add("Authorization", bearerToken);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
