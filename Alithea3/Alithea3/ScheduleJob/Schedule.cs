using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Alithea3.ScheduleJob
{
    public class Schedule
    {
        public static async Task Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            for(int i = 0; i < 3; i++)
            {
                IJobDetail job = JobBuilder.Create<JobDe>().WithIdentity("IDGjobID-" + i, "IDG-" + i).UsingJobData("IDGJobID", "noi dung " + i).Build();
                ITrigger trigger = TriggerBuilder.Create().WithIdentity("IDGjobID-" + i, "IDG-" + i).StartAt(DateTime.Parse("2020/05/08 10:50")).WithPriority(1).Build();

                await scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}