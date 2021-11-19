using System;
using System.Threading.Tasks;

namespace Client.Contexts
{
    public class SecurityContext {
        private int? userId;

        public bool IsAuthenticated => UserId.HasValue;

        public int? UserId {
            get => userId;
            set {
                if (userId != value) {
                    userId = value;
                    OnAuthenticationStatusChanged();
                }
            }
        }

        public event EventHandler AuthenticationStatusChanged;

        private void OnAuthenticationStatusChanged() {
            AuthenticationStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task SetUserDetailsAsync(int? userId) {

            bool changed = false;

            if (UserId != userId) {
                UserId = userId;

                changed = true;
            }

            if (changed) {
                OnAuthenticationStatusChanged();
            }
        }

        internal async Task ClearUserDetailsAsync() {
            await SetUserDetailsAsync(null);
        }
    }
}
