using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Worklance.Api.Infrastructure.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody != null && operation.RequestBody.Content.TryGetValue("multipart/form-data", out var mediaType))
            {
                if (mediaType.Schema != null && mediaType.Schema.Properties.TryGetValue("AadhaarProof", out var property))
                {
                    property.Type = "string";
                    property.Format = "binary";
                }
            }
        }
    }
}
