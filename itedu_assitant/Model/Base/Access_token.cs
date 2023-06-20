namespace itedu_assitant.Model.Base
{
    public class Access_token
    {
        public int id { get; set; }
        public string  access_token{ get; set; }
        public int expires_in { get; set; }
        public List<string> scopes { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; } = "Bearer";
        public DateTime last_change { get; set; } = DateTime.Now.Date.ToUniversalTime();
            
    }
}
