using Api.Properties;
using Api.Utility;
using IdentityProviderShared;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            SSOSettings ssoSettings = services.AddSettings<SSOSettings>(Configuration);


            services.Configure<CookiePolicyOptions>(options => {
                // SameSiteMode.None is required to support SAML SSO.
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;

                // Some older browsers don't support SameSiteMode.None.
                options.OnAppendCookie = cookieContext => SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => SameSite.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            services.AddLogging();

            services.AddScoped<UserDetailsProvider, UserDetailsProvider>();
            services.AddScoped<ServiceAuthorizationManager>();
            services.AddScoped<AuthenticatedSessionProvider>();
            services.AddScoped<IAuthenticatedSessionProvider>(s => s.GetService<AuthenticatedSessionProvider>());

            services.AddSaml(Configuration.GetSection("SAML"));

            services
                .AddAuthentication(IdentityConstants.ExternalScheme)
                .AddSaml(options => {
                    options.PartnerName = (httpContext) => ssoSettings.PartnerName;
                    options.LoginCompletionUrl = (httpContext, relayState, a) => ssoSettings.LoginCompletionUrl;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                    options.Cookie.MaxAge = TimeSpan.FromDays(7);
                })
                // SAML logins get persisted as "Identity.External" by default
                .AddCookie(IdentityConstants.ExternalScheme, options => {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                    options.Cookie.MaxAge = TimeSpan.FromDays(7);
                });

            services.AddControllers();

            services.AddAuthorization(options => {
                options.AddPolicy("User",
                    policy => policy.AddRequirements(new UserAuthRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseRouting();

            // TODO: This has been built up using a scattergun approach and I have no idea what is required and what isn't.
            app.UseCors(builder => builder
                .WithOrigins("http://localhost:60004", "https://localhost:44304", "http://localhost:56681", "https://localhost:44378")
                .AllowCredentials()
                .WithMethods("POST", "GET")
                .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, HeaderNames.Cookie, HeaderNames.Location)
                .WithExposedHeaders(HeaderNames.Location)); // Client needs to read Location header to perform redirect for 401

            app.UseAuthentication();

            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.Map("/LogoutHandler.ashx", endpoints
                    .CreateApplicationBuilder()
                    .UseMiddleware<LogoutMiddleware>()
                    .Build());

                endpoints.MapControllers();
            });
        }
    }
}
