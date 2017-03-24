using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MojCRM.Models
{
    [Table("LogError")]
    public class LogError
    {
        [Key]
        public int Id { get; set; }
        public string Method { get; set; }
        public string Parameters { get; set; }
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string Request { get; set; }
        public string User { get; set; }
        public DateTime InsertDate { get; set; }
    }
}