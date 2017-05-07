using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBS.SequentialTask
{
    public class ConcurrencyHandler<T> where T : BaseHandlerModel
    {
        private T _initializer;

        public ConcurrencyHandler(T initializer)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }
            _initializer = initializer;

        }



        private int index = 0;
        Dictionary<int, Func<T, T>> queue = new Dictionary<int, Func<T, T>>();
        private object lockMe = new object();
        List<ErrorModel> errors = new List<ErrorModel>();
        /// <summary>
        /// آیا خطا دارد
        /// </summary>
        public bool HassError => errors.Count > 0;
        /// <summary>
        /// لیست خطاها
        /// </summary>
        public List<ErrorModel> ErrorList => errors;
        /// <summary>
        /// افزودن وظیفه به صف و زنجیره وظایف
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>شاخص و شمارنده ی عملیات</returns>
        public int RegisterTaskToQueue(Func<T, T> operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }
            var j = index;
            queue.Add(index++, operation);
            return j;
        }
        /// <summary>
        /// اجرای متوالی
        /// </summary>
        /// <returns></returns>
        public T Fire()
        {
            if (queue.Count == 0)
            {
                return _initializer;
            }

            if (queue.Count == 1)
            {
                try
                {
                    var r = queue[0](_initializer);
                    return r;
                }
                catch (Exception ex)
                {
                    errors.Add(new ErrorModel
                    {
                        Error = ex,
                        OperationIndex = 0,
                        TaskId = Task.CurrentId
                    });
                    return _initializer;
                }
            }
            int counter = 0;
            Task<T> t = Task.Run<T>(() =>
            {
                if (!_initializer.ContinueWithError && HassError)
                {
                    return _initializer;
                }

                try
                {
                    var r = queue[0](_initializer);
                    return r;
                }
                catch (Exception ex)
                {
                    errors.Add(new ErrorModel { Error = ex, OperationIndex = 0, TaskId = Task.CurrentId });
                    return _initializer;
                }
            });
            counter++;

            Task<T> tmp = t;
            while (counter < index)
            {
                var i = counter;

                var current = tmp.ContinueWith((lastStep) =>
                {
                    var r = lastStep.Result;

                    if (!r.ContinueWithError && HassError)
                    {
                        return r;
                    }

                    try
                    {
                        var x = queue[i](r);
                        return x;
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new ErrorModel { Error = ex, OperationIndex = i, TaskId = Task.CurrentId });
                        return r;
                    }
                });
                counter++;
                tmp = current;
            }
            Task.WaitAll(tmp);
            return tmp.Result;
        }




    }
}
