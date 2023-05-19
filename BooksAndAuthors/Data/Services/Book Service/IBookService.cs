using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace BooksAndAuthors.Data.Services.Book_Service
{
	public interface IBookService
	{
		public Task<List<BookWithAuthorsVM>> GetAllBooks();
		public Task<BookWithAuthorsVM> GetBookByTitle(string title);
		public Task<Book> AddBookWithAuthors(BookWithAuthorsFillingVM book, IFormFile Image);
		public Task<Book> UpdateBookByTitle(string title, BookWithAuthorsVM book);
		public Task<string> TakingOutTheBook(string title, string FullNameAuthor);
		public Task<string> ReturnTheBook(string title, string FullNameAuthor);
		public Task<Book> DeleteAuthorFromBook(string title, string FullNameAuthor);
		public Task<Book> DeleteBook(string title);
	}
}
