using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigManager.Core.Helpers
{
    public static class PathHelper
    {
        public const string ThemePath = "/";//"/App_Plugins/Articulate/Themes";
        public const string VirtualThemePath = "~" + ThemePath;
        private const string VirtualThemePathToken = "~" + ThemePath + "/{0}/";
        private const string VirtualThemeViewPathToken = "~" + ThemePath + "/{0}/Views/{1}.cshtml";
        private const string VirtualThemePartialViewPathToken = "~" + ThemePath + "/{0}/Views/Partials/{1}.cshtml";

        //public static string GetThemePath(IMasterModel model)
        //{
        //    if (model.Theme.IsNullOrWhiteSpace())
        //    {
        //        throw new InvalidOperationException("No theme has been set for this Articulate root, republish the root with a selected theme");
        //    }
        //    return string.Format(VirtualThemePathToken, model.Theme);
        //}

        //public static string GetThemeViewPath(IMasterModel model, string viewName)
        //{
        //    if (model.Theme.IsNullOrWhiteSpace())
        //    {
        //        throw new InvalidOperationException("No theme has been set for this Articulate root, republish the root with a selected theme");
        //    }
        //    return string.Format(VirtualThemeViewPathToken, model.Theme, viewName);
        //}

        //public static string GetThemePartialViewPath(IMasterModel model, string viewName)
        //{
        //    if (model.Theme.IsNullOrWhiteSpace())
        //    {
        //        throw new InvalidOperationException("No theme has been set for this Articulate root, republish the root with a selected theme");
        //    }
        //    return string.Format(VirtualThemePartialViewPathToken, model.Theme, viewName);
        //}

        /// <summary>
        /// Get the full domain of the current page
        /// </summary>
        public static string GetDomain(Uri requestUrl)
        {
            return requestUrl.Scheme +
                System.Uri.SchemeDelimiter +
                requestUrl.Authority;
        }

        public static string GetPath(this Uri url)
        {
            return $"{url.Scheme}{Uri.SchemeDelimiter}{url.Authority}{url.AbsolutePath}";
        }
    }
}
