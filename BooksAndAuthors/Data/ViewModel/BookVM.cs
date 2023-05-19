namespace BooksAndAuthors.Data.ViewModel
{
	public class BookWithAuthorsVM
	{
		public string? Title { get; set; }

		public string? Description { get; set; }

		public byte[]? Image { get; set; }=null;

		public int? Rating { get; set; }

		public int? PublicationYear { get; set; }
		public bool IsTaken { get; set; }

		public List<string>? AuthorFullName { get; set; }
	}

	public class BookWithAuthorsFillingVM
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public int? Rating { get; set; }
		public int? PublicationYear { get; set; }
		public List<string> AuthorFullName { get; set; }
	}
}
