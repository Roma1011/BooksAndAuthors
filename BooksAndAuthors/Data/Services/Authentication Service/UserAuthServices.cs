using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BooksAndAuthors.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;

namespace BooksAndAuthors.Data.Services.Authentication_Service
{
	public class UserAuthServices: IUserAuthServices
	{
		private IConfiguration _config;

		public UserAuthServices(IConfiguration config)
		{
			_config = config;
		}

		public async Task<string> LoginClient(Credential credential, DateTime duration)
		{
			if (credential.UserName == "admin" && credential.Password == "admin")
			{
				List<Claim> claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name,credential.UserName)
				};
				var key =Encoding.ASCII.GetBytes(_config.GetValue<string>("SecretKey"));

				var jwt = new JwtSecurityToken(
					claims: claims,
					notBefore: DateTime.UtcNow,
					expires: duration,
					signingCredentials: new SigningCredentials(
						new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature));
				return new JwtSecurityTokenHandler().WriteToken(jwt);
			}
			
			return string.Empty;
		}
	}
}
