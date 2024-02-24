using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerApp
{
    public class Message
    {
        public int Id { get; set; }
        public int Counter { get; set; }
        public int Time { get; set; }
    }
}
