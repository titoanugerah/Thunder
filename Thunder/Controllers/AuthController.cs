using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Thunder.DataAccess;
using Thunder.Models;
using Thunder.ViewModel;

namespace Thunder.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> logger;
        private readonly ThunderDB thunderDB;

        public AuthController(ILogger<AuthController> _logger, ThunderDB _thunderDB)
        {
            logger = _logger;
            thunderDB = _thunderDB; 
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Login()
        {
            AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        private string GetClaims(List<UserClaim> claims, string key)
        {
            return claims.Where(claim => claim.Name == key).FirstOrDefault().Value;
        }

        public async Task<IActionResult> GoogleResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            List<UserClaim> claims = result.Principal
                .Identities
                .FirstOrDefault()
                .Claims
                .Select(response => new UserClaim
                {
                    Issuer = response.Issuer,
                    OriginalIssuer = response.OriginalIssuer,
                    Type = response.Type,
                    Value = response.Value
                }).ToList();

            User user = thunderDB.User
                .Where(user => user.Email == GetClaims(claims, "emailaddress"))
                .FirstOrDefault();

            user.Name = GetClaims(claims, "name");
            user.Email = GetClaims(claims, "emailaddress");
            //user.Image = GetClaims(claims, "picture");
            if (user == null)
            {
                user.CreatedDate = DateTime.Now;
                user.CreatedById = 1;
                user.IsExist = 1;
                user.RoleId = 1;
                thunderDB.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                await thunderDB.User.AddAsync(user);
            }
            else
            {
                thunderDB.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                thunderDB.User.Update(user);
            }
            await thunderDB.SaveChangesAsync();

            List<Claim> loggedUser = new List<Claim>();
            loggedUser.Add(new Claim("Id", user.Id.ToString()));
            loggedUser.Add(new Claim("Email", user.Email));
            loggedUser.Add(new Claim("RoleId", user.RoleId.ToString()));
            loggedUser.Add(new Claim("Name", user.Name));
            loggedUser.Add(new Claim("IsExist", user.IsExist.ToString()));
            //loggedUser.Add(new Claim("Image", user.Image.ToString()));

            ClaimsIdentity userIdentity = new ClaimsIdentity(loggedUser, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout() 
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
