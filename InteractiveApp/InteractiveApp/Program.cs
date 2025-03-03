using InteractiveApp.Client.Interfaces;
using InteractiveApp.Client.Services;
using InteractiveApp.Components.Account;
using InteractiveApp.Data;
using InteractiveApp.Endpoints;
using InteractiveApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
//builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, ModAuthenticationStateProvider>();
builder.Services.AddScoped<ModAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ModAuthenticationStateProvider>());
builder.Services.AddScoped<IClaimPrincipailSync>(provider => provider.GetRequiredService<ModAuthenticationStateProvider>());
builder.Services.AddScoped<IUserRequirementsService, UserRequirementsService>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<AppState>();

builder.Services.AddTransient<IIdentityUserService, IdentityUserService>();
//builder.Services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//	options.Events = new CookieAuthenticationEvents
//	{
//		OnValidatePrincipal = async context =>
//		{
//			var userPrincipal = context.Principal;
//			if (userPrincipal.Identity.IsAuthenticated)
//			{
//				var identity = (ClaimsIdentity)userPrincipal.Identity;

//				// Dodaj w³asne claims
//				identity.AddClaim(new Claim("Role", "Admin"));

//				// Aktualizuj Principal
//				var newPrincipal = new ClaimsPrincipal(identity);
//				context.ReplacePrincipal(newPrincipal);
//				context.ShouldRenew = true; // Odœwie¿enie ciasteczka
//			}

//			await Task.CompletedTask;
//		}
//	};
//});

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "InteractiveApp API", Version = "v1" });

	// Dodaj konfiguracjê dla uwierzytelniania za pomoc¹ ciasteczek
	c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.ApiKey,
		In = ParameterLocation.Cookie,
		Name = ".AspNetCore.Identity.Application",
		Scheme = "cookieAuth",
		Description = "U¿yj ciasteczka uwierzytelniaj¹cego, aby uzyskaæ dostêp do autoryzowanych endpointów"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "cookieAuth"
				}
			},
			new string[] {}
		}
	});
});

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Events = new CookieAuthenticationEvents
	{
		OnRedirectToLogin = context =>
		{
			if (context.Request.Path.StartsWithSegments("/api"))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			}
			else
			{
				context.Response.Redirect("/error401");
			}
			return Task.CompletedTask;
		},
		OnRedirectToAccessDenied = context =>
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return Task.CompletedTask;
		}
	};
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseMigrationsEndPoint();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "InteractiveApp API v1");
	});
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<InteractiveApp.Components.App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(InteractiveApp.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapIdentityEndpoints();

app.Run();
