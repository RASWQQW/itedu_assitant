using itedu_assitant.Model;
using Microsoft.Extensions.Hosting;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace itedu_assitant.forsave.Methods
{
    public class Wh_Instance
    {
        static public string IdInstance = "1101817976";
        static public string ApiToken = "2296c806df794cf58b7006198428a267249c87ae5f444c6fab";
        static string AvatarPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Files");

        // And it is static value who cathes a big value
        static private Wh_Instance Instance;

        // It is first mesurment of Singleton
        static public Wh_Instance Create(string id_ins = "null", string apitoken = "null")
        {
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
        public byte[] GetQr()
        {
            string qr_get_link = $"https://api.green-api.com/waInstance{IdInstance}/qr/{ApiToken}";

            using(var client = new HttpClient())
            {
                HttpResponseMessage message = client.GetAsync(qr_get_link).Result;
                Dictionary<string, object> ResponseMessage = JsonConvert.DeserializeObject<Dictionary<string, object>>(message.Content.ToString());
                return Encoding.ASCII.GetBytes((string)ResponseMessage["message"]);
            }
        }

        public string Send_Message(string chat_id, string message)
        {

            var link = $"https://api.green-api.com/waInstance{IdInstance}/SendMessage/{ApiToken}";
            Dictionary<string, object> current_json = new Dictionary<string, object> { { "chat_id", chat_id }, { "message", message } };
            string messge_id = RequestSender(link, values: new List<Dictionary<string, object>> { new Dictionary<string, object> { { "type", "json" }, { "jsoncontent", current_json } } });
            return messge_id;
        }

        // Group making area
        public string CreateGroup(string groupName,
                                IEnumerable<string> chatIds,
                                IEnumerable<string> admins = null,
                                string Avatar = "None")
        {
            string func_link = $"https://api.green-api.com/waInstance{IdInstance}/createGroup/{ApiToken}";

            if (admins != null)
                MakeAdmin(admins);

            if (Avatar == "None")
                Avatar = AvatarPath;

            SetPhoto(Avatar);

            return "Check";
        }


        public List<object> MakeAdmin(IEnumerable<string> admins)
        {
            string link = $"https://api.green-api.com/waInstance{IdInstance}/setGroupAdmin/{ApiToken}";

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

        public string RequestSender(string Link, List<Dictionary<string, object>> values)
        {

            using (HttpClient httpc = new HttpClient())
            {


                var formData = new MultipartFormDataContent();
                StringContent content = null;

                //foreach (Dictionary<string, object> val in values)
                //{
                //    if (val.ContainsKey("type") && val["type"] == "form")
                //    {
                //        formData.Add(val["object"], val["name"], val["filename"]);
                //    }
                //    else if (val["type"] == "json")
                //    {
                //        content = new StringContent(val["jsoncontent"], Encoding.UTF8, "application/json");
                //    }
                //}

                HttpResponseMessage message = null;
                if (content != null)
                {
                    message = httpc.PostAsync(Link, content).Result;
                }
                else
                {
                    message = httpc.PostAsync(Link, formData).Result;
                }

                return message.Content.ToString();
            }

        }

    }
}
