#region

using System.Web;
using System.Web.Http;

#endregion

namespace LADocDbAPI
{
    /// <summary>
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}