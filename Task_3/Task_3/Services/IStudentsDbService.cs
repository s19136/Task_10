using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_3.DTO.Request;
using Task_3.DTO.Respose;
using Task_3.Entities;

namespace Task_3.Services
{
    public interface IStudentServiceDb
    {
        StudentListResponse getStudents(StudentContext context);
        StudentListResponse addStudent(StudentContext context, Student student);
        StudentListResponse modifyStudent(StudentContext context, Student student);
        StudentListResponse deleteStudent(StudentContext context, string id);
        StudentServiceResponse EnrollStudent(StudentContext context, EnrollStudentRequest req);
        StudentServiceResponse Promote(StudentContext context, PromoteStudentRequest req);
    }
}
