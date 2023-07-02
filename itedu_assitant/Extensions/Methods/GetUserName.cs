using HtmlAgilityPack;
using OpenQA.Selenium;
using RestSharp;
using System.Diagnostics;
using System.Text;

namespace itedu_assitant.forsave.Methods
{
    public class GetUserName
    {

        public string htmlContent;
        public string finalValue;
        public GetUserName()
        {
            
        }

        public GetUserName GetResponse(string UserNumber)
        {
            new NumberManager().GetProperNumber(UserNumber, out string userNumber);
            string currentLink = $"https://wa.me/{userNumber}";


            RestClient client = new RestClient();
            RestRequest request = new RestRequest(currentLink, Method.Get);
            byte[] resp = client.DownloadData(request);
            htmlContent = Encoding.UTF8.GetString(resp);
            return this;
        }

        public string __GetUserName()  // it mainly gets about presumable name from business accounts only 
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            string isclass = "_9vd6 _9t33 _9bir _9bj3 _9bhj _9v12 _9tau _9tay _9u6w _9se- _9u5y";
            string Text = doc.DocumentNode.Descendants("div")
                .First(w=>w.GetAttributeValue("class", "").Contains(isclass))
                .SelectSingleNode(".//h3[@class=\"_9vd5 _9scb _9scr\"]").InnerText;

            var GetXpath = doc.DocumentNode
                .SelectSingleNode("/html/body/div[1]/div[1]/div[2]/div/section/div/div/div/div[2]/div[1]/h3");

            Debug.WriteLine(Text);
            Debug.WriteLine(GetXpath.InnerText);
            return GetXpath.InnerText;
        }
    }
}
