using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;
using Octavus.Infra.Core;
using Octavus.Authentication;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var keycloakConfig = Configuration.GetSection("Keycloak");
        var authority = keycloakConfig["auth-server-url"];
        var realm = keycloakConfig["Realm"];
        var clientId = keycloakConfig["resource"];

        services.AddCors(o => o.AddPolicy("AllowAll", builder
        =>
        {
            builder.SetIsOriginAllowed(host => true)
            .AllowAnyMethod()
            .AllowAnyHeader();

        }));

        services.AddPersistence(Configuration);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{authority}/realms/{realm}";
                options.Audience = clientId;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"{authority}/realms/{realm}",
                    ValidateAudience = true,
                    ValidateLifetime = true
                };
            });

         services.AddHttpClient<KeycloakService>();
         services.Configure<KeycloakOptions>(keycloakConfig);
        services.AddControllers();

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Octavus Api",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme \r\n\r\n" +
                "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
                "Example: \"Bearer 1asdsadasdsa\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            }
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AlunoPolicy", policy => policy.RequireRole("Aluno"));
            options.AddPolicy("ProfessorPolicy", policy => policy.RequireRole("Professor"));
            options.AddPolicy("ColaboradorPolicy", policy => policy.RequireRole("Colaborador"));
        });
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors("AllowAll");

        var swaggerServerUrl = Configuration.GetSection("Swagger:ServerUrl")?.Value;
        app.UseSwagger(setup =>
        {
            setup.PreSerializeFilters.Add((swaggerDoc, _) =>
            {
                if (!string.IsNullOrEmpty(swaggerServerUrl))
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer()
                        {
                            Url = swaggerServerUrl
                        }
                    };
            });
        });

        app.UseSwaggerUI(setup =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                setup.SwaggerEndpoint($"{swaggerServerUrl}/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
            }
            setup.RoutePrefix = string.Empty;
            setup.DocExpansion(DocExpansion.List);
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
