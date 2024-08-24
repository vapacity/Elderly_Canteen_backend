using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FormDataOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Source.Id == "FormFile" || p.Source.Id == "Form")
            .ToList();

        if (formParams.Any())
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>()
                        }
                    }
                }
            };

            foreach (var param in formParams)
            {
                if (param.ModelMetadata.IsComplexType && param.ModelMetadata.ModelType != typeof(IFormFile))
                {
                    var properties = param.ModelMetadata.Properties;
                    foreach (var prop in properties)
                    {
                        var propSchema = new OpenApiSchema
                        {
                            Type = "string"
                        };
                        operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(prop.PropertyName, propSchema);
                    }
                }
                else if (param.ModelMetadata.ModelType == typeof(IFormFile))
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(param.Name, new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    });
                }
                else
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(param.Name, new OpenApiSchema
                    {
                        Type = "string"
                    });
                }
            }
        }
    }
}
