﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsumerApp.Models;


namespace ConsumerApp.Interfaces
{
    public interface IDatabase
    {
        public void InsertMessage(Message message);
    }
}
