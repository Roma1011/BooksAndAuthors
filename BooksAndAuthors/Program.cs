using System.Text;
using BooksAndAuthors.Data;
using BooksAndAuthors.Data.Models;
using BooksAndAuthors.Data.Services;
using BooksAndAuthors.Data.Services.Authentication_Service;
using BooksAndAuthors.Data.Services.Author_Services;
using BooksAndAuthors.Data.Services.Book_Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);
var configuration =new ConfigurationBuilder().AddJsonFile("appsettings.json").Build(); ;

Log.Logger=new LoggerConfiguration().ReadFrom.Configuration(configuration).MinimumLevel.Debug()
	.WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();
Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey =
			new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretKey"))),
		ValidateLifetime = true,
		ValidateAudience = false,
		ValidateIssuer = false,
		ClockSkew = TimeSpan.Zero
	};
});
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Librery API", Version = "v1" });

	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter JWT Bearer token",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = JwtBearerDefaults.AuthenticationScheme
		}
	};

	c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ securityScheme, new[] { JwtBearerDefaults.AuthenticationScheme } }
	});
});
//builder.Services.AddTransient<BooksService>();
//builder.Services.AddTransient<AuthorServices>();

builder.Services.AddScoped<IBookService, BooksService>();
builder.Services.AddScoped<IAuthorServices,AuthorServices>();
builder.Services.AddScoped<IUserAuthServices, UserAuthServices>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(s=>s.SwaggerEndpoint("/swagger/v1/swagger.json","Librery API V1"));
}
//AppDbInitialer_Demo.Seed(app);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

