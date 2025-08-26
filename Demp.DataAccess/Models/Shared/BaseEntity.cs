using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Models.Shared
{
    public class BaseEntity
    {
        // PK
        public int Id { get; set; }
        // User Id
        public int CreatedBy { get; set; }
        // date of creation
        public DateTime CreatedOn { get; set; } 
        // User Id
        public int LastModifiedBy { get; set; }
        // date of modification [auto calculated]
        public DateTime LastModifiedOn { get; set; } 
        public bool IsDeleted { get; set; }
    }
}
