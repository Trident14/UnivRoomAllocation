namespace UnivRoomAPI.Models
{
    public class StudentResHallPref
    {
        public int PreferenceId { get; set; } // This will be auto-generated in the database
        public string StudentId { get; set; } 
        public string? RoommateId { get; set; }
        public string Gender { get; set; }
        public string? RoomTypePreference { get; set; }
        public string? EmailAddress { get; set; }  
        public DateTime CreatedTime { get; set; } 
    }
}
