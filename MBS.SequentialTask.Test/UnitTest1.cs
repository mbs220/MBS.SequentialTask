using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MBS.SequentialTask.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MyModelHandler mdl = new MyModelHandler
            {
                Dates = new List<DateTime>() { DateTime.Now },
                Sum = 1,
            };

            ConcurrencyHandler<MyModelHandler> q = new ConcurrencyHandler<MyModelHandler>(mdl);
            q.RegisterTaskToQueue((m) =>
            {
                m.Sum += 50;
                m.Dates.Add(DateTime.Now);

                var id = Thread.CurrentContext.ContextID;
                var tid = Thread.CurrentThread.ManagedThreadId;
                var tskId = Task.CurrentId;

                //Thread.Sleep(1000);
                return m;
            });
            // 51
            q.RegisterTaskToQueue((d) =>
            {
                d.Sum *= 2;
                d.Dates.Add(DateTime.Now);
                //Thread.Sleep(4000);
                var id = Thread.CurrentContext.ContextID;
                var tid = Thread.CurrentThread.ManagedThreadId;
                var tskId = Task.CurrentId;

                return d;
            });
            // 102
            q.RegisterTaskToQueue((z) =>
            {
                //Thread.Sleep(2500);

                z.Sum -= 2;
                z.Dates.Add(DateTime.Now);
                var id = Thread.CurrentContext.ContextID;
                var tid = Thread.CurrentThread.ManagedThreadId;
                var tskId = Task.CurrentId;

                return z;
            });
            // 100
            var result = q.Fire();

            if (q.HassError)
            {
                // handle and log errors whie execution
            }

            var last = result.Sum;
            var doneAt = result.Dates.Last();
            var initializer = mdl;

            var id0 = Thread.CurrentContext.ContextID;
            var tid0 = Thread.CurrentThread.ManagedThreadId;
            var tskId0 = Task.CurrentId;


            var zz = last;
        }
    }
}
