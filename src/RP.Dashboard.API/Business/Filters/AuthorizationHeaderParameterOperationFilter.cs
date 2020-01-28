using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RP.Dashboard.API.Business.Filters
{
	[ExcludeFromCodeCoverage]
	public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
			var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
			var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

			if (isAuthorized && !allowAnonymous)
			{
				if (operation.Parameters == null)
					operation.Parameters = new List<OpenApiParameter>();

				operation.Parameters.Add(new OpenApiParameter
                {
					Name = "Authorization",
					In = ParameterLocation.Header,
					Description = "access token",
					Required = true,
				});
			}
		}
    }
}