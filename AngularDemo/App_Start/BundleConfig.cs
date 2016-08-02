using AngularDemo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace AngularDemo.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(Constants.AngularBundle)
                    .Include("~/Scripts/angular.js", 
                            "~/Scripts/angular-route.js")
                    );

            bundles.Add(new AngularTemplateBundle("criminalAppModule", Constants.AcademyTemplatesBundle)
                           .IncludeDirectory("~/scripts/criminalApi/views", "*.html", true));

            bundles.Add(new StyleBundle(Constants.Bootstrap)
                .Include("~/Content/bootstrap.css")
                .Include("~/scripts/bootstrap.js")
                );

            bundles.Add(new ScriptBundle(Constants.jQueryBundle)
                    .Include("~/Scripts/jquery-3.1.0.js")
                    );

            bundles.Add(new ScriptBundle(Constants.ApplicationBundle)
                .IncludeDirectory("~/Scripts/home", "*.js", true)
                );

            bundles.Add(new ScriptBundle(Constants.CriminalApiAngular)
                .IncludeDirectory("~/Scripts/criminalApi", "*.js", true)
                );
        }
    }
}