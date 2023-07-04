using Google.Apis.Auth.OAuth2;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Util.Store;
using System.Diagnostics;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Http;
using Google.Apis.Auth.OAuth2.Responses;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using System.Net.Sockets;
using System.Security.Cryptography;
using itedu_assitant.DB;
using System.Net.Http.Headers;
using RestSharp;
using Microsoft.EntityFrameworkCore;
using itedu_assitant.Model.Base;
using itedu_assitant.forsave.Contact_is.Methods;
using itedu_assitant.forsave.Methods;

namespace itedu_assitant.forsave.Contact_is
{
    public class CreateContact
    {
        public CreateContact(BaseExec baseExec, string userIp)
        {
            _baseExec = baseExec;
            UserIp = userIp;
            // set secrets before run
            SetSecrets();
        }

        // is oversea values
        public BaseExec _baseExec;
        public StatExec _statExec;
        public string UserIp;

        // below all static inner values
        static GoogleCredential creds;
        static string appname;
        static PeopleServiceService peopleService;
        static string[] Scopes = { PeopleServiceService.Scope.Contacts, PeopleServiceService.Scope.UserinfoEmail, PeopleServiceService.Scope.UserinfoProfile, PeopleServiceService.Scope.ContactsReadonly};

        private string client_secret;
        private string client_id;
        public Tuple<bool, string> content;


        // here is main allocator, explicily main two overloads that one by group another one by only list name and number
        public void SetSecrets()
        {
            // to datas from current directory
            string ispath = Path.Combine(Directory.GetCurrentDirectory(), "Extensions", "Contact_is", "Files");
            string datas = File.ReadAllText(Path.Combine(ispath, "authdata.json"));

            var isconverted = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(datas);
            client_id = (string)isconverted["web"]["client_id"];
            client_secret = (string)isconverted["web"]["client_secret"];
        }

        public void SetInBaseAToken(string code)
        {
            //authorization_code
            var form = new Dictionary<string, string>{
                {"grant_type", "authorization_code"},
                {"client_id", client_id },
                {"client_secret", client_secret },
                {"redirect_uri", "https://localhost:7157/authorize" },
                {"code", code }
            };

            var link2 = "https://oauth2.googleapis.com/token";
            using (HttpClient client = new HttpClient())
            {
                bool attempt = false;

                while (true) { 
                    var resp = client.PostAsync(link2, new FormUrlEncodedContent(form)).Result;
                    var res = resp.Content.ReadAsStringAsync().Result;

                    if (resp.IsSuccessStatusCode && !(resp.Content.ToString().ToLower().Contains("error")))
                    {
                        Dictionary<string, object> isval = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                        string atoken = (string)isval["access_token"];
                        List<string> scopes = null;

                        try { 
                           scopes = JsonConvert.DeserializeObject<List<string>>((string)isval["scope"]);}
                        catch (Exception ex){
                            scopes = new List<string> { (string)isval["scope"] };
                        }
                        _baseExec.SetInBase(atoken, (string)isval["refresh_token"], Convert.ToInt32(isval["expires_in"]), scopes);
                        
                        
                        break;
                    }
                    // if token set is failed here goes only one time of removing it in line
                    else if(!attempt)
                    {
                        RevokeToken(_baseExec.GetFromBase().access_token);
                        attempt = true;
                    }
                }
            }
        }
        public string RevokeToken(string currentactivetoken)
        {
            // first revoke step of access tokend
            string revokestring = $"https://oauth2.googleapis.com/revoke?token={currentactivetoken}";
            RestClient client = new RestClient(revokestring);
            RestRequest request = new RestRequest();
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            return client.ExecutePost(request).ToString();
        }

        public CreateContact SetCredential()
        {
            GoogleCredential GetHashCode(string atoken)
            {
                // here  goes about cred distr which gives cred via accesstoken
                creds = GoogleCredential.FromAccessToken(atoken).CreateScoped(Scopes);
                return creds;
            }
            try{

                var iscurrententry = _baseExec.GetFromBase();

                DateTime currentdexpiredate = iscurrententry.last_change.AddSeconds(iscurrententry.expires_in);
                bool fordef = iscurrententry.last_change.AddSeconds((double)iscurrententry.expires_in) > DateTime.Now.ToUniversalTime();
                if (fordef)
                    creds = GetHashCode(iscurrententry.access_token);
                else
                {
                    // it is actually needless but stays for a reason of TODO
                    //this.RevokeToken(iscurrententry.refresh_token);
                    Debug.WriteLine("Manually excepting");
                    throw new Exception();
                }
            }catch (Exception ex){
                Debug.WriteLine(ex);
                content = new Tuple<bool, string>(false, StatExec.GenerateRedirectLink(client_id, UserIp));
                return this;
            }

            // laslty inintalization of code for making http requests without struggling in peopleservices
            peopleService = new PeopleServiceService(new BaseClientService.Initializer(){
                HttpClientInitializer = creds,
                ApplicationName = appname,
            });

            content = new Tuple<bool, string>(true, "Your input contacts will sooner created");
            
            return this;
        }

        public void ContactSaveNoticer(List<List<object>> contacts)
        {
            //Wh_Instance.Create();
            foreach(var contact in contacts)
            {
            }
        }

        public void Number(string group_id, bool contactToAdmins=false)
        {
            var users = _baseExec.GetUserById(group_id);
            foreach(var user in users){
                if (!user.hasContact)
                {
                    if (user.userStatus == "admin" && !contactToAdmins) { }
                    else
                        _baseExec.AcceptContact(user);
                    this.setPeople(user.userName, user.userWhatsappId);
                }
            }
        }
            
        public void Number(List<Dictionary<string, string>> numbers)
        {
            foreach(var user in numbers){
                this.setPeople(user["ContactName"], user["PhoneNumber"]);
                _baseExec.AcceptContact(whId: user["PhoneNumber"]);
            }
        }

        public string GetPeople()
        {
            var istotal = peopleService.ContactGroups.BatchGet().ToString();
            var from_me = peopleService.People.Connections.List("people/me");

            return $"{istotal}{from_me}";
        }

        public string setPeople(string firstName, string userNumber, string SecondName = null)
        {
            Google.Apis.PeopleService.v1.Data.Person person = new Google.Apis.PeopleService.v1.Data.Person();
            
            Name chef = new Name();
            chef.GivenName = firstName; chef.FamilyName = SecondName == null ? "" : SecondName;
            person.Names = new List<Name> { chef };

            PhoneNumber phone = new PhoneNumber();
            phone.Value = userNumber; phone.Type = "mobile";
            person.PhoneNumbers = new List<PhoneNumber> { phone };

            ContactToCreate contactToCreate = new ContactToCreate();
            contactToCreate.ContactPerson = person;
      
            BatchCreateContactsRequest request = new BatchCreateContactsRequest();
            request.Contacts = new List<ContactToCreate> { contactToCreate };
            //request.ReadMask = "names";

            BatchCreateContactsResponse reply = peopleService.People.BatchCreateContacts(request).Execute();
            return reply.ToString();
        }
    }
}
