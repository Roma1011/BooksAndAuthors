using BooksAndAuthors.Data.Models;

namespace BooksAndAuthors.Data
{
	//public class AppDbInitialer_Demo
	//{
	//	public static void Seed(IApplicationBuilder applicationBuilder)
	//	{
	//		using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
	//		{
	//			var context=serviceScope.ServiceProvider.GetService<AppDbContext>();

	//			if (!context.Books.Any())
	//			{
	//					context.Books.AddRange(new Book() 
	//					{
	//						Title = "1st book Title",
	//						Description = "1st book description",
	//						IsTaken = false,
	//						PublicationYear = DateTime.Now.AddDays(-10),
	//						Rating = 5,
	//						Image = "url"

	//					},
	//					new Book()
	//					{
	//						Title = "2st book Title",
	//						Description = "2st book description",
	//						IsTaken = false,
	//						PublicationYear = DateTime.Now.AddDays(-10),
	//						Rating = 5,
	//						Image = "url"
	//					});
	//				context.SaveChanges();
					
	//			}
	//		}
	//	}
	//}
}
