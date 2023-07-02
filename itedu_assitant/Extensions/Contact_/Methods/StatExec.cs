namespace itedu_assitant.forsave.Contact_is.Methods
{

    // here must go methods which uses only userDatas
    public class StatExec
    {
        public StatExec()
        {
            //_userDatas = userdatas;
        }

        //public Dictionary<string, object> _userDatas; 

        public static string GenerateRedirectLink(string clientId, string userIp, string? response_type = "code")
        {

            // new SecretGenerator().GetSecretRedis((string)_userDatas["userIp"])
            // here goes selenium mere code   
            //  "scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly&" +
            string islink = "https://accounts.google.com/o/oauth2/v2/auth?" +
                "scope=https%3A//www.googleapis.com/auth/contacts&" +
                "access_type=offline&" +
                "include_granted_scopes=true&" +
                $"response_type={response_type}&" +
                $"state={new SecretGenerator().GetSecretRedis(userIp)}&" +
                "redirect_uri=https://localhost:7157/Specifics/authorize&" +
                $"client_id={clientId}";

            return islink;
        }
    }
}
