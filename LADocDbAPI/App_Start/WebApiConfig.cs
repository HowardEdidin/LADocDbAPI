#region

using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;

#endregion

namespace LADocDbAPI
{
    /// <summary>
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            // added Cors support ( Origins, headers, methods)
            var cors = new EnableCorsAttribute("http://localhost:17614", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
                );
        }
    }
}