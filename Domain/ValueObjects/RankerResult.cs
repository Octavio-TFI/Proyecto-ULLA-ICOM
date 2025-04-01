using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    internal record RankerResult
    {
        public bool Score { get; set; }
    }
}
