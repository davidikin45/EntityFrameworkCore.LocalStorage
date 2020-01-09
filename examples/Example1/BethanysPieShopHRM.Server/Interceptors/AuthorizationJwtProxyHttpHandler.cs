using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.Server.Interceptors
{
    //Spa > Server > API
    public class AuthorizationJwtProxyHttpHandler : DelegatingHandler
    {
        private readonly string accessToken;
        public AuthorizationJwtProxyHttpHandler(IHttpContextAccessor httpContextAccessor)
        {
            accessToken = httpContextAccessor.HttpContext.GetTokenAsync("access_token").GetAwaiter().GetResult();
        }

        public AuthorizationJwtProxyHttpHandler(HttpMessageHandler innerHandler, IHttpContextAccessor httpContextAccessor)
            : base(innerHandler)
        {
            accessToken = httpContextAccessor.HttpContext.GetTokenAsync("access_token").GetAwaiter().GetResult();
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (accessToken != null)
                request.Headers.Add("Authorization", $"Bearer {accessToken}");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
