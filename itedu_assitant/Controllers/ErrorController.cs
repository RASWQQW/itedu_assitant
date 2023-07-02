using itedu_assitant.Controllers.Methods;
using itedu_assitant.DB;
using itedu_assitant.forsave.Methods;
using Microsoft.AspNetCore.Mvc;

namespace itedu_assitant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController: ControllerBase
    {
        public ErrorController()
        {

        }

        [HttpGet("/[action]")]
        public IActionResult Authorize()
        {
            return Ok(Request.Query["errors"]);
        }

        [HttpGet("Check1")]
        public IActionResult Check2([FromQuery] string usernumber)
        {
            return Ok(new GetUserName().GetResponse(usernumber).__GetUserName());
        }

    }
}
