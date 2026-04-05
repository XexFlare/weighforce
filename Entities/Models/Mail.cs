namespace WeighForce.Models
{
    static class AlertType
    {
        public const string BOOKINGS = "Bookings";
        public const string REPORTS = "Reports";
        public const string WEEKLY_SYNC = "Weekly Sync";
        public const string TOLERANCE = "Tolerance";
        public const string TRAIN = "Trains";
    }
    public class UserMail : Syncable
    {
        public ApplicationUser User { get; set; }
        public string Alert { get; set; }
        public Office Office { get; set; }
    }
    
    public class MailingList 
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name{ get; set; }
        public string Alert { get; set; }
        public Office Office { get; set; }
    }
}