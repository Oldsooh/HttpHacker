using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ReverseProxy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = HttpContext;
            // Create the web request to communicate with the back-end site
            string remoteUrl = ConfigurationManager.AppSettings["ProxyUrl"] +
                    context.Request.Path;
            if (context.Request.QueryString.ToString() != "")
                remoteUrl += "?" + context.Request.QueryString;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteUrl);
            request.AllowAutoRedirect = false;
            request.Method = context.Request.HttpMethod;
            request.ContentType = context.Request.ContentType;
            request.UserAgent = context.Request.UserAgent;
            //string basicPwd = ConfigurationManager.AppSettings.Get("basicPwd");
            //request.Credentials = basicPwd == null ?
            //    CredentialCache.DefaultCredentials :
            //    new NetworkCredential(context.User.Identity.Name, basicPwd);
            //request.PreAuthenticate = true;
            // The Remote-User header is non-ideal; included for compatibility
            request.Headers["Remote-User"] = context.User.Identity.Name;
            foreach (String each in context.Request.Headers)
                if (!WebHeaderCollection.IsRestricted(each) && each != "Remote-User")
                    request.Headers.Add(each, context.Request.Headers.Get(each));
            if (context.Request.HttpMethod == "POST")
            {
                Stream outputStream = request.GetRequestStream();
                CopyStream(context.Request.InputStream, outputStream);
                outputStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return new HttpWebResponseResult(response);
        }

        public void CopyStream(Stream input, Stream output)
        {
            Byte[] buffer = new byte[1024];
            int bytes = 0;
            while ((bytes = input.Read(buffer, 0, 1024)) > 0)
                output.Write(buffer, 0, bytes);
        }

    }
}