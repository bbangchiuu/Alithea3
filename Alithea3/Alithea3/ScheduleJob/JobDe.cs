using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Alithea3.ScheduleJob
{
    public class JobDe : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            string value = dataMap.GetString("IDGJobID");

            //WriteLog.WriteFile.WriteToFile(value);
            Debug.WriteLine(value);
            return Task.Factory.StartNew(() => 0);
        }
    }
}