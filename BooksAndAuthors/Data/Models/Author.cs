using System.Text.Json.Serialization;

namespace BooksAndAuthors.Data.Models
{
	public class Author
	{
		public int Id { get; set; }
		public string FullName { get; set; }
		public int? YearOfBirth { get; set; }

		[JsonIgnore]
		public List<Book_Author> Book_Authors { get; set; }
	}

}
