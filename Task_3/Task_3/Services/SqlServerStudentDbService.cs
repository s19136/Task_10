using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Task_3.DTO.Request;
using Task_3.DTO.Respose;
using Task_3.Entities;

namespace Task_3.Services
{
    public class SqlServerStudentDbService : IStudentServiceDb
    {
        public StudentListResponse getStudents(StudentContext context)
        {
            return new StudentListResponse
            {
                students = context.Student.ToList(),
                Error = ""
            };
        }

        public StudentListResponse addStudent(StudentContext context, Student student)
        {
            if (context.Student.Any(s => s.IndexNumber == student.IndexNumber))
            {
                return new StudentListResponse
                {
                    students = null,
                    Error = "There already is student with this index"
                };
            }
            context.Add<Student>(student);
            context.SaveChanges();
            return getStudents(context);
        }

        public StudentListResponse modifyStudent(StudentContext context, Student student)
        {
            if (!context.Student.Any(s => s.IndexNumber == student.IndexNumber))
            {
                return new StudentListResponse
                {
                    students = null,
                    Error = "There are no students with this id"
                };
            }
            context.Entry(student).State = EntityState.Modified;
            context.SaveChanges();
            return getStudents(context);
        }


        public StudentListResponse deleteStudent(StudentContext context, string id)
        {
            if (!context.Student.Any(s => s.IndexNumber == id))
            {
                return new StudentListResponse
                {
                    students = null,
                    Error = "There are no students with this id"
                };
            }
            var stud = context.Student.First(s => s.IndexNumber == id);
            context.Entry(stud).State = EntityState.Deleted;
            context.SaveChanges();
            return getStudents(context);
        }

        public StudentServiceResponse EnrollStudent(StudentContext context, EnrollStudentRequest request)
        {
            try
            {

                if (!context.Studies.Any(s => s.Name == request.Studies)) //Check if studies exists
                {
                    return new StudentServiceResponse
                    {
                        studentResponse = null,
                        Error = "No such studies"
                    };
                }
                var studies = context.Studies.First(s => s.Name == request.Studies);
                var IdStudy = studies.IdStudy;

                var IdEnrollment = 1;
                var enrollment_available = (from enroll in context.Enrollment
                                  join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                                  where enroll.Semester == 1 && studi.Name == request.Studies
                                  select new { enroll, studi }).Any(); 

                if (!enrollment_available) // Check if Enrollment with semester = 1 exists for these studies
                {
                    var maxId = context.Enrollment.Max(e => e.IdEnrollment);
                    IdEnrollment =maxId + 1; //take IdEnrollment that we created
                    context.Add<Enrollment>(new Enrollment
                    {
                        IdEnrollment = IdEnrollment,
                        IdStudy = IdStudy,
                        Semester = 1,
                        StartDate = DateTime.Now
                    });
                }
                else
                {
                    IdEnrollment = (from enroll in context.Enrollment
                                    join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                                    where enroll.Semester == 1 && studi.Name == request.Studies
                                    select new { enroll, studi }).First().enroll.IdEnrollment; //take existing IdEnrollment to insert in Student later
                }

                var student = context.Student.Any(s => s.IndexNumber == request.IndexNumber);
                if (student) //Check if there is already student with this index number
                {
                    return new StudentServiceResponse
                    {
                        studentResponse = null,
                        Error = "There already is student with this index"
                    };
                }

                context.Add<Student>(new Student
                {
                    IndexNumber = request.IndexNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BirthDate = DateTime.Parse(request.BirthDate),
                    IdEnrollment = IdEnrollment,
                    PassW = "dummy"

                });
                context.SaveChanges();

                return new StudentServiceResponse
                {
                    studentResponse = context.Enrollment.First(e => e.IdEnrollment == IdEnrollment),
                    Error = ""
                };
            }
            catch (Exception e)
            {
                return new StudentServiceResponse
                {
                    studentResponse = null,
                    Error = e.ToString()
                };
            }
        }


        public StudentServiceResponse Promote(StudentContext context, PromoteStudentRequest request)
        {
            try
            {
                var enrollment_present = (from enroll in context.Enrollment
                                          join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                                          where enroll.Semester == (request.Semester) && studi.Name == request.Studies
                                          select new { enroll, studi }).Any();

                if (!enrollment_present) //Check if studies exists
                {
                    return new StudentServiceResponse
                    {
                        studentResponse = null,
                        Error = "No such record in Enrollment"
                    };
                }

                var nextEnrollment = (from enroll in context.Enrollment
                                      join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                                      where enroll.Semester == (request.Semester + 1) && studi.Name == request.Studies
                                      select new { enroll, studi }).Any();
                var IdNextEnrollment = 0;
                if (!nextEnrollment) // check if there is enrollment with this study and semester + 1
                {
                    var maxId = context.Enrollment.Max(e => e.IdEnrollment);
                    var needed_study = context.Studies.First(s => s.Name == request.Studies);
                    context.Add<Enrollment>(new Enrollment
                    {
                        IdEnrollment = maxId + 1,
                        IdStudy = needed_study.IdStudy,
                        Semester = (request.Semester) + 1,
                        StartDate = DateTime.Now
                    });
                    IdNextEnrollment = maxId + 1;
                }
                else
                {
                    IdNextEnrollment = (from enroll in context.Enrollment
                                        join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                                        where enroll.Semester == (request.Semester + 1) && studi.Name == request.Studies
                                        select new { enroll, studi }).First().enroll.IdEnrollment;
                }

                var students = from enroll in context.Enrollment
                               join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                               join student in context.Student on enroll.IdEnrollment equals student.IdEnrollment
                               where enroll.Semester == (request.Semester) && studi.Name == request.Studies
                               select student;
                foreach (Student student in students) // changing enrollment for students
                {
                    student.IdEnrollment = IdNextEnrollment;
                }
                context.SaveChanges();

                var new_enrollment = (from enroll in context.Enrollment
                                      join studi in context.Studies on enroll.IdStudy equals studi.IdStudy
                                      where enroll.Semester == (request.Semester) + 1 && studi.Name == request.Studies
                                      select new { enroll, studi }).First();

                return new StudentServiceResponse
                {
                    studentResponse = new_enrollment.enroll,
                    Error = ""
                };
            }
            catch (Exception e)
            {
                return new StudentServiceResponse
                {
                    studentResponse = null,
                    Error = e.ToString()
                };
            }

        }

    }
}
