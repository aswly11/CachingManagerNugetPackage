using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CachingManager.Models
{
    public class CachingManagerOptions
    {
        public string ConnectionString { get; set; }
        public TimeSpan? ExpiresIn { get; set; }
    }
}
