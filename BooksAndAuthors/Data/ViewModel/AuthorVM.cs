namespace BooksAndAuthors.Data.ViewModel
{
	public class AuthorVM
	{
		public string FullName { get; set; }
		public int YearOfBirth { get; set; }
	}

	public class AuthorWithBooksVM
	{
		public string FullName { get; set; }
		public int YearOfBirth { get; set; }
		public List<string>? Books { get; set; }
	}
}
