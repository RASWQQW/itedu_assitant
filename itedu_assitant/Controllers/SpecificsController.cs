using Aspose.Email.Clients.Exchange.WebService.Schema_2016;
using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.Model.ForvView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using itedu_assitant.Controllers.Methods;

namespace itedu_assitant.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class SpecificsController : ControllerBase
    {

        public readonly BaseExec baseExec;
        public readonly StatExec statExec;
        public CreateContact ismainCreator;
        public AuthCodeGetting maincodegetting;

        public readonly dbcontext _context;
        public Dictionary<string, object> UserDatas = new Dictionary<string, object>();

        public SpecificsController(dbcontext context_s)
        {
            _context = context_s;
            baseExec = new BaseExec(_context);
            statExec = new StatExec(); // it now stands to just keep being opened as a rest of main present classes
        }

        [HttpPost("speccheck")]
        [HttpGet("speccheck")]
        public OkObjectResult Checking([FromForm] string? san = "12", [FromQuery] string? san2 = "12")
        {
            var isIp = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(Convert.ToInt32(Request.Form["san"]) * 2);
        }

        // it creates new AuthCodeGetting class when it each time revokes
        // and relatively it gives chance make that class with each specific user ip addresses
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void ClassMaker(HttpContext context){
            maincodegetting = new ContactClassManager().ContactGatherer(baseExec, HttpContext);
        }

        /// <summary>
        /// Creates Google Contacts by its name and contact phone number.
        /// </summary>
        [HttpPost("create_g_contact")]
        public IActionResult GetCodestring([FromBody] ContactMembers contactMembers)
        {
            if (contactMembers != null && Request.Method == "POST")
            {
                // here must be go little selenium operation tho
                // this code is mainly to little code and send for bringing it to authorize such it can go further
                ClassMaker(HttpContext);
                var execution = maincodegetting.ExecuteCreatingContact(contactMembers);
                if (!execution.Item1)
                    return Ok($"You have to log in, log in is {execution.Item2}");
            }
            return Ok("You've must to give post argument values");
        }

        [HttpGet("authorize")]
        public IActionResult Reg([FromQuery(Name = "state")] string? state = null, [FromQuery(Name = "code")] string? code = null, [FromQuery(Name = "scope")] string? scope = null)
        {
            Debug.WriteLine(code);
            // Here obviously mus to be place with userDatas to get >
            // >> It
            ClassMaker(HttpContext);
            ismainCreator.SetInBaseAToken(code);
            return Ok(maincodegetting.ExecuteCreatingContact(byQueue: true).Item2);

        }
    }
}
