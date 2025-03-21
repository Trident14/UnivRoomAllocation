using Microsoft.AspNetCore.Mvc;
using UnivRoomAPI.Models;
namespace UnivRoomAPI.Interface
{
    public interface IStudentRepository
    {
        Task<StudentResHallPref?> GetNextStudentAsync();
        Task<bool> CheckRoomAvailabilityAsync(int roomId);
        Task<int?> FindAvailableRoomAsync(string gender, string? roomType);
        Task AllocateRoomAsync(string studentId, int roomId, string bedName); 
        Task DeleteStudentPreferenceAsync(string studentId); 
        Task<StudentResHallPref?> GetStudentByIdAsync(string studentId);
        Task<RoomAvailability?> FindAvailablePartialRoomAsync(string gender, string? roomTypePreference);
        Task<IEnumerable<dynamic>> FinalRoomAllocationsAsync();
        Task<IEnumerable<dynamic>> FinalRoomAllocationsByIdAsync(string studentId);
        Task<bool> AddStudentPreferenceAsync(StudentResHallPref studentPref);
    }
}
