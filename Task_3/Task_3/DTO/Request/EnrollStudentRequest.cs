using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Task_3.DTO.Request
{
    public class EnrollStudentRequest
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)[0-9][0-9]$")]
        //mm-dd-yyyy | mm.dd.yyyy | mm/dd/yyyy format
        public string BirthDate { get; set; }
        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }
    }
}
