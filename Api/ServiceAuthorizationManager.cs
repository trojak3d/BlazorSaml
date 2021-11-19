using System;
using System.Collections.Generic;
using System.Reflection;

namespace Api
{

    public class ServiceAuthorizationManager {

        private readonly UserDetailsProvider userDetailsProvider;
        private readonly IAuthenticatedSessionProvider authenticationProvider;

        public ServiceAuthorizationManager(UserDetailsProvider userDetailsProvider, IAuthenticatedSessionProvider authenticationProvider) {
            this.userDetailsProvider = userDetailsProvider;
            this.authenticationProvider = authenticationProvider;
        }

        public bool CheckAccess(MethodInfo methodInfo) {

            IEnumerable<AllowAnonymousAttribute> allowAnonymous =
                methodInfo.GetCustomAttributes<AllowAnonymousAttribute>();

            if (allowAnonymous != null)
            {
                return true;
            }

            UserDetails userDetails = userDetailsProvider.UserDetails;

            string loginName = userDetailsProvider.LoginName;

            if(loginName == null) {
                return false; // Cause a 401 to be returned
            } else if (userDetails == null) {
                throw new Exception("Account Not Found");
            }

            // TODO: Should throw a fault here?
            return false;
        }
    }
}
