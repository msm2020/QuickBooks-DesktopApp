using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickBooks.Data.Entity
{
    public class QuickBooksXmlEntity
    {
        public string Name { get; set; }
        public string Req { get; set; }
        public List<string> Ret { get; set; }
        public bool RemoveId { get; set; }
        public string Extra { get; set; }
    }
}
