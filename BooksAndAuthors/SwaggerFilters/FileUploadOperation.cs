using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperation : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		if (operation.Parameters != null)
		{
			var fileUploadParameter = context.ApiDescription.ParameterDescriptions
				.FirstOrDefault(p => p.ModelMetadata.ModelType == typeof(IFormFile));

			if (fileUploadParameter != null)
			{
				operation.Parameters.Add(new OpenApiParameter
				{
					Name = fileUploadParameter.Name,
					In = ParameterLocation.Header,
					Schema = new OpenApiSchema
					{
						Type = "string",
						Format = "binary"
					},
					Description = "Upload Image",
					Extensions = new Dictionary<string, IOpenApiExtension>
					{
						{ "x-ms-meta-filetypes", new OpenApiString("jpg,png") }
					}
				});
			}
		}
	}
}