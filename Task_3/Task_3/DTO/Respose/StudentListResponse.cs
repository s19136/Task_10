using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_3.Entities;

namespace Task_3.DTO.Respose
{
    public class StudentListResponse
    {
        public List<Student> students { get; set; }
        public string Error { get; set; }
    }
}
