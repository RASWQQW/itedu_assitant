using itedu_assitant.DB;
using itedu_assitant.forsave.Methods;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace itedu_assitant.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        public readonly dbcontext _context;
        public readonly CheckActive checkActive;
        public readonly Wh_Instance Container;

        public AdminController(dbcontext Context)
        {
            _context = Context;
            Container = Wh_Instance.Create(context: _context);
            checkActive = new CheckActive(dbcontext: _context, isclass: Container);
        }

        //[HttpGet(Name="Check")]
        [HttpGet("/get_qr")]
        public IActionResult Check()
        {
            try
            {
                var res = Container.GetQr();
                byte[] readqr = System.IO.File.ReadAllBytesAsync(res).Result;
                return File(readqr, "image/jpeg");
            }
            catch (Exception ex)
            {
                return base.Content($"<h1>There happened trouble{ex}<h1>", "text/html");
            }
        }

        [HttpGet("/authorizing")]
        [HttpGet("/set_number")]
        public IActionResult SetNumber()
        {
            var isres = checkActive.SetNewNumber();
            return RedirectToAction(isres.Item1, isres.Item2);
        }

        [HttpGet("/get_present_account/{userNumber?}")]
        public IActionResult GetCurrentUser(bool userNumber = false)
        {
            var is_active = checkActive.GetInstanceStatus();
            Dictionary<string, object> phonenumber = null;

            if (userNumber && is_active["stateInstance"].ToString() == "authorized")
                phonenumber = checkActive.GetActiveNumberFromBase();

            return Ok(new Dictionary<string, object>() { { "status", is_active }, { "phoneNumber", phonenumber } });
        }
    }
}
