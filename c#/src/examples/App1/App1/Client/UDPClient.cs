using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Net.Sockets;
using System.Diagnostics;
using MsgPack;
using MsgPack.Serialization;

namespace App1.Client
{
    class UDPClient
    {
        DatagramSocket socket;
        Queue<double[]> dgrams;
        MessagePackSerializer serializer;

        public int available { get { return this.dgrams.Count; }}
        public UDPClient()
        {
            Debug.WriteLine("Created new UDPClient");
            this.socket = new DatagramSocket();
            this.dgrams = new Queue<double[]>();
            this.serializer = SerializationContext.Default.GetSerializer<MessagePackObject>();
        }

        public async void Bind()
        {
            Debug.WriteLine("Bound to server port");
            await this.socket.BindServiceNameAsync(Settings.serverPort);
        }

        public async void Bind(string serverPort)
        {
            Debug.WriteLine("Bound to server port");
            await this.socket.BindServiceNameAsync(serverPort);       
        }

        public void Listen()
        {
            Debug.WriteLine("Listening ....");
            socket.MessageReceived += Socket_MessageReceived;
        }

        public double[] Receive()
        {
            return this.dgrams.Dequeue();
        }

        private void Socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            Stream streamIn = args.GetDataStream().AsStreamForRead();
            MessagePackObject msg = (MessagePackObject)this.serializer.Unpack(streamIn);
            MessagePackObject[] msgPackGP = msg.AsEnumerable().ToArray();
            double[] gp = new double[2];
            gp[0] = (double)msgPackGP[0];
            gp[1] = (double)msgPackGP[1];
            this.dgrams.Enqueue(gp);
        }
    }
}
