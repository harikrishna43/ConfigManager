﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;
using Umbraco.Core;

namespace ConfigManager.Core.Controllers
{
    internal static class UrlHelperExtensions
    {
        internal static string GetTreePathFromFilePath(this UrlHelper urlHelper, string virtualPath, string basePath = "")
        {
            //This reuses the Logic from umbraco.cms.helpers.DeepLink class
            //to convert a filepath to a tree syncing path string. 

            //removes the basepath from the path 
            //and normalises paths - / is used consistently between trees and editors
            basePath = basePath.TrimStart("~");
            virtualPath = virtualPath.TrimStart("~");
            virtualPath = virtualPath.Substring(basePath.Length);
            virtualPath = virtualPath.Replace('\\', '/');

            //-1 is the default root id for trees
            var sb = new StringBuilder("-1");

            //split the virtual path and iterate through it
            var pathPaths = virtualPath.Split('/');

            for (var p = 0; p < pathPaths.Length; p++)
            {
                var path = HttpUtility.UrlEncode(string.Join("/", pathPaths.Take(p + 1)));
                if (string.IsNullOrEmpty(path) == false)
                {
                    sb.Append(",");
                    sb.Append(path);
                }
            }
            return sb.ToString().TrimEnd(",");
        }
    }
}
