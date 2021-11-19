using System.Reflection;
using Client.Contexts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Client.Authentication
{
    public class CustomRouteView : RouteView {
        private readonly RenderFragment renderPageDelegate;

        [Inject]
        public ComponentContext Context { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public CustomRouteView() {
            renderPageDelegate = RenderPage;
        }

        protected override void Render(RenderTreeBuilder builder) {
            var pageLayoutType = RouteData.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType
                ?? DefaultLayout;

            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), pageLayoutType);
            builder.AddAttribute(2, nameof(LayoutView.ChildContent), renderPageDelegate);
            builder.CloseComponent();
        }

        private void RenderPage(RenderTreeBuilder builder) {
            bool isLoggedIn = Context.Security.IsAuthenticated;

            var authAttribute = RouteData.PageType.GetCustomAttribute<RequireAuthenticationAttribute>();
            bool isAuthRequired = authAttribute != null;
            bool isNoAuthRequired = RouteData.PageType.GetCustomAttribute<RequireNoAuthenticationAttribute>() != null;

            if (isLoggedIn && isNoAuthRequired) {
                NavigationManager.NavigateTo(Routes.HomeRoute);
                return;
            }

            if (!isLoggedIn && !isNoAuthRequired) {
                NavigationManager.NavigateTo(Routes.LoginRoute);
                return;
            }

            builder.OpenComponent(0, RouteData.PageType);

            foreach (var kvp in RouteData.RouteValues) {
                builder.AddAttribute(1, kvp.Key, kvp.Value);
            }

            builder.CloseComponent();
        }
    }
}
