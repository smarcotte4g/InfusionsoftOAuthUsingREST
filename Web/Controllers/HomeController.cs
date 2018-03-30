using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using InfusionsoftOAuth.Models;
using Newtonsoft.Json.Linq;

namespace InfusionsoftOAuth.Controllers
{
    public class HomeController : Controller
    {
        private string DeveloperAppKey = "ENTERYOURAPPKEY";
        private string DeveloperAppSecret = "ENTERYOURAPPSECRET";

        private string CallbackUrl = "http://localhost:2412/home/callback";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authorize()
        {
            // Hard coded "scope=full" and "responseType=code" since they are the only supported options
            string authorizeUrlFormat = "https://signin.infusionsoft.com/app/oauth/authorize?scope=full&redirect_uri={0}&response_type=code&client_id={1}";

            // Url encode CallbackUrl
            return Redirect(string.Format(authorizeUrlFormat, Server.UrlEncode(CallbackUrl), DeveloperAppKey));
        }

        public ActionResult Callback(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string tokenUrl = "https://api.infusionsoft.com/token";

                // Hard coded "grant_type=authorization_code" since it is the only supported option
                string tokenDataFormat = "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";

                HttpWebRequest request = HttpWebRequest.Create(tokenUrl) as HttpWebRequest;
                request.Method = "POST";
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";

                // Url encode CallbackUrl
                string dataString = string.Format(tokenDataFormat, code, DeveloperAppKey, DeveloperAppSecret, Server.UrlEncode(CallbackUrl));
                var dataBytes = Encoding.UTF8.GetBytes(dataString);
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(dataBytes, 0, dataBytes.Length);
                }

                string resultJSON = string.Empty;
                using (WebResponse response = request.GetResponse())
                {
                    var sr = new StreamReader(response.GetResponseStream());
                    resultJSON = sr.ReadToEnd();
                    sr.Close();
                }

                var jsonSerializer = new JavaScriptSerializer();

                var tokenData = jsonSerializer.Deserialize<TokenData>(resultJSON);

                ViewData.Add("AccessToken", tokenData.Access_Token);
            }

            return View();
        }
        // Refactored to work with REST and not Legacy xmlrpc
        public async Task<ActionResult> FindContact()
        {
            var accessToken = Request.QueryString["AccessToken"];
            var email = Request.QueryString["Email"];
            var Baseurl = "https://api.infusionsoft.com/crm/rest/v1/";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("contacts?email=" + email + "&access_token=" + accessToken);
                if (Res.IsSuccessStatusCode)
                {
                    var ConResponse = Res.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(ConResponse);
                    int count = (int)json["count"];
                    List<Contact> contact = new List<Contact>();
                    for (int i = 0; i < count; i++)
                    {
                        contact.Add(new Contact
                        {
                            Id = (int)json["contacts"][i]["id"],
                            FirstName = (string)json["contacts"][i]["given_name"],
                            LastName = (string)json["contacts"][i]["family_name"]
                        });
                    }
                    return View(contact);
                }
                return View();
            }
        }
    }
}
