using itedu_assitant.Model;
using Microsoft.Extensions.Hosting;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using itedu_assitant.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using itedu_assitant.DB;
using Microsoft.Extensions.Primitives;
using itedu_assitant.Model.Base;
using Microsoft.EntityFrameworkCore;

namespace itedu_assitant.forsave.Methods
{
    public class Wh_Instance
    {
        static public string IdInstance = "1101817976";
        static public string ApiToken = "2296c806df794cf58b7006198428a267249c87ae5f444c6fab";
        static string AvatarPath = Combine(new List<string> { "images", "groupimage.png" });
        static string QRPath = Combine(new List<string> { "qr" });
        static dbcontext _context;
        static IEnumerable<EndpointDataSource> _endpoints;

        static string Combine(List<string> vals)
        {
            string tempval = Path.Combine(Directory.GetCurrentDirectory(), "forsave", "Files");
            foreach (var val in vals){
                tempval = Path.Combine(tempval, val);
            }

            return tempval;
        }

        // And it is static value who cathes a big value
        static private Wh_Instance Instance;

        // It is first measurment of Singleton
        static public Wh_Instance Create(dbcontext context, IEnumerable<EndpointDataSource> endpoints = null, string id_ins = "null", string apitoken = "null")
        {

            _endpoints = endpoints;
            _context = context;
            bool newVals = true;

            if (id_ins == "null" || apitoken == "null")
            {
                id_ins = IdInstance; apitoken = ApiToken;
                newVals = false;
            }
            if (Instance == null || newVals)
                Instance = new Wh_Instance(id_ins, apitoken);
            return Instance;
        }
        // It is private accordingly non accesable outisde constructor
        private Wh_Instance(string idinstance, string apitoken, params object[] vals)
        {
            IdInstance = idinstance;
            ApiToken = apitoken;
        }

        // Area where happens basic manners


        // It actually gets qr link as string and resturns as response
        public string GetQr()
        {
            string qr_get_link = $"https://api.green-api.com/waInstance{IdInstance}/qr/{ApiToken}";

            using(var client = new HttpClient())
            {
                var ResponseMessage = client.GetAsync(qr_get_link).Result.Content.ReadFromJsonAsync<Dictionary<string, object>>().Result;
                //Dictionary<string, object> ResponseMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

                string givenString = ResponseMessage["message"].ToString();
                if (givenString != null)
                {
                    byte[] imageBytes = Convert.FromBase64String(givenString);
                    string filename = QRPath + "\\UserQr.png";
                    File.WriteAllBytes(filename, imageBytes);
                    return filename;
                }
                else
                    return "Expected bytes are absence";
            }
        }
        
        // Here starts a checking of next methods to be sure with them
        public bool IsActive(){
            return new CheckActive(this, _context).GetInstanceStatus()["stateInstance"].ToString() == "authorized";
        }

        public Dictionary<string, object> GetActionUrl(string funcIs)
        {
            var HostString = Environment.GetEnvironmentVariable("HostServer2");
            // It first goes to get that one whole endpoints
            if(_endpoints != null)
            {
                var currentendPoints = _endpoints
                    .SelectMany(val => val.Endpoints)
                    .OfType<RouteEndpoint>();

                // after this we gonna catch specefic controller and action url route
                var urlroute = currentendPoints
                                .Select(vals => vals.Metadata
                                    .OfType<ControllerActionDescriptor>()
                                    .Where(val => val.ActionName.Contains(funcIs))
                                    .Select(val => {
                                        return new Dictionary<string, object>{
                                            {"ActionIs", val.RouteValues.Values},
                                            { "RouteIs", vals.RoutePattern.RawText.TrimStart('/')}
                                        };
                                    }));

                var itworks = new Dictionary<string, object>();
                int routecount = 0;
                foreach(var val in urlroute){
                    var firstIs = val.FirstOrDefault();
                    if(firstIs != null && firstIs is IDictionary){
                        itworks.TryAdd("action", firstIs["ActionIs"]);
                        if (HostString != null){
                            if (itworks.ContainsKey($"route{firstIs["ActionIs"]}{routecount}")) routecount++;
                                itworks.TryAdd($"route{firstIs["ActionIs"]}{routecount}", $"{HostString}/{firstIs["RouteIs"]}");
                        }
                    }
                }
                return itworks;
            }
            throw new Exception("here have to be _endpints in object");
        }

        public string Send_Message(string chat_id, string message) 
        {
            if (!IsActive())
                // GetActionUrl(typeof(HomeController).GetMethods().FirstOrDefault(w => w.Name.Contains("SetNumber")).Name)
                return $"You must to login {GetActionUrl("SetNumber")}";

            chat_id = new Manager().GetNumberAsToken(chat_id);
            var link = $"https://api.green-api.com/waInstance{IdInstance}/SendMessage/{ApiToken}";
            Dictionary<string, object> current_json = new Dictionary<string, object> { { "chatId", chat_id }, { "message", message } };
            var newsendingvalue = 
                new List<Dictionary<string, object>>{ 
                    new Dictionary<string, object> { 
                        { "type", "json" }, 
                        { "jsoncontent", current_json } 
                    } 
                };

            string messge_id = RequestSender(link, qtype: "post", values: newsendingvalue) ;
            return messge_id;
        }

        // Group making area


        public void DeleteGroup(string? groupName = null, int? groupId = null )
        {
            if(groupName != null)
            {
                var curgr = _context.contusergroups.FirstOrDefault(w => w.name == groupName);
                _context.contusergroups.Remove(curgr);
            }
            else if (groupId != null)
            {
                var curgr = _context.contusergroups.FirstOrDefault(w => w.group_id == groupName);
                _context.contusergroups.Remove(curgr);
            }
            _context.SaveChanges();
        }

        public void DeleteAdmin()
        {
            
        }

        public void DeleteUser()
        {

        }


        // create group
        public string CreateGroup(string groupName,
                                List<string> chatIds,
                                List<string> admins = null,
                                string Avatar = "None")
        {

            if (!IsActive())
                // GetActionUrl(typeof(HomeController).GetMethods().FirstOrDefault(w => w.Name.Contains("SetNumber")).Name)
                return $"You must to login {GetActionUrl("SetNumber")}";

            string func_link = $"https://api.green-api.com/waInstance{IdInstance}/createGroup/{ApiToken}";


            List<string> MakeProperNumber<T>(List<T> numbers) {
                List<string> newchatIds = new List<string>();
                for (int i = 0; i < numbers.Count(); i++)
                {
                    newchatIds.Add(new Manager().GetNumberAsToken(numbers.ToList()[i]));
                }
                return newchatIds;
            }

            var currentContent = new Dictionary<string, object> { { "groupName", groupName }, { "chatIds", MakeProperNumber<string>(chatIds) } };
            var iscontent = new List<Dictionary<string, object>> {
                new Dictionary<string, object> { 
                    { "type", "json" }, 
                    { "jsoncontent", currentContent}
                }
            };

            var isresult = JsonConvert.DeserializeObject<Dictionary<string, object>>(RequestSender(func_link, qtype: "post", iscontent));
            isresult.TryGetValue("chatId", out object? isgroupid);
            isresult.TryGetValue("groupInviteLink", out object? islink);

            if(isgroupid != null && islink != null)
            {
                _context.Add(new Groups{
                        name = groupName != null ? groupName : "new lec group" + _context.contusergroups.FirstOrDefault().id,
                        group_id = (string)isgroupid,
                        group_link= (string)islink,

                    });
                _context.SaveChanges();
            }


            if (admins != null)
                MakeAdmin(MakeProperNumber<string>(admins), (string)isgroupid);

            if (Avatar == "None") 
                Avatar = AvatarPath;
            SetPhoto(Avatar);

            return "Check";
        }


        public List<object> MakeAdmin(IEnumerable<string> admins, string chatid)
        {
            string link = $"https://api.green-api.com/waInstance{IdInstance}/setGroupAdmin/{ApiToken}";

            List<object> responses = new List<object>();
            foreach(string admin in admins){
                var content_is = new Dictionary<string, object> { { "groupId", chatid }, { "participantChatId", admin} };
                var response = RequestSender(link, qtype: "post", new List<Dictionary<string, object>> { new Dictionary<string, object> { { "type", "json" }, { "jsoncontent", content_is } } });
                responses.Add(response);
            }

            return new List<object> { "Check" };
        }

        public void SetPhoto(string photoPath)
        {
            using (HttpClient httpc = new HttpClient())
            {
                // Reading a file inside file to capable of getting message
                Stream file = File.OpenRead(photoPath);
                var mems = new MemoryStream();
                file.CopyTo(mems);
                byte[] bytearray = mems.ToArray();

                string requestLink = $"https://api.green-api.com/waInstance{IdInstance}/setGroupPicture/{ApiToken}";

                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new ByteArrayContent(bytearray), "file", "image/jpeg");
                    HttpResponseMessage messge = httpc.PostAsync(requestLink, formData).Result;
                }
            }
        }

        public string RequestSender(string Link, string qtype, List<Dictionary<string, object>>? values = null)
        {
            using (HttpClient httpc = new HttpClient())
            {
                if(qtype == "get")
                    return httpc.GetAsync(Link).Result.Content.ReadAsStringAsync().Result;

                if (qtype == "post" && values != null)
                {
                    var formData = new MultipartFormDataContent();
                    StringContent content = null;

                    foreach (Dictionary<string, object> val in values)
                    {
                        if(val.ContainsKey("type"))
                            if (val["type"].ToString() == "json")
                            {
                                if (val.ContainsKey("jsoncontent") && val["jsoncontent"] is Dictionary<string, object>)
                                {
                                    var currentjson = val["jsoncontent"];
                                    Debug.WriteLine(currentjson + " " + currentjson.GetType().ToString());
                                    string isstring = JsonConvert.SerializeObject(currentjson);
                                    content = new StringContent(isstring, Encoding.UTF8, "application/json");
                                }
                                else if (val["type"] == "form"){
                                    var firstarg = val["type"];
                                    Dictionary<string, object> nexc = val.ContainsKey("content") ? (Dictionary<string, object>)val["content"] : null;
                                    if (firstarg as HttpContent != null)
                                       if (nexc == null)
                                           formData.Add((HttpContent)firstarg);
                                       else
                                           formData.Add((HttpContent)firstarg, nexc.ContainsKey("name") ? (string)nexc["name"] : default, nexc.ContainsKey("filename") ? (string)nexc["filename"] : default);
                                }   
                            }
                    }

                    HttpResponseMessage message;
                    if (content != null)
                        message = httpc.PostAsync(Link, content).Result;
                    else
                        message = httpc.PostAsync(Link, formData).Result;

                    return message.Content.ReadAsStringAsync().Result;
                }

                throw new ArgumentException("You didn't pass qtype arg or is wrong");

            }

        }

    }
}
