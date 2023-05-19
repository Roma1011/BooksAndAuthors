using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.Services.Author_Services;
using BooksAndAuthors.Data.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BooksAndAuthors.Data.Services
{
	public class AuthorServices:IAuthorServices
	{
		private readonly AppDbContext _context;

		public AuthorServices(AppDbContext context)
		{
				_context=context;
		}

		public async Task<AuthorWithBooksVM> GetAuthorWithBooks(string authorName)
		{
			if (authorName == null)
				throw new ArgumentNullException("Parameter is not valid format");

			var _author = await _context.Authors.Where(a =>
					a.FullName == authorName)
				.Select(s => new AuthorWithBooksVM()
				{
					FullName = s.FullName,
					YearOfBirth = s.YearOfBirth ?? 0, // Add null-check here
					Books = s.Book_Authors.Select(s => s.Book.Title).ToList()
				}).FirstOrDefaultAsync();


			return _author;
		}

		public async Task <Author> AddAuthor(AuthorVM author)
		{
			var _author = new Author()
			{
				FullName = author.FullName,
				YearOfBirth = author.YearOfBirth,
			};
			await _context.Authors.AddAsync(_author);
			if (await _context.SaveChangesAsync() == 0)
			{
				throw new DbUpdateException("Failed to save author.");
			}
			return _author;
		}
	}
}
