using ConfigManager.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Editors;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Umbraco.Web.WebApi.Filters;

namespace ConfigManager.Core.Controllers
{
    [PluginController("ConfigEditorManager")]
    [UmbracoApplicationAuthorize(Constants.Applications.Settings)]
    public class ConfigEditorController : BackOfficeNotificationsController
    {
        public ConfigEditorController(IGlobalSettings globalSettings, IUmbracoContextAccessor umbracoContextAccessor, ISqlContext sqlContext, ServiceContext services, AppCaches appCaches, IProfilingLogger logger, IRuntimeState runtimeState, UmbracoHelper umbracoHelper) : base(globalSettings, umbracoContextAccessor, sqlContext, services, appCaches, logger, runtimeState, umbracoHelper)
        {
        }
        private readonly IFileSystem _themesFileSystem = new PhysicalFileSystem(PathHelper.VirtualThemePath);
        public CodeFileDisplay GetByPath(string virtualPath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(virtualPath));

            virtualPath = HttpUtility.UrlDecode(virtualPath);

            if (_themesFileSystem.FileExists(virtualPath))
            {
                return MapFromVirtualPath(virtualPath);
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
        private CodeFileDisplay MapFromVirtualPath(string virtualPath)
        {
            using (var reader = new StreamReader(_themesFileSystem.OpenFile(virtualPath)))
            {
                var display = new CodeFileDisplay
                {
                    Content = reader.ReadToEnd(),
                    FileType = Path.GetExtension(virtualPath),
                    Id = HttpUtility.UrlEncode(virtualPath),
                    Name = Path.GetFileName(virtualPath),
                    Path = Url.GetTreePathFromFilePath(virtualPath),
                    VirtualPath = NormalizeVirtualPath(virtualPath, PathHelper.VirtualThemePath)
                };
                display.FileType = Path.GetExtension(virtualPath).TrimStart('.');
                return display;
            }
        }
        private string NormalizeVirtualPath(string virtualPath, string systemDirectory)
        {
            if (virtualPath.IsNullOrWhiteSpace())
                return string.Empty;

            systemDirectory = systemDirectory.TrimStart("~");
            systemDirectory = systemDirectory.Replace('\\', '/');
            virtualPath = virtualPath.TrimStart("~");
            virtualPath = virtualPath.Replace('\\', '/');
            virtualPath = ClientDependency.Core.StringExtensions.ReplaceFirst(virtualPath, systemDirectory, string.Empty);

            return virtualPath;
        }

        public CodeFileDisplay PostSaveConfigFile(CodeFileDisplay themeFile)
        {
            if (themeFile == null) throw new ArgumentNullException("ConfigFile");

            if (ModelState.IsValid == false)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            switch (themeFile.FileType)
            {
                case "css":
                    CreateOrUpdateFile(".css", themeFile);
                    break;
                case "js":
                    CreateOrUpdateFile(".js", themeFile);
                    break;
                case "cshtml":
                    CreateOrUpdateFile(".cshtml", themeFile);
                    break;
                case "config":
                    CreateOrUpdateFile(".config", themeFile);
                    break;
                case "cs":
                    CreateOrUpdateFile(".cs", themeFile);
                    break;
                case "asax":
                    CreateOrUpdateFile(".asax", themeFile);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return MapFromVirtualPath(themeFile.VirtualPath);
        }

        private void CreateOrUpdateFile(string expectedExtension, CodeFileDisplay display)
        {
            display.VirtualPath = EnsureCorrectFileExtension(NormalizeVirtualPath(display.VirtualPath, PathHelper.VirtualThemePath), expectedExtension);
            display.Name = EnsureCorrectFileExtension(display.Name, expectedExtension);

            //if the name has changed we need to delete and re-create
            if (!Path.GetFileNameWithoutExtension(display.VirtualPath).InvariantEquals(Path.GetFileNameWithoutExtension(display.Name)))
            {
                //remove the original file
                _themesFileSystem.DeleteFile(display.VirtualPath);
                //now update the virtual path to be correct
                var parts = display.VirtualPath.Split('/');
                display.VirtualPath = string.Join("/", parts.Take(parts.Length - 1)).EnsureEndsWith('/') + display.Name;
            }

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(display.Content);

                writer.Flush();

                //create or overwrite it
                _themesFileSystem.AddFile(display.VirtualPath.TrimStart('/'), stream, true);
            }
        }

        private string EnsureCorrectFileExtension(string value, string extension)
        {
            if (value.EndsWith(extension) == false)
                value += extension;

            return value;
        }

    }
}
