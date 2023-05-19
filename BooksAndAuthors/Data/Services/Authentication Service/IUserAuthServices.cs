using BooksAndAuthors.Data.Models;

namespace BooksAndAuthors.Data.Services.Authentication_Service
{
	public interface IUserAuthServices
	{
		public Task<string> LoginClient(Credential credential,DateTime duration);
	}
}
