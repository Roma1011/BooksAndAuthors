using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.Services;
using BooksAndAuthors.Data.Services.Book_Service;
using BooksAndAuthors.Data.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.Helpers;

namespace BooksAndAuthors.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BooksController : ControllerBase
	{
		private readonly IBookService _booksService;
		private readonly ILogger<BooksController> _logger;
		public BooksController(IBookService booksService, ILogger<BooksController> logger)
		{
			_booksService = booksService;
			_logger = logger;
		}

		[HttpGet("get-all-books")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]

		public async Task <IActionResult> GetAllBooks()
		{
			try
			{
				var allBooks =await _booksService.GetAllBooks();
				_logger.LogInformation("The command was successfully executed for all books GetAllBooks()");
				return Ok(allBooks);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}

		}
		//--------------------------------------------------------------------------------------------

		[HttpGet("get-book-for-title/{title}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetBookByTitle(string title)
		{
			try
			{
				var allBooks = await _booksService.GetBookByTitle(title);
				_logger.LogInformation("The command was successfully executed for books title GetBookByTitle()");
				return Ok(allBooks);
			}
			catch (ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}

		}
		//--------------------------------------------------------------------------------------------
		[Authorize]
		[HttpPost("add-book-with-authors")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]

		public async Task<IActionResult> AddBook([FromForm] BookWithAuthorsFillingVM book, IFormFile image)
		{
			try
			{
				var bookResult =await _booksService.AddBookWithAuthors(book, image);
				_logger.LogInformation("The command was successfully executed for AddBook()");
				return Created("Created successfull", bookResult);
			}
			catch (InvalidDataException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(500, e.Message);
			}
			catch(ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (DbUpdateException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(500, e.Message);
			}
		}

		//--------------------------------------------------------------------------------------------
		[Authorize]
		[HttpPut("update-book-by-title/{title}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateBookDetalsByTitle(string title, [FromBody] BookWithAuthorsVM book)
		{
			try
			{
				var bookResult =await _booksService.UpdateBookByTitle(title, book);
				_logger.LogInformation("The command was successfully executed for UpdateBookByTitle()");
				return Ok(bookResult);
			}
			catch (DbUpdateException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(500, e.Message);
			}
			catch (ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}
		//--------------------------------------------------------------------------------------------

		[Authorize]
		[HttpPut("take-book-by-title and author/{title}/{author}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult>  TakeBook(string title, string author)
		{
			try
			{
				var bookResult =await _booksService.TakingOutTheBook(title, author);
				_logger.LogInformation("The command was successfully executed for TakeBook()");
				return Ok(bookResult);
			}
			catch (DbUpdateException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(500, e.Message);
			}
			catch (ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}

		}
		//--------------------------------------------------------------------------------------------

		[Authorize]
		[HttpPut("return-book-by-title and author/{title}/{author}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> ReturnBook(string title, string author)
		{
			try
			{
				var bookResult =await _booksService.ReturnTheBook(title, author);
				_logger.LogInformation("The command was successfully executed for ReturnBook()");
				return Ok(bookResult);
			}
			catch (DbUpdateException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(500, e.Message);
			}
			catch (ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}
		//--------------------------------------------------------------------------------------------
		[Authorize]
		[HttpDelete("delete-author-from-book{title}/{authorname}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> DeleteAuthor(string title, string authorname)
		{
			try
			{
				var bookResult =await _booksService.DeleteAuthorFromBook(title, authorname);
				_logger.LogInformation("The command was successfully executed for DeleteAuthor()");
				return Ok(bookResult);
			}
			catch (ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (DbUpdateException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(500, e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

		//--------------------------------------------------------------------------------------------

		[Authorize]
		[HttpDelete("delete-book/{title}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeleteBook(string title)
		{
			try
			{
				var bookResult =await _booksService.DeleteBook(title);
				_logger.LogInformation("The command was successfully executed for DeleteBook()");
				return NoContent();
			}
			catch (ArgumentNullException e)
			{
				_logger.LogInformation(e.Message);
				return BadRequest(e.Message);
			}
			catch (DbUpdateException e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
			catch (ArgumentException e)
			{
				_logger.LogInformation(e.Message);
				return NotFound(e.Message);
			}
			catch (Exception e)
			{
				_logger.LogInformation(e.Message);
				return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
			}
		}


	}
}
