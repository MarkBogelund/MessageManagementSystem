using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerApp.Models
{
    class Message
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int counter;
    }
}
