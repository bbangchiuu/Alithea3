using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace Alithea3.DailyJob
{
    public class OurScheduler
    {
        public static async Task Start()
        {
            /* STEP 1: we create a scheduler and Start it. */
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            /* STEP 2: We create a DailyMailJob instance using Quartz JobBuilder. */
            IJobDetail job = JobBuilder.Create<DailyMailJob>().Build();

            /* STEP 3: We Create a Trigger and configure it to fire once everyday. */
            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => s.WithIntervalInSeconds(10).OnEveryDay().StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))).Build();

            /* STEP 4: Finally we schedule our Job in the scheduler using the trigger we created above. */
            await scheduler.ScheduleJob(job, trigger);
        }
    }
}