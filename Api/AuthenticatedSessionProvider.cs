using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Properties;
using ComponentSpace.Saml2.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Api
{
    public class AuthenticatedSessionProvider : IAuthenticatedSessionProvider {
        private const string MosaicUserIdHeaderName = "X-User-Id";

        public const string LoginNameClaimType = "loginName";

        private readonly SamlAuthenticationHandler samlAuthenticationHandler;
        private HttpContext httpContext;
        private SSOSettings ssoSettings;

        public AuthenticatedSessionProvider(SSOSettings ssoSettings,
            SamlAuthenticationHandler samlAuthenticationHandler) {
            this.ssoSettings = ssoSettings;
            this.samlAuthenticationHandler = samlAuthenticationHandler;
        }

        public void Initialize(HttpContext httpContext) {
            if (this.httpContext != null && this.httpContext != httpContext) {
                throw new InvalidOperationException();
            }

            this.httpContext = httpContext;
        }

        public string CurrentLoginName {
            get {
                var matchingClaims = httpContext.User.FindAll(ct => ct.Type == ssoSettings.NameIdentifierClaimType);

                if (matchingClaims != null && matchingClaims.Any()) {
                    return matchingClaims.Last().Value;
                } else {
                    return httpContext.User.FindFirstValue(LoginNameClaimType);
                }
            }
        }

        public void StartSession(string loginName) {
            Claim[] claims = new[] {
                new Claim(LoginNameClaimType, loginName)
            };

            ClaimsIdentity claimsUser =
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsUser),
                new AuthenticationProperties());
        }

        public async Task EndSessionAsync() {
            ////await httpContext.SignOutAsync(SamlAuthenticationDefaults.AuthenticationScheme);
            await httpContext.SignOutAsync(SamlAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties()
                {
                    RedirectUri = null,
                });
        }
    }
}
