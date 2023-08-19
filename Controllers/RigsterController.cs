using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chrestin_Project.DTO;
using Chrestin_Project.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Chrestin_Project.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly  UserManager<ApplicationUser> userManager;
		private readonly IConfiguration config;
		public AccountController(UserManager<ApplicationUser> userManager,IConfiguration config) 
		{
			this.userManager = userManager;
			this.config = config;

		}

		public UserManager<ApplicationUser> UserManager { get; }


		//////Create new user POST method
		[HttpPost("register")]
		public async Task<IActionResult> Register(RigesterUserDto userDto)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser newUser = new ApplicationUser();
				newUser.Email = userDto.Email;
				newUser.UserName = userDto.UserName;
				IdentityResult result = await userManager.CreateAsync(newUser, userDto.Password);
				if (result.Succeeded)
				{
					return Ok("Account Created");
				}
				return BadRequest("Failed to create Account 1");

			}
			return BadRequest("Failed to create Account 2");
		}
		///////// Check user if is found {LOG IN}
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginUserDto userDto)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = await userManager.FindByNameAsync(userDto.UserName);
				if (user != null)
				{
					 bool found =  await userManager.CheckPasswordAsync(user, userDto.Password);
					if (found)
					{

						/////Create Claims of TOKEN 
						var mycaliams = new List<Claim>();
						mycaliams.Add(new Claim(ClaimTypes.Name, user.UserName)); ///Custom claims 
						mycaliams.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));///Custom claims 
						mycaliams.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); ////predefined claims 
						 ////add Role to Claims 
						var roles =  await userManager.GetRolesAsync(user);
						foreach (var role in roles)
						{
							mycaliams.Add(new Claim(ClaimTypes.Role, role));
						}

						////// Create signingCredentials
						SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecutiryKey"]));
						SigningCredentials signInCreads = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);



						//////Create TOKEN 
						JwtSecurityToken mytoken = new JwtSecurityToken(
							issuer : config["JWT:ValidIssuer"], ///URL OF Service Provider
							audience: config["JWT:ValidAudience"], /// URL of Service Consumer
							claims: mycaliams,/// Paylod of TOKEN
							expires : DateTime.Now.AddDays(1), /// Expiration Date of TOKEN
							signingCredentials: signInCreads

							);;

						return Ok(new
						{
							token =  new JwtSecurityTokenHandler().WriteToken(mytoken),
							expiration = mytoken.ValidTo,
						});
						;
					}

					return Unauthorized();
				}

				return Unauthorized();
			}
			return Unauthorized();
		}
	}
}
