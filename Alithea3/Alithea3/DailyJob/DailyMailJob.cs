using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Alithea3.DailyJob
{
    public class DailyMailJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            
            Debug.WriteLine("dang chay");

            return Task.Factory.StartNew(() => 0);
            //return (Task) context.Result;
        }
    }
}