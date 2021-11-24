using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class LogoutMiddleware
    {

        public LogoutMiddleware(RequestDelegate next) {
        }

        private bool IsAuthorized(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.RequestServices
                .GetRequiredService<IAuthenticatedSessionProvider>()
                .CurrentLoginName);
        }

        public async Task Invoke(HttpContext httpContext) {

            if (!IsAuthorized(httpContext)) {
                return;
            }
            else
            {
                await httpContext.RequestServices
                    .GetRequiredService<IAuthenticatedSessionProvider>().EndSessionAsync();
            }
        }
    }
}
