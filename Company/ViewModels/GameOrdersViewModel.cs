using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Company.Models;
using System.Collections;

namespace Company.ViewModels
{
    public class GameOrdersViewModel
    {
        public IEnumerable<Orders> OrdersVME { get; set; }
        public Game GameVM { get; set; }
        public Orders OrdersVM { get; set; }
    }
}