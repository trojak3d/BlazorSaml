namespace Client.Contexts
{
    public class ComponentContext {
        public ComponentContext(SecurityContext securityContext, CustomHttpClient httpClient) {
            Security = securityContext;
            HttpClient = httpClient;
        }

        public SecurityContext Security { get; }

        public CustomHttpClient HttpClient { get; }
    }
}
