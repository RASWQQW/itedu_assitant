using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace itedu_assitant.Controllers.Methods
{
    public class Middleware
    {

        // here goes mere statistic gatherer and password check algo

       public readonly RequestDelegate _next;
       public ILogger _logger;
       
       public Middleware(RequestDelegate next, ILogger<Middleware> logger)
       {
            _logger = logger;
            _next = next;
       }

        public Task Invoke(HttpContext context)
        {

            //Logger(context.Request.Path.Value, new List<object> { context.Request.Query });

            Checker(context);
            //Logger(context);

            return _next(context);
        }

        public static async void Checker(HttpContext context)
        {
            var userip = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            //var ip2 = context.Request.HttpContext.GetServerVariable("REMOTE_ADDR").ToString();
            var ip3 = context.Request.Host.Value.ToString();
            Debug.WriteLine("Is Context RouteValues" + context.Request.RouteValues);
            Debug.WriteLine("Is Context Path" + context.Request.Path.Value.ToString());
            Debug.WriteLine("Is Context Body" + context.Request.Body.ToString());
            Debug.WriteLine("Is Context From" + context.Request.GetDisplayUrl());
            Debug.WriteLine("Is Context IpAddress" + userip);
            // it is more about now to check
            
            
            SecretGenerator secgen = new SecretGenerator();
            if (context.Request.Path.Value.Contains("authorize")) //
            {
                context.Request.Query.TryGetValue("state", out StringValues statekey);
                if (statekey == secgen.GetSecretRedis(userip))
                {
                    secgen.SetSecretRedis(userip);
                }
                else
                    context.Response.Redirect("Error/Authorize?error=This action is forbiden");
            }

            //For the first check and for giving ip address of user
            secgen.SetNewUser(userip);
            
            // "Your catched exchange code is wronng, please try again";
        }

        public void Logger(HttpContext content)
        {
            var presentReq = content.Request;
            string loggertext = $"\n<-- endpoint - {presentReq.Path.Value};\n\tRequest values(action, controller) - {presentReq.RouteValues}; \n\tDate - {DateTime.Now.ToString("dd/MM/yy hh:mm")}";
            // here goes little log about requests on base or any local base
            _logger.LogInformation(loggertext, DateTime.UtcNow.ToLongTimeString());

            string path = Directory.GetCurrentDirectory().ToString() + "/" + string.Join("/", new string[] { "Log", "logger.txt" });
            using (var logfile = new StreamWriter(path, true))
                logfile.Write(loggertext);
        }
    }
}
