using BooksAndAuthors.Data.Services;
using BooksAndAuthors.Data.Services.Author_Services;
using BooksAndAuthors.Data.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAndAuthors.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthorController : ControllerBase
	{
		private readonly IAuthorServices _authorServices;
		private readonly ILogger<BooksController> _logger;
		public AuthorController(IAuthorServices authorServices, ILogger<BooksController> logger)
		{
			_authorServices = authorServices;
			_logger = logger;
		}

		[HttpGet("get-book-with-author/{fullName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]

		public async Task<IActionResult>  GetAuthorWithBook(string fullName)
		{
			try
			{
				var resulBooks =await _authorServices.GetAuthorWithBooks(fullName);
				_logger.LogInformation("The command was successfully executed for GetAuthorWithBook()");
				return Ok(resulBooks);
			}
			catch (ArgumentNullException e)
			{
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				return NotFound(e.Message);
			}
			catch(Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

		[Authorize]
		[HttpPost("add-author")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> AddAuthor([FromBody] AuthorVM author)
		{
			try
			{
				var bookAuthor =await _authorServices.AddAuthor(author);
				_logger.LogInformation("The command was successfully executed for AddAuthor()");
				return Created("Created successfull", bookAuthor);
			}
			catch (DbUpdateException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
			catch (Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}

		}
	}
}
