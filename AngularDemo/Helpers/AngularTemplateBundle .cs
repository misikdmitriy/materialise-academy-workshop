using System.Web.Optimization;

namespace AngularDemo.Helpers
{
    public class AngularTemplateBundle : Bundle
    {
        public AngularTemplateBundle(string moduleName, string virtualPath)
            : base(virtualPath, new[] { new AngularTemplateTransform(moduleName) })
        {
        }
    }
}