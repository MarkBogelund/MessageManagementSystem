using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ConsumerApp.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int counter;
    }
}
