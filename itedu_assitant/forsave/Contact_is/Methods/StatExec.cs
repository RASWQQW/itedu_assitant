namespace itedu_assitant.forsave.Contact_is.Methods
{

    // here must go methods which uses only userDatas
    public class StatExec
    {
        public StatExec(Dictionary<string, object> userdatas)
        {
            _userDatas = userdatas;
        }
        public Dictionary<string, object> _userDatas; 

        public string GenerateRedirectLink(string clientId, string? response_type = "code")
        {
            // here goes selenium mere code   
            string islink = "https://accounts.google.com/o/oauth2/v2/auth?" +
                "scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly&" +
                "access_type=offline&" +
                "include_granted_scopes=true&" +
                $"response_type={response_type}&" +
                $"state={new SecretGenerator().GetSecret((string)_userDatas["userIp"])}&" +
                "redirect_uri=https://localhost:7157/authorize&" +
                $"client_id={clientId}";

            return islink;
        }
    }
}
