using itedu_assitant.Controllers;
using itedu_assitant.DB;
using itedu_assitant.Model.Base;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace itedu_assitant.forsave.Methods
{
    public class CheckActive
    {

        static string idInstance = Wh_Instance.IdInstance;
        static string ApiToken = Wh_Instance.ApiToken;
        public Wh_Instance mainIns;
        static dbcontext _context;
            
        public CheckActive(Wh_Instance isclass, dbcontext? dbcontext = null)
        {
            mainIns = isclass;
            _context = dbcontext;
        }

        public Dictionary<string, object> GetInstanceStatus()
        {
            string isrequest = $"https://api.green-api.com/waInstance{idInstance}/getStateInstance/{ApiToken}";
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(mainIns.RequestSender(Link: isrequest, qtype: "get"));
        }
        public Dictionary<string, object> GetActiveNumberFromBase()
        {
            var currentActive = _context.CurrentActive.Include(w => w.IsActive).First();
            return new Dictionary<string, object> { {"activeId", currentActive.IsActive.id }, { "number", currentActive.IsActive.phoneNumber } };
        }
        public void SetNewNumber()
        {
            // First step removing a number from instance if here is already number
            if ((string)this.GetInstanceStatus()["stateInstance"] == "authorized")
            {
                string logoutLink = $"https://api.green-api.com/waInstance{idInstance}/logout/{ApiToken}";
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(mainIns.RequestSender(Link: logoutLink, qtype: "get"));
                if ((bool)result["isLogout"] != true)
                    throw new Exception("Here is happend not fully logout");
            }

            // Next is if there qr scanned there would be current number saved in base as active
            // There must be go cycle in around 10 mins to check and implement

            var val = new BackgroundTaskManage(this);
            var newtokengen = new CancellationTokenSource();
            val.StartAsync(newtokengen.Token);

            //Lastly to the end is sending Qr
            new HomeController().Check();

        }
        public void BaseWriter(object? states)
        {
            bool Check(){
                if (states is CancellationToken)
                    return ((CancellationToken)states).IsCancellationRequested;
                return false;
            }

            int packedTime = 0;
            while (TimeSpan.FromMinutes(15).Seconds <= packedTime) // user custom check
            {  
                string linkIs = $"https://api.green-api.com/wainstace{idInstance}/getSettings/{ApiToken}";
                Dictionary<string, object> response = JsonConvert.DeserializeObject<Dictionary<string, object>>(mainIns.RequestSender(linkIs, "get"));
                string usernumber = new Regex(@"^\d*@c.us$").Match((string)response["wid"]).Value.ToString();

                if(usernumber != null)
                {
                    // Lastly here goes saving on base
                    UserNumbers Query() => _context.UserNumbers.FirstOrDefault(w => w.phoneNumber == usernumber);
                    if (Query() == null)
                        _context.UserNumbers.Add(new UserNumbers { phoneNumber = usernumber });

                    Active currentActive = _context.CurrentActive.First();
                    currentActive.LastInsert = DateTime.Now.Date.ToUniversalTime();
                    currentActive.IsActive = Query();
                    currentActive.changeAmount++;
                    _context.SaveChanges();
                }

                // Check that here is all ok
                if (Check()) break;
                // sleep time to create little delay
                Thread.Sleep(25000);
                // add time to while validation check
                packedTime = packedTime + TimeSpan.FromSeconds(25).Seconds;                

            }
        }

    }
}
