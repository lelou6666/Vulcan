<%@ WebHandler Language="C#" Class="ektronCss" %>

using System;
using System.Web;
using System.Web.Configuration;
using System.Text;
using System.Globalization;
using System.IO;
using Microsoft.Security.Application;

public class ektronCss : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        
        string idParam = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["id"]);
        string[] ids = idParam.Split(new char[]{' '});
        bool isDebugEnabled = HttpContext.Current.IsDebuggingEnabled;
        bool canUseClientCache = true;
        CultureInfo provider = CultureInfo.InvariantCulture;
        DateTime lastUpdate = String.IsNullOrEmpty(context.Request.Headers.Get("If-Modified-Since")) ? DateTime.MinValue : DateTime.ParseExact(context.Request.Headers.Get("If-Modified-Since"), "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", provider);
        
        foreach (string id in ids)
        {
            //set cache keys
            Uri fileCacheKey = Ektron.Cms.API.RegisterUtilities.GetFileCacheKey(id);
            Uri pathCacheKey = Ektron.Cms.API.RegisterUtilities.GetPathCacheKey(id);
            Uri dateTimeCacheKey = Ektron.Cms.API.RegisterUtilities.GetDateTimeCacheKey(id);

            if (true) //cache is invalidated with modification to css file. don't need to check if it exists.      File.Exists(HttpContext.Current.Server.MapPath((string)HttpRuntime.Cache.Get(pathCacheKey.AbsoluteUri))))
            {
                //retrieve date time at which js was cached
                DateTime cacheDate;
                if (!DateTime.TryParse((string)(HttpRuntime.Cache.Get(dateTimeCacheKey.AbsoluteUri) ?? String.Empty), out cacheDate))
                    canUseClientCache = false;

                //if cache date is later than last update, invalidate client cache
                if (canUseClientCache)
                {
                    canUseClientCache = cacheDate.CompareTo(lastUpdate) > 0 ? false : true;
                }

                //add debug information if necessary
                if (isDebugEnabled)
                {
                    context.Response.Write("/* ############################################################# */" + Environment.NewLine);
                    context.Response.Write("/* ektron registered stylesheet */" + Environment.NewLine);
                    context.Response.Write("/* id: " + AntiXss.UrlEncode(id) + " */" + Environment.NewLine);
                    context.Response.Write("/* path: " + (string)HttpRuntime.Cache.Get(pathCacheKey.AbsoluteUri) + Environment.NewLine);
                    context.Response.Write("/* ############################################################# */");
                    context.Response.Write(Environment.NewLine + Environment.NewLine);
                }

                //add css
                context.Response.Write((string)HttpRuntime.Cache.Get(fileCacheKey.AbsoluteUri));
                context.Response.Write(Environment.NewLine + Environment.NewLine);
            } 
            else 
            {
                //file does not exist at specified path - do not allow use of client cache
                canUseClientCache = false;

                if (isDebugEnabled)
                {
                    //output error message even if debug is true
                    context.Response.Write("/* ############################################################# */" + Environment.NewLine);
                    context.Response.Write("/* ektron registered stylesheet: css file not found */" + Environment.NewLine);
                    context.Response.Write("/* id: " + AntiXss.UrlEncode(id) + " */" + Environment.NewLine);
                    context.Response.Write("/* path: " + (string)HttpRuntime.Cache.Get(pathCacheKey.AbsoluteUri) + Environment.NewLine);
                    context.Response.Write("/* ############################################################# */");
                    context.Response.Write(Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    //debug is true - output newline only
                    context.Response.Write(Environment.NewLine);
                }
                
                //invalidate cache
                HttpRuntime.Cache.Remove(pathCacheKey.AbsoluteUri);
            }           
        }
        
        //set header info
        context.Response.ContentType = "text/css";
        context.Response.ContentEncoding = Encoding.UTF8;
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.Now.AddDays(365));
        context.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0, 0));
        context.Response.Cache.SetLastModified(DateTime.Now);

        //if using client cache, set status code and suppress content
        context.Response.StatusCode = canUseClientCache ? 304 : 200;
        context.Response.SuppressContent = canUseClientCache ? true : false;
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}