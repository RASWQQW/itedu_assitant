using itedu_assitant.DB;
using itedu_assitant.forsave.Contact_is;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.Model.ForvView;

namespace itedu_assitant.Controllers.Methods
{
    public class ContactClassManager
    {
        public ContactClassManager() { }

        public AuthCodeGetting ContactGatherer(BaseExec basex, HttpContext context)
        {
            var check = basex; // it makes mere db instance to perform setting and getting of apitoken in CreateContact
            CreateContact contactct = new CreateContact(basex, context.Connection.RemoteIpAddress.ToString()); // It needs to get google object which operates base and creating opeations
            AuthCodeGetting runnerclass = new AuthCodeGetting(contactct); // And lastly this class wraps last consturcted class to call its Number() method to perform backgr tasks and save its contact values such it no loses and to make responses to covering method from top (Reg, ...)
            return runnerclass;
        }
    }
}
