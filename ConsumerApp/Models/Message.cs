using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public int Counter { get; set; }
        public int Time { get; set; }
    }
}
