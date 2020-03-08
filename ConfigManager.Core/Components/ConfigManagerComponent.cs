using ConfigManager.Core.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Core.Sync;
using Umbraco.Web;
using Umbraco.Web.Cache;
using Umbraco.Web.Editors;
using Umbraco.Web.JavaScript;
using IComponent = Umbraco.Core.Composing.IComponent;

namespace ConfigManager.Core.Components
{
    public class ConfigManagerComponent : IComponent
    {
        private readonly AppCaches _appCaches;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly Configs _configs;
        private readonly ILogger _logger;

        public ConfigManagerComponent(AppCaches appCaches, IUmbracoContextAccessor umbracoContextAccessor, Configs configs, ILogger logger)
        {
            _appCaches = appCaches;
            _umbracoContextAccessor = umbracoContextAccessor;
            _configs = configs;
            _logger = logger;
        }

        public void Initialize()
        {
            UmbracoApplicationBase.ApplicationInit += UmbracoApplicationBase_ApplicationInit;
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }
        private void ServerVariablesParser_Parsing(object sender, System.Collections.Generic.Dictionary<string, object> e)
        {

            if (HttpContext.Current == null) throw new InvalidOperationException("HttpContext is null");

            if (e.ContainsKey("configManager")) return;
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));
            e["configManager"] = new Dictionary<string, object>
            {
                {"configManagerEditorsBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<ConfigEditorController>(controller => controller.GetByPath(null))}
            };

            object found;
            if (e.TryGetValue("umbracoUrls", out found))
            {
                var umbUrls = (Dictionary<string, object>)found;
                umbUrls["configManagerEditorsBaseUrl"] = urlHelper.GetUmbracoApiServiceBaseUrl<ConfigEditorController>(controller => controller.GetByPath(null));
            }
            else
            {
                e["umbracoUrls"] = new Dictionary<string, object>
                {
                    {"configManagerEditorsBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<ConfigEditorController>(controller => controller.GetByPath(null))}
                };
            }
        }

        private void App_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (_appCaches?.RequestCache.Get("config-refresh-routes") == null) return;
            //the token was found so that means one or more articulate root nodes were changed in this request, rebuild the routes.
           // _articulateRoutes.MapRoutes(RouteTable.Routes);
        }

        private void UmbracoApplicationBase_ApplicationInit(object sender, EventArgs e)
        {
            var app = (UmbracoApplicationBase)sender;
            //app.ResolveRequestCache += App_ResolveRequestCache;
            app.PostRequestHandlerExecute += App_PostRequestHandlerExecute;
        }

        public void Terminate()
        {
        }
    }
}
