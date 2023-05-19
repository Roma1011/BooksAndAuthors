using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.ViewModel;

namespace BooksAndAuthors.Data.Services.Author_Services
{
	public interface IAuthorServices
	{
		public Task <AuthorWithBooksVM> GetAuthorWithBooks(string authorName);
		public Task <Author> AddAuthor(AuthorVM author);
	}
}
