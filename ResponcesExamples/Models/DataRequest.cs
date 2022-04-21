using System.ComponentModel.DataAnnotations;

namespace ResponcesExamples.Models
{
    public class DataRequest
    {
        [Required]
        [MinLength(2)]
        public string DataItem1 { get; set; }

        [Required]
        [MinLength(4)]
        public string DataItem2 { get; set; }
    }

    
    public class DataResponse
    {
        public string DataItem1 { get; set; }

        public string DataItem2 { get; set; }
    }
}
