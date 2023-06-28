using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PizzaOnline.Dal.Entities
{
    [Flags]
    public enum StuffedCrust
    {
        Normal = 1,
        Cheese = 2,
        Beacon = 3,
        Jalapeno = 4,
    }
}
