using System;
using System.Threading.Tasks;
using Client.Authentication;
using Client.Contexts;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Client {
    public partial class App : ComponentBase
    {

        [Inject]
        protected ComponentContext Context { get; set; }

        private bool isRestoringSession;

        public bool IsLoggedIn => Context.Security.IsAuthenticated;

        protected override async Task OnInitializedAsync()
        {
            Context.Security.AuthenticationStatusChanged += Security_AuthenticationStatusChanged;

            try
            {
                isRestoringSession = true;
                Console.WriteLine("App.razor.cs");

                LoginReply loginReply;

                try
                {
                    loginReply = await Context.HttpClient.PostAsync<LoginReply>("login/who-am-i");
                }
                catch
                {
                    loginReply = null;
                }

                if (loginReply == null)
                {
                    await Context.Security.ClearUserDetailsAsync();
                }
                else
                {
                    await Context.Security.SetUserDetailsAsync(loginReply.UserId);
                }
            }
            finally
            {
                isRestoringSession = false;
            }
        }

        private void Security_AuthenticationStatusChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
    }
}
