using System;
using System.Threading.Tasks;
using Client.Authentication;
using Client.Contexts;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Client.Pages {
    [Route(Routes.FetchData)]
    public partial class FetchData : ComponentBase {

        [Inject]
        ComponentContext Context { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private WeatherForecast[] forecasts;

        protected override async Task OnInitializedAsync()
        {
            forecasts = await Context.HttpClient.PostAsync<WeatherForecast[]>("weather-forecast/get");
        }
    }
}
