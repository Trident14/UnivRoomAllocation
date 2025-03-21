namespace UnivRoomAPI.Models
{
   public class RoomAllocation
    {
        public int RoomId { get; set; }
        public string BedName { get; set; }  // "L" or "R"
        public string? StudentId { get; set; }  
    }


}