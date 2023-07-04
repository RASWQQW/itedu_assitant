using itedu_assitant.Controllers.Methods;
using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.forsave.Methods;
using itedu_assitant.Model.Base;
using itedu_assitant.Model.ForvView;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Cryptography;

namespace itedu_assitant.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {

        public dbcontext _context { get; set; }
        public Wh_Instance Container { get; set; }
        public GroupController(dbcontext context)
        {
            _context = context;
            Container = Wh_Instance.Create(context);
        }

        [ProducesResponseType(200, Type = typeof(OkObjectResult))]
        [HttpPost("/create_group")]
        public OkObjectResult ForCreateG([FromForm] Check val)
        {
            if (Request.Method == "POST")
            {
                // here goes full save of numbers in base and in google contact
                var unitednumbers = new List<string>();
                unitednumbers.AddRange(val.users);
                unitednumbers.AddRange(val.admins);

                unitednumbers = unitednumbers.ToHashSet().ToList();
                var propnumbs = Container.MakeProperNumber<string>(unitednumbers);
                
                var newGroup = new Groups() { name = val.groupname ?? $"new default Students Group {_context.contusergroups.FirstOrDefault().id + 1}"};
                new User_Number_ContactWM(_context, Request.HttpContext).NumberSaveManager(unitednumbers, propnumbs, newGroup, val.admins);

                string ispath = "None";
                var is_group_image = val.group_image;
                if (is_group_image != null)
                {
                    ispath = Path.Combine(Directory.GetCurrentDirectory(), "Extensions", "Files", "userimages", "groupimage.png");

                    void createImage()
                    {
                        using (Stream isstream = new FileStream(ispath, FileMode.OpenOrCreate))
                        {
                            is_group_image.CopyToAsync(isstream);
                            isstream.Close();
                        };
                        if (!System.IO.File.Exists(ispath))
                        {
                            Thread.Sleep(500);
                            createImage();
                        }
                    };
                    createImage();
                }

                var groupVal = Container.CreateGroup(
                    properNumbers: propnumbs,
                    newGroup: newGroup,
                    admins: val.admins,
                    Avatar: ispath);

                // parts of using new created group by user values
                if (groupVal.GetType() == typeof(Dictionary<,>))
                {
                    if (val.Hello_Message != null)
                    {
                        string hmessage = val.Hello_Message;
                        if (hmessage.Trim() == "default")
                            hmessage = $"Our invite link - {groupVal["WhatsAppgroupLink"]}";
                        Container.Send_Message(groupVal["WhatsAppgroupId"], hmessage);
                    }
                }
                return Ok(groupVal);
            }
            return Ok("There is no relevant response");
        }

        [HttpPost("addMembers")]
        public OkObjectResult AddMembers([FromBody] List<string> memberNumbers, string GroupWhId = null, int BaseGroupId = 0)
        {

            if (BaseGroupId != 0)
                GroupWhId = _context.contusergroups.FirstOrDefault(w => w.id == BaseGroupId).group_id;

            Groups curgr = _context.contusergroups.FirstOrDefault(val => val.group_id == GroupWhId);
            new User_Number_ContactWM(_context, Request.HttpContext).NumberSaveManager(memberNumbers, Container.MakeProperNumber(memberNumbers), curgr);
            
            Container.AddUsersToGroup(memberNumbers, GroupWhId);
            return Ok("Users succesfully added");
        }
    }
}
