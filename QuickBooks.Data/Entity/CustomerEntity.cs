using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickBooks.Data.Entity
{
    public class CustomerEntity
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool? IsActive { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double? Balance { get; set; }
        public string CompanyName { get; set; }
        public string EditSequence { get; set; }
    }
}
