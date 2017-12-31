using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using SpamEngine.Models;
using VkNet;
using VkNet.Enums.Filters;

namespace SpamEngine.Builders
{
    public class VkApiBuilder
    {
        public VkApi Build()
        {
            var fileName = ConfigurationManager.AppSettings["Credentials"];
            var json = File.ReadAllText(fileName);
            var credentials = JsonConvert.DeserializeObject<Credentials>(json);

            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = credentials.AppId,
                Login = credentials.Login,
                Password = credentials.Password,
                Settings = Settings.Messages
            });

            return vk;
        }
    }
}