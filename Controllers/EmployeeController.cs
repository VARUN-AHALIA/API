using System.Reflection.Metadata.Ecma335;
using System.Windows.Input;
using Apiexample1.Models;
using Bussiness.Services;
using DataAccess.Context;
using DataAccess.Repositery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Apiexample1.Controllers
{
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeRepo _repo;
        private readonly EmployeeServices _service;
        private readonly Datacontext _context;

        public EmployeeController(EmployeeServices service, EmployeeRepo repo, Datacontext context)
        {
            _repo = repo;
            _service = service;
            _context = context;
        }
        [HttpGet("GetName/{name}", Name = "Getname")]
        public async Task<dynamic> Getname([FromRoute] string name)
        {
            var res = await _service.GetnameService(name);
            return res;
        }

        [HttpPost("PostEmployee", Name = "postEmployee")]

        public async Task<IActionResult> PostEmployee([FromBody] EmployeeModel employee)
        {
            if (employee == null || string.IsNullOrWhiteSpace(employee.Name))
            {
                return BadRequest("Invalid employee data");
            }
            var cmd = "insert into emp(id,Name,Designation,Department) values (@id,@name,@Designation,@Department)";
            using (var connection = _context.createConnectionEmployee())
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(cmd, connection);

                command.Parameters.AddWithValue("@id", employee.Id);
                command.Parameters.AddWithValue("@name", employee.Name);
                command.Parameters.AddWithValue("Designation", employee.Designation);
                command.Parameters.AddWithValue("@Department", employee.Department);
                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return CreatedAtAction(nameof(GetEmpController), new { name = employee.Name},employee);
                }
                else
                {
                    return BadRequest("Failed to insert employee");
                }

            }
            return Ok(employee);
        }
        [HttpGet("GetEmpController/{id}", Name = "GetEmpController")]
        public async Task<IActionResult> GetEmpController([FromRoute] int id)
        {
            var cmd = "SELECT * FROM emp WHERE id = @id";

            var employees = new List<EmployeeModel>();

            using (var connection = _context.createConnectionEmployee())
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(cmd, connection);

                command.Parameters.AddWithValue("@id", id); // Use the id parameter from the route

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var employee = new EmployeeModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Designation = reader.GetString(reader.GetOrdinal("Designation")),
                            Department = reader.GetString(reader.GetOrdinal("Department"))
                        };
                        employees.Add(employee);
                    }
                }
            }

            if (employees.Count == 0)
            {
                return NotFound(); // Return 404 if no employee is found
            }

            return Ok(employees);
        }

        [HttpGet("GetDeptController", Name = "GetDeptController")]
        public async Task<IActionResult> GetDeptController()
        {
            var cmd = "select * from emp";


            var employees = new List<EmployeeModel>();


            using (var connection = _context.createConnectionEmployee())
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(cmd, connection);


                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var employee = new EmployeeModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),

                            Designation = reader.GetString(reader.GetOrdinal("Designation")),
                            Department = reader.GetString(reader.GetOrdinal("Department"))
                        };
                        employees.Add(employee);
                    }
                }
            }

            return Ok(employees);



        }
        [HttpGet("GetALLEMP", Name = "GetALLEMP")]
        public async Task<IActionResult> GetALLEMP()
        {
            var cmd = "SELECT id, name FROM emp";

            var employees = new List<EmployeeModel>();

            using (var connection = _context.createConnectionEmployee())
            {
                await connection.OpenAsync();
                using var command = new SqlCommand(cmd, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var employee = new EmployeeModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        };
                        employees.Add(employee);
                    }
                }
            }

            return Ok(employees);
        }

    }
}
