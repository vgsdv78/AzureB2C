using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();
//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(option => {
//        builder.Configuration.Bind("AzureB2C", option);

//        option.Events.OnSignedOutCallbackRedirect = context =>
//        {
//            context.Response.Redirect("/");
//            context.HandleResponse();
//            return Task.CompletedTask;
//        };
//    });

string? AzureADB2CHostName = builder.Configuration.GetValue<string>("AzureB2C:Instance");
string? Tenant = builder.Configuration.GetValue<string>("AzureB2C:Domain");
string? ClientID = builder.Configuration.GetValue<string>("AzureB2C:ClientId"); ;
string? ClientSecret = builder.Configuration.GetValue<string>("AzureB2C:ClientSecret"); ;
string? PolicySignUpSignIn = builder.Configuration.GetValue<string>("AzureB2C:SignUpSignInPolicyId"); ;
string AuthorityBase = $"{AzureADB2CHostName}/{Tenant}";
string AuthoritySignInSignUp = $"{AuthorityBase}/{PolicySignUpSignIn}/v2.0";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => { 
        options.SignInScheme=CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = AuthoritySignInSignUp;
        options.ClientId = ClientID;
        options.ClientSecret = ClientSecret;
        options.ResponseType = "code";
        options.Scope.Add(ClientID);
        options.SaveTokens = true;
        //options.TokenValidationParameters = new TokenValidationParameters()
        //{
        //    NameClaimType = ""
        //};
    })

;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
