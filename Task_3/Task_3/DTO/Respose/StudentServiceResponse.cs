using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task_3.Entities;

namespace Task_3.DTO.Respose
{
    public class StudentServiceResponse
    {
        public Enrollment studentResponse { get; set; }
        public string Error { get; set; }
    }
}
