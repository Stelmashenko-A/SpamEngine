using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace SpamEngine
{
    public class NewYearJob : IJob
    {
        protected const int Delay = 1000;
        protected const int GroupSize = 20;
        protected VkApi Vk { get; set; }
        protected string Message { get; set; } = "тестовое сообщение";

        protected IEnumerable<MediaAttachment> Attachments { get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {

                var dataMap = context.JobDetail.JobDataMap;
                Vk = (VkApi)dataMap[nameof(VkApi)];
                var userFilter = (UserFilter)dataMap[nameof(UserFilter)];

                var users = Vk.Friends
                    .Get(new FriendsGetParams
                    {
                        UserId = Vk.UserId,
                        Fields = ProfileFields.All,
                        Order = FriendsOrder.Hints
                    })
                    .Where(x => userFilter.Predicate(x)).ToList();
                InitMessage();
                Send(users);

            });
        }

        protected void InitMessage()
        {
            var fileName = ConfigurationManager.AppSettings["Message"];
            Message = File.ReadAllText(fileName);

            fileName = ConfigurationManager.AppSettings["Music"];

            var strs = File.ReadAllLines(fileName);
            Attachments = new List<MediaAttachment>
            {
                new Audio
                {
                    Id = long.Parse(strs[0].Split(',')[1]),
                    OwnerId = long.Parse(strs[1].Split(',')[1])
                }
            };

        }

        protected void Send(IList<VkNet.Model.User> users)
        {
            for (var i = 0; i < users.Count; i++)
            {
                try
                {
                    Vk.Messages.Send(new MessagesSendParams
                    {
                        Message = Message,
                        UserId = users[i].Id,
                        Attachments = Attachments
                    });
                    var t = DateTime.Now;
                    Console.WriteLine(i + " " + t.Minute + ' ' + t.Second);
                }
                catch (CaptchaNeededException)
                {
                    var t = DateTime.Now;
                    Console.WriteLine(i + " error " + t.Minute + ' ' + t.Second);

                }
                catch (Exception)
                {
                    Console.WriteLine(i);
                }
                finally
                {
                    Wait(i);
                }
            }
        }

        protected void Wait(int iteration)
        {
            if ((iteration + 1) % GroupSize == 0)
            {
                Task.Delay(60000 - GroupSize * Delay).Wait();
            }
            else
            {
                Task.Delay(Delay).Wait();
            }
        }
    }
}