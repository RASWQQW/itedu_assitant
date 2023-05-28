using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        public HomeController()
        {
        }

        [HttpGet("/send_message/{chat_id?}")]
        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        public OkObjectResult Meeting(string chat_id = "0", [FromQuery(Name = "chat_id")] string query_chat_id = "None")
        {
            if (Request.Method == "GET")
            {
                if (chat_id != "0")
                {
                    return Ok(new Dictionary<string, object> { { "user_id", chat_id } });
                }
                else if (Request.Query.ContainsKey("chat_id"))
                {
                    var current_user_id = Request.Query.Select(val => val.Key == "chat_id");
                    return Ok(new Dictionary<string, object> { { "user_id", current_user_id } });
                }
            }
            else if (Request.Method == "POST")
            {
                if (Request.HasJsonContentType())
                {
                    var rqeuest_vals = Request.ReadFromJsonAsync<Dictionary<string, object>>();
                }
            }
            return Ok("Bad request");
        }


        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        [HttpPost("api/[controller]/[action]/create_croup/")]
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
        [HttpGet("api/[controller]/[action]/{san?}")]
        public OkObjectResult Check(int san)
        {
            return Ok($"Alma {san}");
        }

        //[HttpGet(Name = "Check2")]
        [HttpGet("api/[controller]/[action]/{alma?}")]
        public OkObjectResult PPPCHECK(int alma)
        {
            return Ok($"There is no relevant response {alma * 2}");
        }

    }
}