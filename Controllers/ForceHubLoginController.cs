using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using WeighForce.Data.Repositories;
using System.Collections.Specialized;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using WeighForce.Models;
using System.Threading.Tasks;

namespace WeighForce.Controllers
{
  public class ForceHubController : ControllerBase
  {
    private readonly IMetaRepository _repo;
    public const string SessionKeyState = "_State";
    public string FORCEHUB_CLIENT_ID;
    public string FORCEHUB_CLIENT_SECRET;
    public string FORCEHUB_URL;
    public string APP_URL;
    public string CLIENT_APP_URL;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public HttpClient Client;
    public ForceHubController(IMetaRepository context, IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
      _repo = context;
      _userManager = userManager;
      _signInManager = signInManager;
      Client = new HttpClient();
      FORCEHUB_CLIENT_ID = configuration.GetValue<string>("Forcehub:ClientID", "");
      FORCEHUB_CLIENT_SECRET = configuration.GetValue<string>("Forcehub:ClientSecret", "");
      FORCEHUB_URL = configuration.GetValue<string>("Forcehub:Url", "");
      APP_URL = configuration.GetValue<string>("Forcehub:AppUrl", "");
      CLIENT_APP_URL = configuration.GetValue<string>("Forcehub:ClientAppUrl", "");
    }

    [HttpGet("login-forcehub")]
    public ActionResult InitiateLogin()
    {
      var state = _randomString(40);
      HttpContext.Session.SetString(SessionKeyState, state);
      var sessionState = HttpContext.Session.GetString(SessionKeyState);
      var param = new Dictionary<string, string>() {
        {"client_id", FORCEHUB_CLIENT_ID},
        {"redirect_uri", APP_URL + "/login-callback"},
        {"response_type", "code"},
        {"scope", ""},
        {"state", state }
      };
      var url = FORCEHUB_URL + "/oauth/authorize";
      var newUrl = new Uri(QueryHelpers.AddQueryString(url, param));
      return Redirect(newUrl.ToString());
    }
    [HttpGet("login-callback")]
    public async Task<ActionResult> CompleteLoginAsync(string code, string state)
    {
      // string sessionState = HttpContext.Session.GetString(SessionKeyState);
      // System.Console.WriteLine(sessionState);
      // if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyState)) || sessionState == state)
      // {
      //   return Unauthorized();
      // }
      using (var wb = new WebClient())
      {
        var data = new NameValueCollection();
        data.Add("grant_type", "authorization_code");
        data.Add("client_id", FORCEHUB_CLIENT_ID);
        data.Add("client_secret", FORCEHUB_CLIENT_SECRET);
        data.Add("redirect_uri", APP_URL + "/login-callback");
        data.Add("code", code);
        var response = wb.UploadValues(FORCEHUB_URL + "/oauth/token", "POST", data);
        var res = JsonSerializer.Deserialize<Dictionary<string, object>>(Encoding.UTF8.GetString(response));
        var headers = new WebHeaderCollection();
        headers.Add("Accept", "application/json");
        headers.Add("Authorization", "Bearer " + res.GetValueOrDefault("access_token"));
        wb.Headers = headers;
        var userResponse = wb.DownloadString(FORCEHUB_URL + "/api/user");
        var user = JsonSerializer.Deserialize<HubUser>(userResponse);
        var appUser = await _userManager.FindByEmailAsync(user.email);
        if(appUser == null) {
          var newUser = new ApplicationUser
            {
                UserName = user.email,
                Email = user.email,
                Name = user.name,
            };
            _userManager.CreateAsync(newUser).GetAwaiter().GetResult();
            appUser = await _userManager.FindByEmailAsync(user.email);
        }
        await _signInManager.SignInAsync(appUser, isPersistent: false);
        return Redirect(CLIENT_APP_URL);
      }
    }
    public string _randomString(int length)
    {
      Random random = new Random();

      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
  }
  public class HubUser {
    public string email { get; set; }
    public string name { get; set; }
  }
}

