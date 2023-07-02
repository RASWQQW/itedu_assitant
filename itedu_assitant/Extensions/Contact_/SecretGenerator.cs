using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using itedu_assitant.DB;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace itedu_assitant.forsave.Contact_is
{

    public class JsonType {
        public string secret { get; set; }
        public string userip { get; set; }
        public string? date { get; set; } = null;
    }

    public class SecretGenerator
    {
        private string path = Path.Combine(Directory.GetCurrentDirectory(), "Extensions", "Contact_is", "secrets.json");
        private readonly Random rnd;
        private IDatabase _redis;
        public SecretGenerator()
        {
            _redis = new rediscontext().GetBase();
            rnd = new Random();
        }

        string RandomString(int len, bool lowercase)
        {
            string stringpassword = "";
            while (stringpassword.Length < len)
            {
                var newrandom = rnd.Next(1, 255);
                string isstringval = Convert.ToChar(Convert.ToInt32(newrandom)).ToString();
                if (new Regex(@"^[A-Za-z]$").Match(isstringval).Success)
                    stringpassword += isstringval.ToString();
            }
            return lowercase ? stringpassword.ToLower() : stringpassword;
        }

        public string RandomInt((int, int)size, int len){

            string val = "";
            for (int l = 0; l < len + 1; l++){
                val += rnd.Next(size.Item1, size.Item2).ToString();
            }
            return val;
        }

        public string Generate(bool state)
        {
            StringBuilder isstring = new StringBuilder();
            if (state){
                isstring.Append(RandomString(5, false));
                isstring.Append(RandomInt((1, 9), 3));
                isstring.Append(RandomString(5, false));

            }
            return isstring.ToString();
        
        }

        public dynamic GetSecret(string ClientIp, bool inner = false)
        {
            string iscode = File.ReadAllText(this.path);
            List<JsonType> tjp = JsonConvert.DeserializeObject<List<JsonType>>(iscode);
            for(int i = 0; i < tjp.Count(); i++)
            {
                if (tjp[i].userip == ClientIp)
                    if(inner)
                        return (tjp[i], tjp, i);
                else
                    return tjp[i].secret;
            }
            if (!inner)
            {
                string isnewcode = Generate(true);
                SaveSecret(isnewcode, ClientIp);
                return isnewcode;
            }
            return ("None", tjp);
        }

        public void SaveSecret(string iscode, string userip)
        {
            var isusers = GetSecret(userip, true);
            if (isusers.Item1 == "None")
                ((List<JsonType>)isusers.Item2).Add(
                    new JsonType { secret = iscode, userip = userip });
            else
                isusers.Item2[isusers.Item3].secret = iscode;

            var isjson = JsonConvert.SerializeObject(isusers);
            File.WriteAllText(path, isjson);
            //"File succesfully saved";
        }

        public string GetSecretRedis(string userIp)
        {
            return _redis.StringGet($"userIp-{userIp}").ToString() ?? "";
        }
        public string SetSecretRedis(string userIp)
        {
            return _redis.StringSetAndGet($"userIp-{userIp}", this.Generate(state:true)).ToString();
        }

        public void SetNewUser(string userIp)
        {
            var givenuserkey = $"userIp-{userIp}";
            if (!_redis.KeyExists(givenuserkey))
                this.SetSecretRedis(userIp);

        }
    }
}
