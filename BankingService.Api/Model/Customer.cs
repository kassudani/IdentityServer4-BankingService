using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Api.Model
{
    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
