using itedu_assitant.DB;
using itedu_assitant.Model.Base;
using Microsoft.EntityFrameworkCore;

namespace itedu_assitant.forsave.Contact_is.Methods
{
    public class BaseExec
    {
        public BaseExec(dbcontext context)
        {
            _context = context;
        }

        public dbcontext _context;


        public IEnumerable<Users> GetUserById(string group_id)
        {
            // _context.contusergroups.FirstOrDefault(w => w.group_id == group_id).group_id
            return _context.contusers.Include(ws => ws.userGroup)
                .Where(w => w.userGroup.group_id == group_id).ToList();
        }

        public Access_token GetFromBase()
        {
            if (_context != null)
                return _context.access_tokens.LastOrDefault();
            else
                throw new Exception("_context is not implemented");
        }
        public void SetInBase(string at, string rt, int expires, List<string> scopes)
        {
            if (_context != null)
            {
                var iscode = _context.access_tokens.FirstOrDefault();
                if (iscode != null)
                {
                    iscode.access_token = at;
                    iscode.refresh_token = rt;
                    iscode.expires_in = expires;
                    iscode.scopes = scopes;
                    iscode.last_change = DateTime.Now.Date.ToUniversalTime();

                }
                else
                    _context.access_tokens.Add(
                        new Access_token { access_token = at, refresh_token = rt, expires_in = expires, scopes = scopes });
                _context.SaveChanges();
            }
            else
                throw new Exception("_context is not implemented");
        }
    }
}
