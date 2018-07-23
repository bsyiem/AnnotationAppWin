using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MsgPack.Serialization;
using MsgPack;
using System.Diagnostics;
using Entity;

namespace KinectCandD
{
    class Connection
    {
        UdpClient client;
        MessagePackSerializer serializer = SerializationContext.Default.GetSerializer<List<Surface>>();
        //MessagePackSerializer serializer = SerializationContext.Default.GetSerializer<MessagePackObject>();

        public Connection()
        {
            this.client = new UdpClient();
        }

        public void Connect()
        {
            this.client.Connect(Settings.remoteHost);
        }

        public void SendSurfaceDetails(List<Surface> surfaces)
        {
            Trace.WriteLine("Sending");
            foreach(Surface surface in surfaces)
            {
                Trace.WriteLine(surface.i);
            }
            byte[] msg = this.SerializeSurfaces(surfaces);
            this.client.Send(msg,msg.Length);
        }

        private byte[] SerializeSurfaces(List<Surface> surfaces)
        {
            return this.serializer.PackSingleObject(surfaces);
        }
    }
}
