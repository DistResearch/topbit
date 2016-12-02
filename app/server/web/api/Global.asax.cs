namespace App.Web.Service
{
    using System;
    using System.ServiceModel.Activation;
    using System.Web;
    using System.Web.Routing;
    using App.Web.Business;

    public class Global : HttpApplication
    {
        private void RegisterRoutes()
        {
            // Edit the base address of PoolService by replacing the "PoolService" string below
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();

            AppContext.Start();
        }
    }
}
