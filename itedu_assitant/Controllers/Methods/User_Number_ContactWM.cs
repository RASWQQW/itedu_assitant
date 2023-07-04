using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.forsave.Methods;
using itedu_assitant.Model.Base;
using itedu_assitant.Model.ForvView;

namespace itedu_assitant.Controllers.Methods
{
    public class User_Number_ContactWM
    {

        public dbcontext _context;
        public HttpContext _qcontent;
        public User_Number_ContactWM(dbcontext context, HttpContext qcontent)
        {
            _context = context;
            _qcontent = qcontent;
        }
        // [OPTIONAL]
        public void NumberSaveManager(List<string> cnumbers, 
                                      List<string> numbers, 
                                      Groups gr = null,
                                      List<string> admins = null) // it saves contact in base as contact
        {
            for (int i = 0; i < cnumbers.Count(); i++)
            {
                var userexist = _context.contusers.FirstOrDefault(w => w.userWhatsappId == cnumbers[i]);
                if (userexist == null)
                {
                    _context.Add(new Users
                    {
                        userName = new GetUserName().GetResponse(numbers[i]).__GetUserName() ?? "None", // Here goes getting User if its not exists in base gets from Almat aga base
                        userWhatsappId = cnumbers[i], // whatsapp id number
                        userNumber = numbers[i], // mere number
                        userGroup = gr,
                        userStatus = admins != null ? admins.Contains(cnumbers[i]) ? "admin" : "student" : "student"
                    });
                }
                else if (gr != null){
                    userexist.userGroup = gr;
                    _context.Update(userexist);
                }
            }
            _context.SaveChanges();
            //ContactSaveManager(gr); // it only give chance to save it when gets real user entry in base
        }

        //[OPTIONAL]
        public void ContactSaveManager(Groups gr = null) // it save contact in google contacts
        {
            if(gr != null){
                new ContactClassManager().ContactGatherer(new BaseExec(_context), _qcontent).ExecuteCreatingContact(new ContactMembers() { admininc = true, groupId = gr.id});
            }
        }
    }
}
