﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerApp
{
    public class Message
    {
        public int counter { get; set; }
        public int unixTime { get; set; }
    }
}