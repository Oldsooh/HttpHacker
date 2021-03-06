﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HttpHacker
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{*.}",
                defaults: new { controller = "handler", action="handlerequest", id = RouteParameter.Optional }
            );
        }
    }
}
