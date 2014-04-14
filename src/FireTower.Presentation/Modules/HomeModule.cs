using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Responses;

namespace FireTower.Presentation.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            RedirectIndex();
        }

        void RedirectIndex()
        {
            Get["/"] = _ => new RedirectResponse("/app/www/index.html");
        }
    }
}