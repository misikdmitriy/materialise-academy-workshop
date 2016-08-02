using System.IO;
using System.Text;
using System.Web;
using System.Web.Optimization;

namespace AngularDemo.Helpers
{
    public class AngularTemplateTransform : IBundleTransform
    {
        private readonly string _moduleName;
        public AngularTemplateTransform(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine("(function () { ");
            scriptBuilder.AppendLine("  \"use strict\";");
            scriptBuilder.AppendLine();

            scriptBuilder.AppendFormat("    angular.module('{0}').run(['$templateCache', ", _moduleName);
            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine("        function ($templateCache) {");

            foreach (var file in response.Files)
            {
                var key = file.VirtualFile.VirtualPath;
                var content = string.Empty;
                using (var fileStream = file.VirtualFile.Open())
                {
                    content = new StreamReader(fileStream).ReadToEnd();
                }
                content = HttpUtility.JavaScriptStringEncode(content);

                scriptBuilder.AppendFormat(@"           $templateCache.put('{0}','{1}');", key, content);
                scriptBuilder.AppendLine();
            }

            scriptBuilder.AppendLine("      }");
            scriptBuilder.AppendLine("  ]);");
            scriptBuilder.AppendLine("})();");

            response.Files = new BundleFile[] { };
            response.Content = scriptBuilder.ToString();
            response.ContentType = "text/javascript";
        }
    }
}