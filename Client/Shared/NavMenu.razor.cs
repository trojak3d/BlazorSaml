using System;
using System.Threading.Tasks;
using Client.Authentication;
using Client.Contexts;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Client.Shared {
    public partial class NavMenu : ComponentBase {

        private bool collapseNavMenu = true;

        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        [Inject]
        ComponentContext Context { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public string DisplayName => "Login";

        public async Task LogOut() {
            await Context.HttpClient.PostAsync("login/logout");
            await Context.Security.ClearUserDetailsAsync();
            NavigationManager.NavigateTo(Routes.HomeRoute);
        }
    }
}
