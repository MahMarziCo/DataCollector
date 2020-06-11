using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using System.Web.Optimization;

namespace DataCollector
{
    public class BundleConfig
    {
        public class FileVersionBundleTransform : IBundleTransform
        {
            public string _Version { get; set; }
            public FileVersionBundleTransform(string version)
            {
                _Version = version;
            }
            public void Process(BundleContext context, BundleResponse response)
            {
                foreach (var file in response.Files)
                {
                    using (FileStream fs = File.OpenRead(HostingEnvironment.MapPath(file.IncludedVirtualPath)))
                    {

                        //encode file hash as a query string param

                        file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=", _Version);
                    }
                }
            }
        }
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            var appVersionTransform = new FileVersionBundleTransform(version);

            bundles.Add(new ScriptBundle("~/bundle/Script/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/Script/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/Script/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/Style/css").Include(
                     "~/Content/bootstrap.css",
                      "~/Content/bootstrap-rtl-min.css"));


            bundles.Add(new StyleBundle("~/bundles/Style/JalliDate").Include(
                     //"~/Content/JalaliDate/main.css",
                     "~/Content/JalaliDate/jspc-royal-blue.css"
                     ));


            var mahStyle = new StyleBundle("~/bundles/Style/Mah").Include(
                     "~/Content/Mah/Layout.css"
                     );

            mahStyle.Transforms.Add(appVersionTransform);

            bundles.Add(mahStyle);

            bundles.Add(new StyleBundle("~/bundles/Style/Kendo").Include(
                       "~/Content/Kendo/kendo.common.css",
                       "~/Content/Kendo/kendo.mobile.all.css",
                       "~/Content/Kendo/kendo.flat.css",
                       "~/Content/Kendo/kendo.rtl.css"
                     ));

            bundles.Add(new ScriptBundle("~/bundle/Script/Kendo").Include(
                       "~/Scripts/Kendo/jszip.min.js",
                       "~/Scripts/Kendo/kendo.all.js",
                       "~/Scripts/Kendo/kendo.aspnetmvc.js",
                       "~/Scripts/kendo.modernizr.custom.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundle/Script/JalliDate").Include(
                       "~/Scripts/JalaliDate/js-persian-cal.min.js"
                       ));

            var mahScripts = new ScriptBundle("~/bundle/Script/Mah").Include(
                       "~/Scripts/mah/Layout.js"
                      );

            mahScripts.Transforms.Add(appVersionTransform);
            bundles.Add(mahScripts);

            var symbolScript = new ScriptBundle("~/bundle/Script/Symbol").Include(
                       "~/Scripts/mah/Symbol.js"
                      );

            symbolScript.Transforms.Add(appVersionTransform);
            bundles.Add(symbolScript);

        }
    }
}
