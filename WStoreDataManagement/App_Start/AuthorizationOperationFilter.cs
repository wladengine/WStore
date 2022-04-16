using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace WStoreDataManagement.App_Start
{
    public class AuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            operation.parameters.Add(new Parameter()
            {
                name = "Authorization",
                @in = "header", //we gonna put this parameter
                description = "Access token", // thats gonna show in our documentation
                required = false, //cuz, some methods don't need an authorization
                type = "string",

            });
        }
    }
}