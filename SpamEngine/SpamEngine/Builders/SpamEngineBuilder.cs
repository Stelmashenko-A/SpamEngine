using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using VkNet;

namespace SpamEngine.Builders
{
    public class SpamEngineBuilder
    {
        protected IScheduler Scheduler { get; set; }
        protected IJobDetail Job { get; set; }
        protected ITrigger NewYearJobTrigger { get; set; }

        public async Task Build()
        {
            try
            {
                await BuildScheduler();
                BuildJob();
                BuildTrigger();

                await Scheduler.ScheduleJob(Job, NewYearJobTrigger);
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }

        protected async Task BuildScheduler()
        {
            var factory = new StdSchedulerFactory();
            Scheduler = await factory.GetScheduler();
            await Scheduler.Start();
        }

        protected void BuildJob()
        {
            Job = JobBuilder.Create<NewYearJob>()
                .WithIdentity(nameof(NewYearJob))
                .SetJobData(new JobDataMap(
                    (IDictionary)new Dictionary<string, object>
                    {
                        {nameof(VkApi), new VkApiBuilder().Build()},
                        {nameof(UserFilter), new UserFilter()}
                    }))
                .Build();
        }

        protected void BuildTrigger()
        {
            var cron = ConfigurationManager.AppSettings["Cron"];
            NewYearJobTrigger = TriggerBuilder.Create()
                .WithIdentity(nameof(NewYearJobTrigger))
                .WithCronSchedule(cron)
                .ForJob(nameof(NewYearJob))
                .Build();
        }
    }
}