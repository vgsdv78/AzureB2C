using System.Diagnostics;
using System.Net.Http.Headers;
using AzureB2CWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureB2CWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHttpClientFactory _httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles ="homeowner")]
        public IActionResult HomeOwner()
        {
            return View();
        }
        [Authorize(Roles = "customer")]
        public IActionResult Customer()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            var scheme = "B2C_1_susi";
            var redirectUrl=Url.ActionContext.HttpContext.Request.Scheme + 
                "://" + Url.ActionContext.HttpContext.Request.Host;
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
            }, scheme);


        }

        public IActionResult SignOut()
        {
            var scheme = "B2C_1_susi";
            return SignOut(new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme, scheme);
        }
        public IActionResult EditProfile()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, "B2C_1_edit");
        }

        public async Task<IActionResult> APICall()
        {
            var client = _httpClientFactory.CreateClient();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44323/WeatherForecast");
            request.Headers.Authorization = 
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            var response = await client.SendAsync(request);

            if(response.StatusCode!= System.Net.HttpStatusCode.OK)
            {
                //error
            }

            return View("APICall",await response.Content.ReadAsStringAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
