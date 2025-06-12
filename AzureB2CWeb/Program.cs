using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(option =>
    {
        builder.Configuration.Bind("AzureB2C", option);
        option.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async opt =>
                    {

                        string? role = opt.Principal.FindFirstValue("extension_Role");
                        if (!string.IsNullOrEmpty(role))
                        {
                            var claims = new List<Claim>
                                    {
                            new Claim(ClaimTypes.Role, role)
                                    };

                            var appIdentity = new ClaimsIdentity(claims);
                            opt.Principal.AddIdentity(appIdentity);
                        }
                    },
            OnSignedOutCallbackRedirect = context =>
                {
                    context.Response.Redirect("/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
        };
    })
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "https://dotnetmasterycoding.onmicrosoft.com/sampleapi/fullaccess" })
    .AddInMemoryTokenCaches();

//string? AzureADB2CHostName = builder.Configuration.GetValue<string>("AzureB2C:Instance");
//string? Tenant = builder.Configuration.GetValue<string>("AzureB2C:Domain");
//string? ClientID = builder.Configuration.GetValue<string>("AzureB2C:ClientId"); ;
//string? ClientSecret = builder.Configuration.GetValue<string>("AzureB2C:ClientSecret"); ;
//string? PolicySignUpSignIn = builder.Configuration.GetValue<string>("AzureB2C:SignUpSignInPolicyId"); ;
//string? EditProfilePolicy = builder.Configuration.GetValue<string>("AzureB2C:EditProfilePolicyId"); ;
//string AuthorityBase = $"{AzureADB2CHostName}/{Tenant}";
//string AuthoritySignInSignUp = $"{AuthorityBase}/{PolicySignUpSignIn}/v2.0";
//string AuthorityEdit = $"{AuthorityBase}/{EditProfilePolicy}/v2.0";
//string Scope = "https://dotnetmasterycoding.onmicrosoft.com/sampleapi/fullaccess";
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = "B2C_1_susi";
//})
//    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddOpenIdConnect("B2C_1_susi", options => {
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.Authority = AuthoritySignInSignUp;
//    options.ClientId = ClientID;
//    options.ClientSecret = ClientSecret;
//    options.ResponseType = "code";
//    options.Scope.Add(Scope);
//    options.SaveTokens = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
//    };
//        options.Events = new OpenIdConnectEvents
//        {
//            OnTokenValidated = async opt =>
//            {

//                string? role = opt.Principal.FindFirstValue("extension_Role");
//                if (!string.IsNullOrEmpty(role))
//                {
//                    var claims = new List<Claim>
//                    {
//                    new Claim(ClaimTypes.Role, role)
//                    };

//                    var appIdentity = new ClaimsIdentity(claims);
//                    opt.Principal.AddIdentity(appIdentity);
//                }
//            }
//        };
//    }).AddOpenIdConnect("B2C_1_edit", GetOpenIDConnectEditPolicyOptions("B2C_1_edit"))

//;

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


//Action<OpenIdConnectOptions> GetOpenIDConnectEditPolicyOptions(string policy) => options =>
//{
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.Authority = AuthorityEdit;
//    options.ClientId = ClientID;
//    options.ClientSecret = ClientSecret;
//    options.ResponseType = "code";
//    options.Scope.Add(Scope);
//    options.CallbackPath = "/signin-oidc-" + policy;
//    options.SaveTokens = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
//    };

//    options.Events = new OpenIdConnectEvents
//    {
//        OnMessageReceived = context =>
//        {
//            if (!string.IsNullOrEmpty(context.ProtocolMessage.Error) &&
//            !string.IsNullOrEmpty(context.ProtocolMessage.ErrorDescription))
//            {
//                if (context.ProtocolMessage.Error.Contains("access_denied"))
//                {
//                    context.HandleResponse();
//                    context.Response.Redirect("/");
//                }

//            }
//            return Task.FromResult(0);
//        }
//    };
//};
