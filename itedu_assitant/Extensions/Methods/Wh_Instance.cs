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
using Aspose.Email;
using itedu_assitant.Model.ForvView;
using System.Net.Http;

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
            string tempval = Path.Combine(Directory.GetCurrentDirectory(), "Extensions", "Files");
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
                if (givenString != null && !givenString.ToLower().Contains("already"))
                {
                    byte[] imageBytes = Convert.FromBase64String(givenString);
                    string filename = QRPath + "\\UserQr.png";
                    File.WriteAllBytes(filename, imageBytes);
                    return filename;
                }
                else
                    return givenString;
            }
        }
        
        // Here starts a checking of next methods to be sure with them
        public bool IsActive(){
            return new InstanceCheckActive(this, _context).GetInstanceStatus()["stateInstance"].ToString() == "authorized";
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

            chat_id = new NumberManager().GetNumberAsId(chat_id);
            var link = $"https://api.green-api.com/waInstance{IdInstance}/SendMessage/{ApiToken}";
            var newsendingvalue = new List<ContentDelivery>(){ };
            newsendingvalue.Add(
                    new ContentDelivery{ 
                        type="json",
                        jsoncontent = new Dictionary<string, object> { { "chatId", chat_id }, { "message", message } }
                    }
                );

            string messge_id = RequestSender(link, qtype: "post", values: newsendingvalue) ;
            return messge_id;
        }

        // Group making area
        public void DeleteGroup(string? groupName = null, int? groupId = null )
        {
            Groups curgr = null;
            if(groupName != null)
                curgr = _context.contusergroups.FirstOrDefault(w => w.name == groupName);
            else if (groupId != null)
                curgr = _context.contusergroups.FirstOrDefault(w => w.group_id == groupName);

            if(curgr != null)
            {
                var users = _context.contusers.Where(w => w.userGroup.group_id == curgr.group_id).ToList();
                foreach(var user in users)
                {
                    DeleteUser(user.userWhatsappId,curgr.group_id);
                }

                var leaveUrl = $"https://api.green-api.com/waInstance{IdInstance}/leaveGroup/{ApiToken}";

                var data = new List<ContentDelivery>() { };
                data.Add(new ContentDelivery{
                        type = "json",
                        jsoncontent = new Dictionary<string, object> { { "groupId", curgr.group_id } }
                    }
                );
                RequestSender(leaveUrl, qtype: "post", values: data);
            }
            _context.SaveChanges();

            // https://green-api.com/docs/api/groups/RemoveGroupParticipant/
            // https://green-api.com/docs/api/groups/LeaveGroup/
        }

        public void DeleteAdmin()
        {
            
        }

        public void DeleteUser(string UserId, string GroupId)
        {
            var deleteLink = $"https://api.green-api.com/waInstance{IdInstance}/removeGroupParticipant/{ApiToken}";

            var contents = new List<ContentDelivery>() { };
            contents.Add(new ContentDelivery {
                        type = "json",
                        jsoncontent = new Dictionary<string, object> { { "groupId", UserId }, { "participantChatId", GroupId } }
                    }
            );

            RequestSender(Link: deleteLink, values: contents, qtype: "post");
            
            // https://green-api.com/docs/api/groups/LeaveGroup/
        }

        public List<string> MakeProperNumber<T>(List<T> numbers)
        {
            List<string> newchatIds = new List<string>();
            for (int i = 0; i < numbers.Count(); i++)
            {
                newchatIds.Add(new NumberManager().GetNumberAsId(numbers.ToList()[i]));
            }
            return newchatIds;
        }

        public dynamic AddUsersToGroup(List<string> Users, string gr_id)
        {
            string link = $"https://api.green-api.com/waInstance{IdInstance}/addGroupParticipant/{ApiToken}";
            List<string> userNumbers = MakeProperNumber<string>(Users);

            foreach(string userNumber in userNumbers)
            {
                var sendingcontent = new List<ContentDelivery>() { };
                sendingcontent.Add(
                        new ContentDelivery{
                            type = "json",
                            jsoncontent = new Dictionary<string, object> { { "groupId", gr_id }, { "participantChatId", userNumber } }
                        });

                var response = RequestSender(link, values: sendingcontent, qtype: "post");
                Debug.WriteLine(response);
            }

            return "Success";
        }
        // create group
        public dynamic CreateGroup(List<string> properNumbers,
                                Groups newGroup,
                                string groupName = null,
                                List<string> admins = null,
                                string Avatar = "None")
        {
            if (!IsActive())
                // GetActionUrl(typeof(HomeController).GetMethods().FirstOrDefault(w => w.Name.Contains("SetNumber")).Name)
                return $"You must to login {GetActionUrl("SetNumber")}";

            string func_link = $"https://api.green-api.com/waInstance{IdInstance}/createGroup/{ApiToken}";
            var newadmins = new List<string>();

            for(int v = 0; v < admins.Count(); v++){
                newadmins.Add(properNumbers.FirstOrDefault(val => val.Contains(admins[v][1..])));
            }

            var data = new List<ContentDelivery>() { };
            data.Add(new ContentDelivery{
                    type = "json",
                    jsoncontent = new Dictionary<string, object> { { "groupName", newGroup.name }, { "chatIds", properNumbers } }
                }
            );

            var isresult = JsonConvert.DeserializeObject<Dictionary<string, object>>(RequestSender(func_link, qtype: "post", data));
            isresult.TryGetValue("chatId", out object? isgroupid);
            isresult.TryGetValue("groupInviteLink", out object? islink);

            // start of the base and other stuffs
            try{if (newadmins != null || newadmins.Count() > 0)
                    MakeAdmin(admins, (string)isgroupid);
            }catch (Exception ex) { Debug.WriteLine(ex); }

            try{if (Avatar == "None")
                    Avatar = AvatarPath;
                SetPhoto(isgroupid.ToString(), Avatar);
            }catch (Exception ex) { Debug.WriteLine(ex); }

            var ReturnObject = new Dictionary<string, object>();

            // saving new group into base
            // [OPTIONAL]
            if (isgroupid != null && islink != null)
            {
                newGroup.group_id = (string)isgroupid;
                newGroup.group_link = (string)islink;
                
                ReturnObject.Add("BasegroupId", newGroup); // add base gr id to take it into AuthCodeGetting
                _context.Update(newGroup);
                _context.SaveChanges();
            }

            ReturnObject.Add("WhatsAppgroupId", isgroupid.ToString());
            ReturnObject.Add("WhatsAppgroupLink", islink.ToString());
            return ReturnObject;
        }

        public List<object> MakeAdmin(IEnumerable<string> admins, string chatid)
        {
            string link = $"https://api.green-api.com/waInstance{IdInstance}/setGroupAdmin/{ApiToken}";
            List<object> responses = new List<object>();
            foreach(string admin in admins){
                var content_is = new Dictionary<string, object> { { "groupId", chatid }, { "participantChatId", admin} };
                var response = RequestSender(link, qtype: "post", new List<ContentDelivery> { new ContentDelivery { type = "json", jsoncontent = content_is } });
                responses.Add(response);
            }

            return new List<object> { "Check" };
        }

        public void SetPhoto(string groupId, string photoPath)
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

                    string isstring = string.Format(@"{{""groupId:"" ""{0}""}}", arg0: groupId);
                    string secondString = JsonConvert.SerializeObject(new Dictionary<string, object> { { "groupId", groupId } });
                   

                    bool vall = isstring == secondString;
                    formData.Add(new StringContent(isstring, Encoding.UTF8, "application/json"));;
                    formData.Add(new ByteArrayContent(bytearray), "file", "image/jpeg");
                    HttpResponseMessage message = httpc.PostAsync(requestLink, formData).Result;
                    Debug.WriteLine(message.Content.ReadAsStringAsync().Result);
                    Debug.WriteLine(message.StatusCode);
                }
            }
        }

        public string RequestSender(string Link, string qtype, List<ContentDelivery> values = null)
        {
            using (HttpClient httpc = new HttpClient())
            {
                if(qtype == "get")
                    return httpc.GetAsync(Link).Result.Content.ReadAsStringAsync().Result;

                if (qtype == "post" && values != null)
                {
                    var formData = new MultipartFormDataContent();
                    StringContent Stringcontent = null;

                    foreach (ContentDelivery itercontent in values)
                    {
                        if(Stringcontent == null && itercontent.type != null) { 
                            if (itercontent.type == "json")
                            {
                                var currentjson = itercontent.jsoncontent;
                                
                                Debug.WriteLine(currentjson + " " + currentjson.GetType().ToString());

                                string isstring = JsonConvert.SerializeObject(currentjson);
                                Stringcontent = new StringContent(isstring, Encoding.UTF8, "application/json");
                            }
                            else if (itercontent.type == "form"){
                                Dictionary<string, object> ccontent = itercontent.formcontent;
                                if (ccontent["HttpContent"] as HttpContent != null){
                                    formData.Add((HttpContent)ccontent["HttpContent"],
                                                ccontent.ContainsKey("name") ? (string)ccontent["name"] : default,
                                                ccontent.ContainsKey("filename") ? (string)ccontent["filename"] : default);
                                }
                            }
                        }
                    }
                    HttpResponseMessage message;
                    if (Stringcontent != null)
                            message = httpc.PostAsync(Link, Stringcontent).Result;
                    else
                        message = httpc.PostAsync(Link, formData).Result;

                    return message.Content.ReadAsStringAsync().Result;
                }

                throw new ArgumentException("You didn't pass qtype arg or is wrong");

            }

        }

    }
}
