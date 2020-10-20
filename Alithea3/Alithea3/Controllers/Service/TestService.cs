using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alithea3.Controllers.Service
{
    public class TestService : ITestService
    {
        public string get()
        {
            return "hello";
        }
    }
}