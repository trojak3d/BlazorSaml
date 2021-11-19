namespace Api
{
    public class UserDetailsProvider {
        private IAuthenticatedSessionProvider authenticatedSessionProvider;
        private UserDetails userDetails;

        public UserDetailsProvider(IAuthenticatedSessionProvider authenticatedSessionProvider) {
            this.authenticatedSessionProvider = authenticatedSessionProvider;
        }

        public string LoginName => authenticatedSessionProvider.CurrentLoginName;
        public UserDetails UserDetails => userDetails;

        public void InitializeUserDetails() {
            if (string.IsNullOrEmpty(authenticatedSessionProvider.CurrentLoginName))
            {
                userDetails = null;
                return;
            }

            userDetails =  new UserDetails
            {
                UserId = 1,
                LoginName = authenticatedSessionProvider.CurrentLoginName,
            };
        }
    }
}
