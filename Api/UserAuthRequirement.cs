using Microsoft.AspNetCore.Authorization;

namespace Api {
    public class UserAuthRequirement : IAuthorizationRequirement {
        public bool IsAuthenticated(UserDetailsProvider userDetailsProvider) {
            return !string.IsNullOrEmpty(userDetailsProvider.UserDetails?.LoginName);
        }
    }
}
