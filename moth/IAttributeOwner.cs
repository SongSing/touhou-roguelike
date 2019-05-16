using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moth
{
    interface IAttributeOwner
    {
        AttributeInfo GetAttribute(string name, Bullet requester);
    }
}
