using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Task_3.DTO.Request;
using Task_3.DTO.Respose;
using Task_3.Entities;
using Task_3.Services;

namespace Task_3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentServiceDb _service;
        private readonly IConfiguration _configuration;
        private readonly StudentContext _context;

        public EnrollmentsController(IStudentServiceDb service, IConfiguration configuration, StudentContext context)
        {
            _service = service;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public IActionResult getStudents()
        {
            return Ok(_service.getStudents(_context));
        }


        [HttpPost]
        public IActionResult addStudent(Student student)
        {
            var res = _service.addStudent(_context, student);
            if (res.Error == "")
                return Ok(res.students);
            else
                return BadRequest(res.Error);
        }

        [HttpPut]
        public IActionResult modifyStudent(Student student)
        {
            var res = _service.modifyStudent(_context, student);
            if (res.Error == "")
                return Ok(res.students);
            else
                return BadRequest(res.Error);
        }

        [HttpDelete]
        public IActionResult deleteStudent(string id)
        {
            var res = _service.deleteStudent(_context, id);
            if (res.Error == "")
                return Ok(res.students);
            else
                return BadRequest(res.Error);
        }

        [HttpPost("enroll", Name = nameof(EnrollStudent))]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(_context, request);

            if (response.studentResponse == null)
            {
                switch (response.Error)
                {
                    case "No such studies":
                        return BadRequest("No such studies");
                    case "There already is student with this index":
                        return BadRequest("There already is student with this index");
                    default:
                        return BadRequest(response.Error);
                }
            }
            else
            {
                return CreatedAtAction(nameof(EnrollStudent), response.studentResponse);
            }
            
        }

        [HttpPost("promote",Name = "promote")]
        public IActionResult Promote(PromoteStudentRequest request)
        {
            var response = _service.Promote(_context, request);

            if (response.studentResponse == null)
            {
                switch (response.Error)
                {
                    case "No such record in Enrollment":
                        return NotFound("No such record in Enrollment");
                    default:
                        return StatusCode(400);
                }
            }
            else
            {
                return CreatedAtAction(nameof(EnrollStudent), response.studentResponse);
            }

        }

    }
}