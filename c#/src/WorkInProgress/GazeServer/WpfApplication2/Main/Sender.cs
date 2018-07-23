using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;
using System.Net;
using System.Net.Sockets;
using MsgPack.Serialization;
using MsgPack;
using System.IO;

namespace GazeCollector
{
    public class Sender
    {
        TcpClient clientConn;
        NetworkStream stream;

        MessagePackSerializer serializer = SerializationContext.Default.GetSerializer<Gazepoint>();

        public void initConnection(IPEndPoint target)
        {
            this.clientConn = new TcpClient();
            clientConn.Connect(target);
            this.stream = clientConn.GetStream();
        }

        public void Send(Object obj)
        {
            byte[] byteBuf = serializer.PackSingleObject(obj);
            this.stream.Write(byteBuf,0,byteBuf.Length);                   
        }

    }
}
