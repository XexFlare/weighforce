using System;
using System.Reactive.Subjects;

namespace WeighForce.Services
{
    public readonly struct StatusType
    {
        public const string SUCCESS = "success";
        public const string ERROR = "error";
        public const string WARNING = "warning";
        public const string INFO = "info";

    }
    public class Status
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }
    public class SyncStatus
    {
        public Status Status;
        public bool Log { get; set; }
        private readonly Subject<Status> _status = new Subject<Status>();

        public SyncStatus(bool log = false)
        {
            UpdateStatus(new Status { });
            Log = log;
        }
        public Status UpdateStatus(Status status)
        {
            if (Status != status)
            {
                Status = status;
                if (Log)
                    System.Console.WriteLine(status.Message);
                _status.OnNext(Status);
            }
            return Status;
        }

        public IObservable<Status> StreamStatus()
        {
            return _status;
        }
    }
}
