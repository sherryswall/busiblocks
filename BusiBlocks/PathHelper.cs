using System;
using System.IO;
using System.Web;

namespace BusiBlocks
{
    /// <summary>
    /// Static class with path related methods. Can be useful to combine url or to retrive the server root directory.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// If the path is absolute is return as is, otherwise is combined with AppDomain.CurrentDomain.SetupInformation.ApplicationBase
        /// The path are always server relative path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LocateServerPath(string path)
        {
            if (Path.IsPathRooted(path) == false)
                path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);

            return path;
        }


        public static string CombineUrl(string baseUrl, string relativeUrl)
        {
            if (relativeUrl.Length == 0 || relativeUrl[0] != '/')
                relativeUrl = '/' + relativeUrl;

            if (baseUrl.Length > 0 && baseUrl[baseUrl.Length - 1] == '/')
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            return baseUrl + relativeUrl;

            //Uri uriBase = new Uri(baseUrl);
            //Uri uriRelative = new Uri(relativeUrl, UriKind.Relative);
            //Uri uri = new Uri(uriBase, uriRelative);

            //return uri.AbsoluteUri;
        }

        /// <summary>
        /// Get the web site application root path. Combine the request.Url.GetLeftPart(UriPartial.Authority) with request.ApplicationPath
        /// </summary>
        /// <returns></returns>
        public static string GetWebAppUrl()
        {
            HttpRequest request = HttpContext.Current.Request;

            return CombineUrl(request.Url.GetLeftPart(UriPartial.Authority), request.ApplicationPath);
        }
    }
}