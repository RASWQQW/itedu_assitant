using itedu_assitant.Controllers.Methods;
using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.forsave.Methods;
using itedu_assitant.Model.ForvView;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;

namespace itedu_assitant.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        public HomeController(dbcontext dbcontext = null, IEnumerable<EndpointDataSource> data_endpoints = null)
        {
            // Theses all secondry setting variables and gets vals after setting env and global vals
            _context = dbcontext;
            endpoints = data_endpoints;
            Container = Wh_Instance.Create(_context, data_endpoints);
            checkActive = new InstanceCheckActive(dbcontext: _context, isclass: Container);
        }

        static dbcontext _context;
        public readonly IEnumerable<EndpointDataSource> endpoints;
        public Wh_Instance Container;
        public InstanceCheckActive checkActive;


        [HttpGet("/checkgetroute")]
        public IActionResult Runner()
        {
            string val = "Apple |NApple2";
            val = val.Replace("|N", "\n");
            return Ok(Container.Send_Message(chat_id: "87789197489", val));
            //var islocalpart = Request.Host.Host;
            //Environment.SetEnvironmentVariable("HostServer2", islocalpart);
            //return Ok($"It is string - {Container.GetActionUrl("SetNumber")["route"]}");
        }

        [HttpPost("/send_message_list")]
        [ProducesResponseType(200, Type=typeof(OkObjectResult))]
        public OkObjectResult Message_ListSender([FromBody] List<SendMessage> messageList, string? commonMessage)
        {
            if(Request.Method == "POST")
            {
                List<string> responses = new List<string>();
                foreach (var message in messageList)
                {
                    var s_message = commonMessage ?? message.Message;
                    if (s_message == null || s_message == "string")
                        return Ok("Each User Message is required, Please check it validated");

                    var changedstr = s_message.Replace("{username}", message.UserName ?? new GetUserName().GetResponse(message.Chat_id).__GetUserName()).Replace("|N", "\n");
                    responses.Add(Container.Send_Message(message.Chat_id, changedstr));
                }
                return Ok(responses);
            }
            return Ok("You have query only post request there with proper varaibles");
        }

        [HttpGet("/send_message/{chat_id?}")]
        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        public OkObjectResult ChatSender(
                                [FromForm][FromBody]string? chat_id = "None", 
                                [FromQuery(Name = "chat_id")] string? query_chat_id = "None",
                                [FromQuery][FromForm] string? message = "User defauly message")
        {
            chat_id = chat_id == "None" ? query_chat_id : chat_id;
            // some checking meaninfull for knowing thing for sure
            Request.Query.TryGetValue("chat_id", out StringValues apple);
            apple.GetType();

            if (Request.Method == "GET")
                return Ok(Container.Send_Message(chat_id, message));
            //return Ok(new Dictionary<string, object> { { "user_id", chat_id } });

            else if (Request.Method == "POST") {
                if (Request.HasJsonContentType()) {
                    var requests_val = Request.ReadFromJsonAsync<Dictionary<string, object>>().Result;
                    var usermessage = requests_val["message"];
                    var to_chat_id = requests_val["chat_id"];
                    return Ok(Container.Send_Message(message: message, chat_id: chat_id));
                }
            }
            return Ok("Bad request");
        }
    }
}