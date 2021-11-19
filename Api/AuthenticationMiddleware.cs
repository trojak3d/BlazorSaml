using System;
using System.Threading.Tasks;
using Api.Properties;
using ComponentSpace.Saml2.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class AuthenticationMiddleware {
        private readonly RequestDelegate nextRequestDelegate;
        private readonly IAuthenticationSchemeProvider schemes;
        
        public AuthenticationMiddleware(RequestDelegate next,
            IAuthenticationSchemeProvider schemes, SSOSettings ssoSettings) {
            this.nextRequestDelegate = next;
            this.schemes = schemes;
        }

        private bool IsAuthorized(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.RequestServices
                .GetRequiredService<IAuthenticatedSessionProvider>()
                .CurrentLoginName);
        }

        public async Task Invoke(HttpContext httpContext,
            UserDetailsProvider userDetailsProvider,
            AuthenticatedSessionProvider authenticatedSessionProvider,
            SamlAuthenticationHandler samlAuthenticationHandler) {

            authenticatedSessionProvider.Initialize(httpContext);

            if (!string.IsNullOrEmpty(authenticatedSessionProvider.CurrentLoginName)) {
                userDetailsProvider.InitializeUserDetails();
            }

            if (authenticatedSessionProvider.CurrentLoginName != null && userDetailsProvider.UserDetails == null) {
                ////AuthenticationScheme samlScheme = await schemes.GetSchemeAsync("SAML");
                await samlAuthenticationHandler.SignOutAsync(new AuthenticationProperties());
            }

            try {
                await nextRequestDelegate.Invoke(httpContext);
            } catch (Exception ex) {
                throw ex;
            }

            if (httpContext.Response.StatusCode == 302)
            {
                httpContext.Response.StatusCode = 200;
            }

            // Lookup or Service middleware will cause 401 response
             if (httpContext.Response.StatusCode == 401 || !IsAuthorized(httpContext)) {
                AuthenticationScheme samlScheme = await schemes.GetSchemeAsync("SAML");

                if (samlScheme != null) {
                    await httpContext.ChallengeAsync(samlScheme.Name);


                    // Override SAML 302 as this upsets CORS
                    httpContext.Response.StatusCode = 401;
                    return;
                }
            }
        }
    }
}
