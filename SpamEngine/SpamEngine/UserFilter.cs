using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace SpamEngine
{
    public class UserFilter
    {
        protected IEnumerable<long> BlackList { get; set; }

        public UserFilter()
        {
            var fileName = ConfigurationManager.AppSettings["BlackList"];
            BlackList = File.ReadAllLines(fileName)
                .Select(x=>x.Split(','))
                .Select(x=>x.First())
                .Select(long.Parse);
        }

        public bool Predicate(VkNet.Model.User user)
        {
            return !BlackList.Contains(user.Id);
        }
    }
}