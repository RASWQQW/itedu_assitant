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

       public Middleware(RequestDelegate next)
       {
            _next = next;
       }

        public Task Invoke(HttpContext context)
        {
            Checker(context);
            //Logger(context.Request.Path.Value, new List<object> { context.Request.Query });

            return _next(context);
        }

        public static async void Checker(HttpContext context)
        {
            var userip = context.Connection.RemoteIpAddress;
            Debug.WriteLine("Is Context RouteValues" + context.Request.RouteValues);
            Debug.WriteLine("Is Context Path" + context.Request.Path.ToString());
            Debug.WriteLine("Is Context Body" + context.Request.Body);
            Debug.WriteLine("Is Context From" + context.Request.GetDisplayUrl());
            Debug.WriteLine("Is Context IpAddress" + userip);
            if (context.Request.RouteValues.ContainsKey("authorize"))
            {
                context.Request.Query.TryGetValue("state", out StringValues statekey);
                SecretGenerator secgen = new SecretGenerator();
                if(statekey == secgen.GetSecret(userip.ToString()))
                {
                    var newcode = secgen.Generate(state: true);
                    secgen.SaveSecret(newcode, userip.ToString());
                }
                else
                    context.Response.Redirect("Error/Authorize?cause=This action is forbiden");
            }
            // "Your catched exchange code is wronng, please try again";
        }

        public void Logger(string endpoint, List<object> args)
        {
            // here goes little log about requests on base or any local base
            Debug.WriteLine("Is Context RouteValues" + endpoint);
            for(int val = 0; val < args.Count(); val++)
            {
                Debug.WriteLine($"Args val {args[val]}");
            }
        }
    }
}
