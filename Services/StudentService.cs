using Microsoft.AspNetCore.Mvc;
using UnivRoomAPI.Interface;
using UnivRoomAPI.Models;

namespace UnivRoomAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task AllocateRoomsAsync()
        {
            Console.WriteLine("Allocating rooms...");
            var student = await _studentRepository.GetNextStudentAsync();
            if (student == null) return; // No students left to allocate

            // Check if the student has a mutual roommate preference
            if (!string.IsNullOrEmpty(student.RoommateId))
            {
                // Fetch the roommate based on student.RoommateId instead of getting the next student
                var roommate = await _studentRepository.GetStudentByIdAsync(student.RoommateId);

                if (roommate != null && roommate.StudentId == student.RoommateId)
                {
                    // Both want to be roommates, try to allocate them together
                    int? roomId = await _studentRepository.FindAvailableRoomAsync(student.Gender, student.RoomTypePreference);

                    if (roomId.HasValue)
                    {
                        Console.WriteLine($"Allocating room {roomId} to students {student.StudentId} and {roommate.StudentId}");
                        await _studentRepository.AllocateRoomAsync(student.StudentId, roomId.Value, "L");
                        await _studentRepository.AllocateRoomAsync(roommate.StudentId, roomId.Value, "R");
                        await _studentRepository.DeleteStudentPreferenceAsync(student.StudentId);
                        await _studentRepository.DeleteStudentPreferenceAsync(roommate.StudentId);
                        return;
                    }
                }
            }

            // If no mutual preference OR preference could not be satisfied, assign the student
            RoomAvailability? availableRoom = await _studentRepository.FindAvailablePartialRoomAsync(student.Gender, student.RoomTypePreference);

            if (availableRoom!= null)
            {
                Console.WriteLine($"Allocating room {availableRoom} to student {student.StudentId}");
                // Assign student to the available room
                await _studentRepository.AllocateRoomAsync(student.StudentId, availableRoom.RoomId, availableRoom.BedName);
            }
            else
            {
                // Fallback: Assign to any gender-appropriate room
                int? room = await _studentRepository.FindAvailableRoomAsync(student.Gender, null);
                
                if (room.HasValue)
                {
                    await _studentRepository.AllocateRoomAsync(student.StudentId, room.Value, "L");
                }
            }

            // Remove student preference after assignment
            await _studentRepository.DeleteStudentPreferenceAsync(student.StudentId);
        }

        public async Task<IEnumerable<dynamic>> FinalRoomAllocationsAsync()
        {
             Console.WriteLine("Fetching final room allocations...");
                return await _studentRepository.FinalRoomAllocationsAsync();
        }

   


        public async Task<IEnumerable<dynamic>> FinalRoomAllocationsByIdAsync(string studentId)
        {
            return await _studentRepository.FinalRoomAllocationsByIdAsync(studentId);
        }
        public async Task<bool> AddStudentPreferenceAsync(StudentResHallPref studentPref)
        {
            try
            {
                // Call the repository to add the student preference
                return await _studentRepository.AddStudentPreferenceAsync(studentPref);
            }
            catch (Exception)
            {
                // Handle the exception, log it if needed
                return false;
            }
        }


    }
}
