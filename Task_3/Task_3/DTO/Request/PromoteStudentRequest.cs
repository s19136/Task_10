
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Task_3.DTO.Request
{
    public class PromoteStudentRequest
    {
        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }
        [Required]
        public int Semester { get; set; }
    }
}
