using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RP.Dashboard.API.Business.Clients;
using RP.Dashboard.API.Business.Contexts;
using RP.Dashboard.API.Business.Helpers;
using RP.Dashboard.API.Business.Services;
using RP.Dashboard.API.Models.Data.DB;
using RP.Dashboard.API.Validators;

namespace RP.Dashboard.API
{
	public class Startup
	{
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		private readonly string AllowDevelopmentOrigins = "_allowDevelopmentOrigins";
		private readonly string AllowProductionOrigins = "_allowProductionOrigins";

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Auth0: Add Authentication Services
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.Authority = Configuration["Auth0:Authority"];
				options.Audience = Configuration["Auth0:Audience"];
			});

			services.AddCors(options =>
			{
				options.AddPolicy(AllowDevelopmentOrigins,
				builder =>
				{
					builder.WithOrigins("http://localhost:8080", "https://sample-int-website.com")
						   .AllowAnyHeader()
						   .AllowAnyMethod();
				});
			});

			services.AddCors(options =>
			{
				options.AddPolicy(AllowProductionOrigins,
				builder =>
				{
					builder.WithOrigins("https://dashboard.relativelyproductive.com")
						   .AllowAnyHeader()
						   .AllowAnyMethod();
				});
			});

			services.AddSingleton<IConfiguration>(Configuration);
			services.AddSingleton<MemoryCache>();

			// Previously Static classes
			services.AddSingleton<CryptHelper>();
			services.AddSingleton<ConfigurationHelper>();
			services.AddSingleton<TogglHttpClient>();
			services.AddScoped<TogglHelper>();
			services.AddScoped<IValidator<Goal>, CreateGoalValidator>();
			services.AddScoped<ResponseHelper>();

			services.AddScoped<UserService>();
			services.AddScoped<GoalService>();

			// DB services
			services.AddDbContext<SqlDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("SqlDb")));

			services.AddMemoryCache();

            services.AddMvc(option => option.EnableEndpointRouting = false);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
			{
				// Add auth headers
				//c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();

				c.SwaggerDoc("v1", new OpenApiInfo { Title = "RP.Dash.Api", Version = "v1-prerelease" });
			});
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // TODO change IHostingEnvironment to IWebHostingEnvironment
        [System.Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			// Run DB Migration scripts
			using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				scope.ServiceProvider.GetService<SqlDbContext>().Database.Migrate();
			}

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "RP.Dash.Api V1");
			});

			// Auth0: Enable authentication middleware
			app.UseAuthentication();

			// TODO: This CORS stuff probably needs refactoring. Don't think this is the best way to do this.
			if (env.IsDevelopment())
			{
				app.UseCors(AllowDevelopmentOrigins);
			}
			else
			{
				app.UseCors(AllowProductionOrigins);
			}

			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}