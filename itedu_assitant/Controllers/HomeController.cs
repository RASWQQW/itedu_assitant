using itedu_assitant.DB;
using itedu_assitant.forsave.Methods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace itedu_assitant.Controllers
{

    public class Check{
        [Required]
        public string groupname { get; set; }

        public List<string> admins { get; set; } = null;
        public IFormFile group_image { get; set; } = null;
    }

    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        public HomeController(dbcontext dbcontext = null, IEnumerable<EndpointDataSource> data_endpoints=null)
        {
            _context = dbcontext;
            endpoints = data_endpoints;
            Container = Wh_Instance.Create(endpoints: endpoints);
            checkActive = new CheckActive(dbcontext: _context, isclass: Container);

        }

        static dbcontext _context;
        public readonly IEnumerable<EndpointDataSource> endpoints;
        public Wh_Instance Container;
        public CheckActive checkActive;

        [HttpGet("/checkgetroute")]
        public IActionResult Runner()
        {
            return Ok($"It is string - {Container.GetActionUrl("SetNumber")}");
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

            else if (Request.Method == "POST"){
                if (Request.HasJsonContentType()){
                    var rqeuest_vals = Request.ReadFromJsonAsync<Dictionary<string, object>>();
                }
            }
            return Ok("Bad request");
        }


        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        [HttpPost("/create_croup")]
        public OkObjectResult ForCreateG(Check val, IFormFile group_avatar)
        {
            if (Request.Method == "POST")
            {
                if (Request.HasJsonContentType())
                {
                    var group_content = Request.ReadFromJsonAsync<Dictionary<string, object>>();
                }
            }
            return Ok("There is no relevant response");
        }

        //[HttpGet(Name="Check")]
        [HttpGet("/[action]/get_qr/{id?}")]
        public IActionResult Check(string? id = null)
        {
            Debug.WriteLine("Qr_get in", id);
            try
            {
                var res = Container.GetQr();
                //var html = $"<img href='{res}'>";
                //return base.Content(html, "text/html0");
                byte[] readqr = System.IO.File.ReadAllBytesAsync(res).Result;
                return File(readqr, "image/jpeg");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return base.Content($"<h1>There happened trouble{ex}<h1>", "text/html");
            }
        }

        [HttpGet("/get-c-whaccount/{userNumber?}")]
        public IActionResult GetCurrentUser(bool userNumber = false)
        {
            var is_active = checkActive.GetInstanceStatus();
            Dictionary<string, object> phonenumber = null;

            if (userNumber && is_active["stateInstance"].ToString() == "authorized")
                phonenumber = checkActive.GetActiveNumberFromBase();
            
            return Ok(new Dictionary<string, object>(){ {"status", is_active }, { "phoneNumber", phonenumber} });
        }

        [HttpGet("/set_number")]
        public void SetNumber(){
            checkActive.SetNewNumber();
        }
    }
}