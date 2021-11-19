using System;
using System.Threading.Tasks;
using Client.Authentication;
using Client.Contexts;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Client.Pages {
    [Route(Routes.LoginRoute)]
    [RequireNoAuthentication]
    public partial class LoginPage : ComponentBase {

        [Inject]
        ComponentContext Context { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public string DisplayName => "Login";

        public async Task TryExternalLogin() {
            try {
                await Context.HttpClient.PostAsync("login/external-login");
            } catch (ServiceUnauthorizedException ex) {
                NavigationManager.NavigateTo(ex.ChallengeUri?.AbsoluteUri, true);
                return;
            } catch (Exception ex) {
                ////await Context.Services.LoginService.CallAsync(service => service.Logout(), false, false);
                Console.WriteLine(ex.Message);
                ////NavigationManager.NavigateTo(ex.ChallengeUri?.AbsoluteUri, true);
            }

            LoginReply reply = await Context.HttpClient.PostAsync<LoginReply>("login/who-am-i");

            if (reply == null) {
                return;
            }

            await Context.Security.SetUserDetailsAsync(reply.UserId);

            NavigationManager.NavigateTo(Routes.HomeRoute);
        }
    }
}
