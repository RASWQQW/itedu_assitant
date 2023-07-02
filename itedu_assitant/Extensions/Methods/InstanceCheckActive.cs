using itedu_assitant.Controllers;
using itedu_assitant.DB;
using itedu_assitant.Model.Base;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace itedu_assitant.forsave.Methods
{
    public class InstanceCheckActive
    {
        static string idInstance = Wh_Instance.IdInstance;
        static string ApiToken = Wh_Instance.ApiToken;
        public Wh_Instance mainIns;
        static dbcontext _context;
        public InstanceCheckActive(Wh_Instance isclass, dbcontext? dbcontext = null)
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
            var currentActive = _context.contcurrentactive.Include(w => w.IsActive).FirstOrDefault();
            if(currentActive != null)
                return new Dictionary<string, object> { {"activeId", currentActive?.IsActive.id }, { "number", currentActive?.IsActive.phoneNumber } };
            return new Dictionary<string, object> { { "active_number", "None" } };
        }
        public Tuple<string, string> SetNewNumber()
        {
            // First step removing a number from instance if here is already number
            if ((string)GetInstanceStatus()["stateInstance"] == "authorized")
            {
                string logoutLink = $"https://api.green-api.com/waInstance{idInstance}/logout/{ApiToken}";
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(mainIns.RequestSender(Link: logoutLink, qtype: "get"));
                if ((bool)result["isLogout"] != true)
                    throw new Exception("Here is happend not fully logout");
            }

            // Next is if there qr scanned there would be current number saved in base as active
            // There must be go cycle in around 10 mins to check and implement

            var val = new BackgroundTaskManage().SetArgs(is_method: this.BaseWriter);
            var newtokengen = new CancellationTokenSource();
            val.StartAsync(newtokengen.Token);

            //Lastly to the end is sending Qr
            //new HomeController().Check();

            // Here likely goes just Redirect but it is enugh to catch
            //HomeController.Check();
            return new Tuple<string, string>("Check", "Home");

        }
        public void BaseWriter(object? states)
        {
            bool Check(){
                if (states is CancellationToken)
                    return ((CancellationToken)states).IsCancellationRequested;
                return false;
            }

            int packedTime = 0;
            string usernumber = null;
            string linkIs = $"https://api.green-api.com/waInstance{idInstance}/getSettings/{ApiToken}";
            while (TimeSpan.FromMinutes(15).Seconds <= packedTime) // user custom check
            {  
                // TODO here is one thing that api request can not wok properly
                var isRequestResult = mainIns.RequestSender(linkIs, "get");
                Dictionary<string, object> response = JsonConvert.DeserializeObject<Dictionary<string, object>>(isRequestResult);
                string isnumberid = (string)response["wid"];
                if (new Regex(@"^\d*(@c\.us|@g\.us)$").Match(isnumberid).Success)
                    usernumber = new Regex(@"^\d+").Match(isnumberid).Value;

                if (usernumber != null)
                {
                    // Lastly here goes saving on base
                    ManagerNumbers Query() => _context.contmanagernumbers.FirstOrDefaultAsync(w => w.phoneNumber == usernumber).Result;
                    if (Query() == null)
                        _context.contmanagernumbers.Add(new ManagerNumbers { phoneNumber = usernumber });

                    Active currentActive = _context.contcurrentactive.FirstOrDefaultAsync().Result;
                    if(currentActive != null)
                    {
                        currentActive.LastInsert = DateTime.Now.Date.ToUniversalTime();
                        currentActive.IsActive = Query();
                        currentActive.changeAmount++;
                    }
                    else
                        _context.contcurrentactive.Add(new Active { IsActive = Query() });

                    _context.SaveChanges();

                }

                // Check that here is all ok
                if (Check()) break;
                // sleep time to create little delay
                Thread.Sleep(25000);
                // add time to while validation check
                packedTime = packedTime + TimeSpan.FromSeconds(10).Seconds;                

            }
        }

    }
}
