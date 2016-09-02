﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CostEffectiveCode.AutoMapper;
using CostEffectiveCode.Cqrs.Commands;
using CostEffectiveCode.Cqrs.Queries;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Costeffectivecode.WebApi2.Example
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);


            AutoMapperWrapper.Init();

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            var registration = Lifestyle.Scoped.CreateRegistration<CommandQueryFactory>(container);


            container.AddRegistration(typeof(IQueryFactory), registration);
            container.AddRegistration(typeof(ICommandFactory), registration);

            // This is an extension method from the integration package.
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

        }
    }
}
