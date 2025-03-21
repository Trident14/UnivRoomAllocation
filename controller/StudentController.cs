using Microsoft.AspNetCore.Mvc;

using UnivRoomAPI.Interface;
using UnivRoomAPI.Models;

namespace UnivRoomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("allocate")]
        public async Task<IActionResult> AllocateRooms()
        {
            Console.WriteLine("Allocating rooms...");
            await _studentService.AllocateRoomsAsync();
            return Ok("Room allocation completed.");
        }
        
        [HttpGet("FinalAllocateRooms")]
        public async Task<IActionResult> GetFinalRoomAllocations()
        {
            Console.WriteLine("Fetching final room allocations...");
            var result = await _studentService.FinalRoomAllocationsAsync();
            return Ok(result);
        }

        [HttpGet("FinalAllocateRoomsById")]
        public async Task<IActionResult> GetFinalRoomAllocationsById(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return BadRequest("Student ID is required.");
            }

            Console.WriteLine("Fetching final room allocations by student ID...");
            var result = await _studentService.FinalRoomAllocationsByIdAsync(studentId);
            
            if (result == null || !result.Any())
            {
                return NotFound("No room allocations found for the given student ID.");
            }

            return Ok(result);
        }

        [HttpPost("addStudentPreference")]
        public async Task<IActionResult> AddStudentPreference([FromBody] StudentResHallPref studentPref)
        {
            if (studentPref == null)
            {
                return BadRequest("Student preference data is required.");
            }

            try
            {
                // Call the service layer to handle data insertion
                var result = await _studentService.AddStudentPreferenceAsync(studentPref);

                if (result)
                {
                    return Ok("Student preference added successfully.");
                }
                else
                {
                    return BadRequest("Failed to add student preference.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        
    }
}
