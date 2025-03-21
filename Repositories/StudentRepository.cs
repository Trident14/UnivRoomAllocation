using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Data;
using UnivRoomAPI.Interface;
using UnivRoomAPI.Models;

namespace UnivRoomAPI.Repositories  
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbConnection _db;

        public StudentRepository(IDbConnection db)
        {
            _db = db;
        }

        // Get the next student based on First-Come-First-Serve (earliest created time)
        public async Task<StudentResHallPref?> GetNextStudentAsync()
        {
            Console.WriteLine("Fetching next student...");
            var query = @"
                SELECT * FROM ""student_reshall_pref"" 
                ORDER BY createdat ASC 
                LIMIT 1;";
            
            return  await _db.QueryFirstOrDefaultAsync<StudentResHallPref>(query);
        }

        // Get student by student ID
        public async Task<StudentResHallPref?> GetStudentByIdAsync(string studentId)
        {
            var query = @"
                SELECT * FROM ""student_reshall_pref"" 
                WHERE studentid = @StudentId 
                LIMIT 1;";

            return await _db.QueryFirstOrDefaultAsync<StudentResHallPref>(query, new { StudentId = studentId });
        }


        // Check if the given room has an empty bed
        public async Task<bool> CheckRoomAvailabilityAsync(int roomId)
        {
            var query = @"
                SELECT COUNT(*) FROM ""room_allocation""
                WHERE roomid = @RoomId AND studentid IS NULL;";
            
            int count = await _db.ExecuteScalarAsync<int>(query, new { RoomId = roomId });
            return count > 0;
        }

        // Find a room that fits the gender and room type preference
       public async Task<int?> FindAvailableRoomAsync(string gender, string? roomType)
        {
            var query = @"
                SELECT roomid 
                FROM get_available_rooms(@Gender, @RoomType) 
                LIMIT 1;"; // Get the first available room

            return await _db.ExecuteScalarAsync<int?>(query, new { Gender = gender, RoomType = roomType });
        }

        //Find partial room 
      public async Task<RoomAvailability?> FindAvailablePartialRoomAsync(string gender, string? roomTypePreference)
        {
            var query = @"
                SELECT roomid, bedname
                FROM get_available_rooms_partial(@Gender, @RoomType) 
                LIMIT 1;";  // Ensure we only fetch one available room

            return await _db.QueryFirstOrDefaultAsync<RoomAvailability>(
                query, new { Gender = gender, RoomType = roomTypePreference });
        }



        // Insert if no allocation exists, otherwise update
        public async Task AllocateRoomAsync(string studentId, int roomId, string bedName)
        {
            Console.WriteLine($"Allocating room {roomId} to student {studentId} on bed {bedName}");
            var query = @"
                INSERT INTO ""room_allocation"" (roomid, bedname, studentid)
                VALUES (@RoomId, @BedName, @StudentId)
                ON CONFLICT (roomid, bedname) 
                DO UPDATE SET studentid = @StudentId;";

            await _db.ExecuteAsync(query, new { StudentId = studentId, RoomId = roomId, BedName = bedName });
        }

        // Delete student preference after allocation
        public async Task DeleteStudentPreferenceAsync(string studentId)
        {
            var query = "CALL delete_student_preference(@StudentId);";
            await _db.ExecuteAsync(query, new { StudentId = studentId });
        }

        public async Task<IEnumerable<dynamic>> FinalRoomAllocationsAsync()
        {
            // Step 1: Check if room_master has data
            var roomCount = await _db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM room_master");

            // Step 2: If there are rooms, call the stored procedure
            if (roomCount > 0)
            {
                var result = await _db.QueryAsync("sp_insert_room_allocations", commandType: CommandType.StoredProcedure);
                var query = @"SELECT * FROM ""room_allocation_result"";";
                var finalResult = await _db.QueryAsync(query);
              
                return finalResult;
            }

            // Step 4: If no rooms exist, return a message (can be customized further)
            return new List<dynamic> { new { message = "No rooms available in room_master" } };
        }

        public async Task<IEnumerable<dynamic>> FinalRoomAllocationsByIdAsync(string studentId)
        {
            var query = @"SELECT * FROM ""room_allocation_result"" WHERE studentid = @StudentId::character varying OR roommateid = @StudentId;";
            // Await the asynchronous query to get the result
            var result = await _db.QueryAsync(query, new { StudentId = studentId });
            return result;
        }

   public async Task<bool> AddStudentPreferenceAsync(StudentResHallPref studentPref)
    {
        try
        {
            var query = @"
                CALL insert_student_preference(
                    @StudentId, 
                    @RoommateId, 
                    @Gender, 
                    @EmailAddress, 
                    @RoomTypePreference
                );";

            // Execute the stored procedure
            var result = await _db.ExecuteAsync(query, new
            {
                studentPref.StudentId,
                studentPref.RoommateId,
                studentPref.Gender,
                studentPref.EmailAddress,
                studentPref.RoomTypePreference,
            });

            // Query the database to check if the preference was added
            var query1 = @"
                SELECT * FROM ""student_reshall_pref"" 
                WHERE studentid = @StudentId 
                LIMIT 1;";

            var result1 = await _db.QueryFirstOrDefaultAsync<StudentResHallPref>(query1, new { StudentId = studentPref.StudentId });

            // If we successfully fetched a result, it means the preference was inserted correctly
            if (result1 != null)
            {
                Console.WriteLine($"Student preference added: {result}");
                return true;
            }
            else
            {
                Console.WriteLine("Failed to fetch the inserted student preference.");
                return false;
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    }
}
