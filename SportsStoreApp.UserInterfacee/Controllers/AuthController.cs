
 
using System.Security.Claims;
using System.Threading.Tasks;
using SportsStoreApp.UserInterface.Auth;
using SportsStoreApp.UserInterface.Helpers;
using SportsStoreApp.UserInterface.Models;
using SportsStoreApp.UserInterface.Models.Entities;
using SportsStoreApp.UserInterface.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace SportsStoreApp.UserInterface.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }

          var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
          return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        [Authorize]
        [HttpGet("Products")]
        public IActionResult Products()
        {
           var product= new List<Product> {
                new Product(1, "Product 1", "Category 1", "Product 1 (Category 1)", 100),
                new Product(2, "Product 2", "Category 1", "Product 2 (Category 1)", 100),
                new Product(3, "Product 3", "Category 1", "Product 3 (Category 1)", 100),
                new Product(4, "Product 4", "Category 1", "Product 4 (Category 1)", 100),
                new Product(5, "Product 5", "Category 1", "Product 5 (Category 1)", 100),
                new Product(6, "Product 6", "Category 2", "Product 6 (Category 2)", 100),
                new Product(7, "Product 7", "Category 2", "Product 7 (Category 2)", 100),
                new Product(8, "Product 8", "Category 2", "Product 8 (Category 2)", 100),
                new Product(9, "Product 9", "Category 2", "Product 9 (Category 2)", 100),
                new Product(10, "Product 10", "Category 2", "Product 10 (Category 2)", 100),
                new Product(11, "Product 11", "Category 3", "Product 11 (Category 3)", 100),
                new Product(12, "Product 12", "Category 3", "Product 12 (Category 3)", 100),
                new Product(13, "Product 13", "Category 3", "Product 13 (Category 3)", 100),
                new Product(14, "Product 14", "Category 3", "Product 14 (Category 3)", 100),
                new Product(15, "Product 15", "Category 3", "Product 15 (Category 3)", 100),
            };

            return Ok(product);
        }

}

    public class Product
    {
        public Product(int id, string name, string category, string description, int price)
        {
            this.Id = id;
            this.Name = name;
            this.Category = category;
            this.Description = description;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
    }
}