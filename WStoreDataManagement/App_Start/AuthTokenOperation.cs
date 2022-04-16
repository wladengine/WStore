using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace WStoreDataManagement.App_Start
{
    // Thanks for guys from StackOverflow for answering me, how to add new endpoint for token-based authentification!
    // https://stackoverflow.com/questions/51117655/how-to-use-swagger-in-asp-net-webapi-2-0-with-token-based-authentication
    public class AuthTokenOperation : IDocumentFilter 
    {
        //this is the implementation of basic Swagger interface to add some new functionality
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/token", new PathItem() //the new route for Swagger
            {
                post = new Operation() //it's a POST command
                {
                    tags = new List<string> { "Auth" }, //has the tags
                    consumes = new List<string> { //using this kind of data (like you doing this in Postman)
                        "application/x-www-urlencoded"
                    },
                    parameters = new List<Parameter>() //and have theese 3 patameters
                    {
                        new Parameter()
                        {
                            type = "string",
                            name = "grant_type",
                            required = true, //this is nessecary param
                            @in = "formData",
                            @default = "password" //just make a default input value (we will use this only for authentification, yea?)
                        },
                        new Parameter()
                        {
                            type = "string",
                            name = "username",
                            required = false, //this is not
                            @in = "formData"
                        },
                        new Parameter()
                        {
                            type = "string",
                            name = "password",
                            required = false, //and this is not too
                            @in = "formData"
                        },
                    }
                }
            });
        }
    }
}