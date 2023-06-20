using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is;
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
            Container = Wh_Instance.Create(endpoints: endpoints, context: _context);
            checkActive = new CheckActive(dbcontext: _context, isclass: Container);
        }

        static dbcontext _context;
        public readonly IEnumerable<EndpointDataSource> endpoints;
        public Wh_Instance Container;
        public CheckActive checkActive;


        [HttpGet("/checkgetroute")]
        public IActionResult Runner()
        {

            var islocalpart = Request.Host.Host;
            Environment.SetEnvironmentVariable("HostServer2", islocalpart);
            return Ok($"It is string - {Container.GetActionUrl("SetNumber")["route"]}");
        }

        [HttpGet("/send_message/{chat_id?}")]
        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        public OkObjectResult ChatSender(string? chat_id = "None", [FromQuery(Name = "chat_id")] string? query_chat_id = "None")
        {
            chat_id = chat_id == "None" ? query_chat_id : chat_id;

            // some checking meaninfull for knowing thing for sure
            Request.Query.TryGetValue("chat_id", out StringValues apple);
            apple.GetType();

            if (Request.Method == "GET")
                return Ok(Container.Send_Message(chat_id, "Hello My food Mate"));
            //return Ok(new Dictionary<string, object> { { "user_id", chat_id } });

            else if (Request.Method == "POST") {
                if (Request.HasJsonContentType()) {
                    var rqeuest_vals = Request.ReadFromJsonAsync<Dictionary<string, object>>();
                }
            }
            return Ok("Bad request");
        }

        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        [HttpPost("/create_croup")]
        public OkObjectResult ForCreateG([FromForm] Check val)
        {
            if (Request.Method == "POST")
            {
                string ispath = "None";
                var is_group_image = val.group_image;
                if(is_group_image != null) {
                    ispath = Path.Combine(Directory.GetCurrentDirectory(), "forsave", "Files", "userimages", "usergroupfile.png");

                    void createImage(){
                        using (Stream isstream = new FileStream(ispath, FileMode.OpenOrCreate)){
                            is_group_image.CopyToAsync(isstream);
                            if (!Directory.Exists(ispath)){
                                createImage();
                            }
                        };
                    };
                    createImage();
                }
                
                Container.CreateGroup(groupName: val.groupname, admins: val.admins, chatIds: val.users, Avatar: ispath);
            }
            return Ok("There is no relevant response");
        }

        
    }
}