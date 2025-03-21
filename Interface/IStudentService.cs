using Microsoft.AspNetCore.Mvc;
using UnivRoomAPI.Models;

namespace UnivRoomAPI.Interface
{
    public interface IStudentService
    {
        Task AllocateRoomsAsync();
        Task<IEnumerable<dynamic>> FinalRoomAllocationsAsync();
        Task<IEnumerable<dynamic>> FinalRoomAllocationsByIdAsync(string studentId);
        Task<bool> AddStudentPreferenceAsync(StudentResHallPref studentPref);
        
    }

}
