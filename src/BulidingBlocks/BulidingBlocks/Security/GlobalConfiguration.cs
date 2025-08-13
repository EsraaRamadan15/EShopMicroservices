using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace BuildingBlocks.Security
{
    public static class GlobalConfiguration
    {
        public static void AddCommonConfig(this IServiceCollection services)
        {
            //services.AddScoped<IReCaptchaService, ReCaptchaService>();
            //services.AddScoped<IUriHelperService, UriHelperService>();
            //services.AddScoped<IUserContextService, UserContextService>();
            //services.AddSingleton<IntegrationBase>();
            services.AddHttpContextAccessor();
        }

        public static IMvcBuilder AddApiControllersWithGlobalConfig(this IServiceCollection services)
        {
            return services.AddControllers(delegate (MvcOptions options)
            {
                //options.Filters.AddGlobalFilters();
            }).AddJsonOptions(delegate (JsonOptions options)
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        public static IMvcBuilder AddMvcControllersWithGlobalConfig(this IServiceCollection services)
        {
            return services.AddControllersWithViews(delegate
            {
            }).AddJsonOptions(delegate (JsonOptions options)
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        public static void AddDefaultCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            string[] allowedOrigins = configuration.GetSection("Security:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(delegate (CorsOptions o)
            {
                o.AddPolicy("DefaultCorsPolicy", delegate (CorsPolicyBuilder builder)
                {
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(allowedOrigins).AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        public static void AddRequestLocalization(this IServiceCollection services, IConfiguration configuration)
        {
            //CultureConfiguration.CultureConfigurationConfig = configuration;
            //string defaultCultureName = CultureInfo.GetDefaultCultureKey;
            //services.Configure(delegate (RequestLocalizationOptions options)
            //{
            //    options.DefaultRequestCulture = new RequestCulture(defaultCultureName, defaultCultureName);
            //    options.SupportedCultures = CultureInfo.GetSupportedCulturesInfo();
            //    options.SupportedUICultures = options.SupportedCultures;
            //    options.FallBackToParentCultures = true;
            //    options.FallBackToParentUICultures = true;
            //    options.RequestCultureProviders = new IRequestCultureProvider[1]
            //    {
            //    new GlobalRequestCultureProvider()
            //    };
            //});
        }

        public static void AddCustomConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<GReCaptchaSettings>(configuration.GetSection("GoogleReCAPTCHA"));
            //  services.Configure<DefaultSettings>(configuration.GetSection("DefaultSettings"));
            services.AddOptions();
        }

        //public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        //{
        //    SwaggerSettings config = configuration.GetSection("SwaggerSettings").Get<SwaggerSettings>();
        //    services.AddSwaggerGen(delegate (SwaggerGenOptions swagger)
        //    {
        //        swagger.SwaggerDoc(config?.Version ?? "v1", new OpenApiInfo
        //        {
        //            Title = (config?.ApiName ?? Assembly.GetEntryAssembly()?.GetName().Name)
        //        });
        //        swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //        {
        //            Description = "Example: 'Bearer 12345'",
        //            Name = "Authorization",
        //            In = ParameterLocation.Header,
        //            Type = SecuritySchemeType.ApiKey,
        //            Scheme = "Bearer"
        //        });
        //        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement {
        //    {
        //        new OpenApiSecurityScheme
        //        {
        //            Reference = new OpenApiReference
        //            {
        //                Type = ReferenceType.SecurityScheme,
        //                Id = "Bearer"
        //            },
        //            Scheme = "oauth2",
        //            Name = "Bearer",
        //            In = ParameterLocation.Header
        //        },
        //        new List<string>()
        //    } });
        //    });
        //    services.AddSwaggerGenNewtonsoftSupport();
        //}

        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", delegate (JwtBearerOptions options)
            {
                options.RequireHttpsMetadata = false;
                options.Authority = configuration.GetValue<string>("Identity:AuthorityUrl");
                options.Audience = configuration.GetValue<string>("Identity:ApiResourceName");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration.GetValue<string>("Identity:ApiResourceName"),
                    ValidateAudience = true
                };
            });
            services.AddAuthentication("AdminBearer").AddJwtBearer("AdminBearer", delegate (JwtBearerOptions options)
            {
                options.RequireHttpsMetadata = false;
                options.Authority = configuration.GetValue<string>("Identity:AdminAuthorityUrl");
                options.Audience = configuration.GetValue<string>("Identity:ApiResourceName");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration.GetValue<string>("Identity:ApiResourceName"),
                    ValidateAudience = true
                };
            });
        }

        public static void AddHSTS(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHsts(delegate (HstsOptions options)
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(configuration.GetValue<int>("Security:HstsMaxAgeInDays"));
            });
        }

        public static void AddRateLimiter(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            //  services.Configure<IpRateLimitOptions>(configuration.GetSection(AppSettingKeyConstants.RateLimiterLimitingConfig));
            //  services.Configure<IpRateLimitPolicies>(configuration.GetSection(AppSettingKeyConstants.RateLimiterIPCustomConfig));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public static void AddSecurityHeaders(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.Use(async delegate (HttpContext context, Func<Task> next)
            {
                AddHeaderValue(configuration, context, "X-Xss-Protection", "ResponseHeader:XssProtection");
                AddHeaderValue(configuration, context, "X-Content-Type-Options", "ResponseHeader:ContentTypeOptions");
                AddHeaderValue(configuration, context, "Content-Security-Policy", "ResponseHeader:ContentSecurityPolicy");
                AddHeaderValue(configuration, context, "X-Xss-Protection", "ResponseHeader:XssProtection");
                AddHeaderValue(configuration, context, "Referrer-Policy", "ResponseHeader:ReferrerPolicy");
                AddHeaderValue(configuration, context, "X-Frame-Options", "ResponseHeader:FrameOptions");
                AddHeaderValue(configuration, context, "X-Content-Type-options", "ResponseHeader:ContentTypeOptions");
                AddHeaderValue(configuration, context, "Cache-Control", "ResponseHeader:CacheControl");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("Server");
                await next();
            });
        }

        public static void AddSecurityCookies(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always,
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict
            });
        }

        private static void AddHeaderValue(IConfiguration configuration, HttpContext context, string headerKey, string appSettingsKey)
        {
            string value = configuration.GetValue<string>(appSettingsKey);
            if (!string.IsNullOrWhiteSpace(value))
            {
                context.Response.Headers.Add(headerKey, value);
            }
        }
    }
}
