using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeridianTcp
{
    public class Response
    {
        public DateTime Data1;
        public DateTime Data2;

        public Response(DateTime data1, DateTime data2)
        {
            Data1 = data1;
            Data2 = data2;
        }
    }
}
