using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BooksAndAuthors.Data.Models
{
	public class Book
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public byte[]? Image { get; set; }=null;

		public int? Rating { get; set; }

		public int? PublicationYear { get; set; }
		public bool IsTaken { get; set; }

		public DateTime DataAdded { get; set; }

		[JsonIgnore]
		public List<Book_Author> Book_Author { get; set; }
	}
}
