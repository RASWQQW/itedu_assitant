using Aspose.Email.Clients.Exchange.WebService.Schema_2016;
using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.Model.ForvView;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace itedu_assitant.Controllers
{


    [ApiController]
    [Route("spec/[controller]")]
    public class SpecificsController : ControllerBase
    {

        public readonly BaseExec baseExec;
        public readonly AuthCodeGetting allocExec;
        public readonly StatExec statExec;
        public readonly CreateContact ismainCreator;

        public readonly dbcontext _context;
        public static Dictionary<string, object> UserDatas;
        SpecificsController(dbcontext context_s)
        {


            _context = context_s;
            UserDatas = new Dictionary<string, object> { { "userIp", HttpContext.Connection.RemoteIpAddress } };

            baseExec = new BaseExec(_context);
            statExec = new StatExec(UserDatas);
            ismainCreator = new CreateContact(baseExec, statExec);
            allocExec = new AuthCodeGetting(ismainCreator);

        }

        [HttpPost("/create_g_contact")]
        public IActionResult GetCodestring([FromForm] ContactMembers contactMembers)
        {
            if (contactMembers != null && Request.Method == "POST")
            {
                // here must be go little selenium operation tho
                // this code is mainly to little code and send for bringing it to authorize such it can go further
                var execution = allocExec.ExecuteCreatingContact(contactMembers);
                if (!execution.Item1)
                    return Ok($"You have to log in, log in is {execution.Item2}");
            }
            return Ok("You've must to give post argument values");
        }

        [HttpGet("/authorize")]
        public IActionResult Reg([FromQuery(Name = "state")] string? state = null, [FromQuery(Name = "code")] string? code = null, [FromQuery(Name = "scope")] string? scope = null)
        {
            Debug.WriteLine(code);
            // Here obviously mus to be place with userDatas to get >
            // >> It
            ismainCreator.SetInBaseAToken(code);
            return Ok(allocExec.ExecuteCreatingContact(byQueue: true).Item2);

        }
    }
}
