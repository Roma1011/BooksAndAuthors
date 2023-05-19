using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.Services.Authentication_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksAndAuthors.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private IUserAuthServices _userAuthServices;
		private readonly ILogger<BooksController> _logger;
		public AuthenticationController(IUserAuthServices userAuthServices, ILogger<BooksController> logger)
		{
			_userAuthServices = userAuthServices;
			_logger = logger;
		}
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> Authenticate([FromBody] Credential credential)
		{
			var duration = DateTime.UtcNow.AddMinutes(10);
			var tokResult=await _userAuthServices.LoginClient(credential, duration);

			if (string.IsNullOrEmpty(tokResult))
			{
				ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
				_logger.LogInformation("Unauthorized");
				return Unauthorized(ModelState);
			}

			_logger.LogInformation("Token Creted Successful");

			return Ok(new
			{
				tokResult,
				durationTime=duration
			});
		}

	}
}
