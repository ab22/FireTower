using System;
using System.Text;
using System.Web;
using Nancy;
using log4net;

namespace FireTower.Presentation.Logging
{
    public class ExceptionFormatterLogger
    {
        readonly HttpContext _httpContext;

        public ExceptionFormatterLogger()
        {
            _httpContext = HttpContext.Current;
        }

        public void LogException(Exception exception, NancyContext context)
        {
            string userName = "User: " + context.CurrentUser.UserName;
            string remoteIp = "IP Address: " + GetIP(_httpContext);
            string pageUrl = "Page url: " + context.Request.Url;
            string userAgent = "Browser: " + _httpContext.Request.UserAgent;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("##### User Details #####");
            sb.AppendLine();
            sb.AppendLine(userName);
            sb.AppendLine(remoteIp);
            sb.AppendLine(pageUrl);
            sb.AppendLine(userAgent);
            sb.AppendLine();

            sb.AppendLine("##### Exception Details #####");
            ILog Log = LogManager.GetLogger(context.GetType());
            Log.Error(sb.ToString(), exception);
        }

        public string GetIP(HttpContext httpContext)
        {
            string ip =
                httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
    }
}