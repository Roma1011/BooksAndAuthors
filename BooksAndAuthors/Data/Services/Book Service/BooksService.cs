using System.Data;
using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.Services.Book_Service;
using BooksAndAuthors.Data.ViewModel;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BooksAndAuthors.Data.Services
{
	public class BooksService:IBookService
	{
		private readonly AppDbContext _context;

		public BooksService(AppDbContext context)
		{
			_context=context;
		}

		public async Task<List<BookWithAuthorsVM>> GetAllBooks()
		{
			var booksWithAuthors = await _context.Books
				.Include(b => b.Book_Author)
				.ThenInclude(ba => ba.Author)
				.Select(b => new BookWithAuthorsVM
				{
					Title = b.Title,
					Description = b.Description,
					Image = b.Image,
					Rating = b.Rating,
					PublicationYear = b.PublicationYear,
					IsTaken = b.IsTaken,
					AuthorFullName = b.Book_Author.Select(ba => ba.Author.FullName).ToList()
				})
				.ToListAsync();

			if (booksWithAuthors == null || booksWithAuthors.Count == 0)
			{
				throw new ArgumentException("Books are not available");
			}

			return booksWithAuthors;
		} 

		public async Task<BookWithAuthorsVM> GetBookByTitle(string title)
		{
			if(string.IsNullOrEmpty(title))
				throw new ArgumentNullException(nameof(title), "Parameter Is Required!");
			
			var bookByTitle = await _context.Books.Where(w=>w.Title==title).Select(book => new BookWithAuthorsVM()
			{
				Title = book.Title,
				Description = book.Description,
				Image = book.Image,
				Rating = book.Rating,
				PublicationYear = book.PublicationYear,
				IsTaken = book.IsTaken,
				AuthorFullName = book.Book_Author.Select(n => $"{n.Author.FullName}{n.Author.YearOfBirth}").ToList()
			}).FirstOrDefaultAsync();

			if (bookByTitle == null)
				throw new ArgumentException("Book Not Found");

			return bookByTitle;
		}
		public async Task<Book> AddBookWithAuthors(BookWithAuthorsFillingVM book, IFormFile image)
		{
			byte[] imageBytes=null;
			if(book==null)
				throw new ArgumentNullException(nameof(book));
			var existingBook = await _context.Books.Where(w => w.Title == book.Title).FirstOrDefaultAsync();
			if (existingBook != null)
			{
				throw new ArgumentException("The book already exists");
			}

			if (image.Length > 0)
			{
				var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
				if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jfif")
				{
					throw new InvalidDataException("Wrong format photo");
				}

				using (var stream = new MemoryStream())
				{
					using (var imageStream = image.OpenReadStream())
					{
						var image_local =await SixLabors.ImageSharp.Image.LoadAsync(imageStream);
						image_local.Mutate(x => x.Resize(new ResizeOptions
						{
							Size = new Size(800, 600),
							Mode = ResizeMode.Max
						}));
						await image_local.SaveAsync(stream, new JpegEncoder { Quality = 50 }); 
					}
					imageBytes = stream.ToArray();
				}
			}
			var _book = new Book()
			{
				Title = book.Title,
				Description = book.Description,
				Image = imageBytes??null,
				Rating = book.Rating,
				PublicationYear = book.PublicationYear,
				IsTaken = false,
				DataAdded = DateTime.Now
			};
			await _context.Books.AddAsync(_book);
			await _context.SaveChangesAsync();
			foreach (var FullNameAuthor in book.AuthorFullName)
			{
				var author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == FullNameAuthor);

				if (author != null)
				{
					var bookAuthor = new Book_Author
					{
						BookId = _book.Id,
						AuthorId = author.Id
					};
					await _context.Books_Authors.AddAsync(bookAuthor);
					await _context.SaveChangesAsync();
				}
				else
					throw new ArgumentException("The book has already been added to the database, but the author does not exist, please add the author and then correct it");
			}
			return _book;
		}

		public async Task<Book> UpdateBookByTitle(string title,BookWithAuthorsVM book)
		{
			if(string.IsNullOrEmpty(title))
				throw new ArgumentNullException(nameof(title), "Parameter Is Required!");

			var _book =await _context.Books.FirstOrDefaultAsync(f => f.Title == title);
			if (_book != null)
			{
				_book.Title = book.Title ?? _book.Title;
				_book.Description = book.Description ?? _book.Description;
				_book.Image = book.Image ?? _book.Image;
				_book.Rating = book.Rating ?? _book.Rating;
				_book.PublicationYear = book.PublicationYear ?? _book.PublicationYear;
				_book.IsTaken = book.IsTaken;

				_context.Entry(_book).State = EntityState.Modified;


				if (await _context.SaveChangesAsync() == 0)
					throw new DbUpdateException("Something is Wrong");

				if (book.AuthorFullName != null)
				{
					foreach (var FullNameAuthor in book.AuthorFullName)
					{
						var author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == FullNameAuthor);

						if (author != null)
						{
							var bookAuthor = new Book_Author
							{
								BookId = _book.Id,
								AuthorId = author.Id
							};
							await _context.Books_Authors.AddAsync(bookAuthor);
							if (await _context.SaveChangesAsync() == 0)
								throw new DbUpdateException("Something is Wrong");
						}
					}
				}
			}
			else
			{
				throw new ArgumentException("Book not found");
			}
			return _book;
		}
		public async Task<string> TakingOutTheBook(string title, string FullNameAuthor)
		{
			if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(FullNameAuthor))
				throw new ArgumentNullException("Parameter is Required!");

			bool reTakeBookHelperMethod = false;
			const int takingOutMethod = 1;
			var _book =await _context.Books.FirstOrDefaultAsync(f => f.Title == title);
			var _author =await _context.Authors.FirstOrDefaultAsync(a => a.FullName == FullNameAuthor);
			var bookAuthor =await _context.Books_Authors.FirstOrDefaultAsync(ba => ba.BookId == _book.Id && ba.AuthorId == _author.Id);

			if (_book == null || _author == null || bookAuthor == null)
				throw new ArgumentException("Data processing failed");

			reTakeBookHelperMethod =await ChackTaking(_book, _author, takingOutMethod);
			if (reTakeBookHelperMethod == false)
				return "The book has already been taken";

			bookAuthor.Book.IsTaken=true;
			await _context.SaveChangesAsync();

			return "It's a very good choice, I wish you a good adventure";
		}

		public async Task<string> ReturnTheBook(string title, string FullNameAuthor)
		{
			const int returnBookMethod= 2;
			bool reTakeBookHelperMethod = false;

			if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(FullNameAuthor))
				throw new ArgumentNullException("Parameter is Required!");

			var _book = await _context.Books.FirstOrDefaultAsync(f => f.Title == title);
			var _author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == FullNameAuthor);

			if (_book == null || _author == null)
			{
				throw new ArgumentException("Book or Author not found!");
			}

			var bookAuthor = await _context.Books_Authors.FirstOrDefaultAsync(ba => ba.BookId == _book.Id && ba.AuthorId == _author.Id);

			if (bookAuthor == null)
			{
				throw new ArgumentException("Book and Author combination not found!");
			}
			reTakeBookHelperMethod =await ChackTaking(_book, _author, returnBookMethod);
			if (reTakeBookHelperMethod == false)
				return "The book is already in the library";

			bookAuthor.Book.IsTaken = false;
			await _context.SaveChangesAsync();
			return "Thanks";
		}

		public async Task<Book> DeleteAuthorFromBook(string title, string fullNameAuthor)
		{
			if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(fullNameAuthor))
				throw new ArgumentNullException("Title and full name of author are required.");

			var book =await _context.Books.FirstOrDefaultAsync(b => b.Title == title);
			var author = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == fullNameAuthor);

			if (book == null)
				throw new ArgumentException($"Book with title '{title}' not found.");
			if (author == null)
				throw new ArgumentException($"Author with full name '{fullNameAuthor}' not found.");

			var bookAuthor =await _context.Books_Authors.FirstOrDefaultAsync(ba => ba.BookId == book.Id && ba.AuthorId == author.Id);

			if (bookAuthor == null)
				throw new InvalidOperationException($"Author with full name '{fullNameAuthor}' not found in book with title '{title}'.");

			_context.Books_Authors.Remove(bookAuthor);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new DbUpdateException("Error occurred while deleting author from book.", ex);
			}

			return book;
		}


		public async Task<Book> DeleteBook(string title)
		{
			if (string.IsNullOrEmpty(title))
				throw new ArgumentNullException(nameof(title), "Title parameter is required!");

			var book =await _context.Books.FirstOrDefaultAsync(b => b.Title == title);
			if (book == null)
				throw new ArgumentException($"Book with title '{title}' not found");

			_context.Books.Remove(book);

			if (await _context.SaveChangesAsync() == 0)
				throw new DbUpdateException("Error deleting book");

			return book;
		}

		public async Task<bool> ChackTaking(Book _book,Author _author,int method)
		{
			if (_book.IsTaken == false && method ==1)
				return true;

			if(_book.IsTaken == true && method == 2)
				return true;

			return false;
		}
	}
}
