using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Api {
    public class UserAuthorizationHandler : AuthorizationHandler<UserAuthRequirement>,
        IAuthorizationRequirement {
        UserDetailsProvider userDetailsProvider;

        public UserAuthorizationHandler(UserDetailsProvider userDetailsProvider) {
            this.userDetailsProvider = userDetailsProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthRequirement requirement) {

            bool result = requirement.IsAuthenticated(userDetailsProvider);

            if (result) {
                context.Succeed(requirement);
            } else {
                context.Fail();
            }

            await Task.CompletedTask;
        }
    }
}
