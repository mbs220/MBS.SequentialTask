using System;

namespace MBS.SequentialTask
{
    public class ErrorModel
    {
        public Exception Error { get; set; }
        public int? TaskId { get; set; }
        public int OperationIndex { get; set; }

    }
}
