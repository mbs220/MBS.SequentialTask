using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBS.SequentialTask
{
    public class BaseHandlerModel
    {
        public BaseHandlerModel()
        {
            CreateDate = DateTime.Now;
            
        }

        public DateTime CreateDate { get;  }
        public bool ContinueWithError { get; set; } = false;

    }
}
