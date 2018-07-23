using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KinectCandD
{
    static class Settings
    {
        public static IPEndPoint remoteHost= new IPEndPoint(IPAddress.Parse("127.0.0.1"),6123);
    }
}
