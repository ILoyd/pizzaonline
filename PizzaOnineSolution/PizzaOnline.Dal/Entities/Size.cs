using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PizzaOnline.Dal.Entities
{
    [Flags]
    public enum Size
    {
        Small = 1,
        Medium = 2,
        Large = 3,
    }
}
