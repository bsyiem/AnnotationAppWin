using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MsgPack;
using MsgPack.Serialization;
using System.Diagnostics;
using Entity;

namespace GazeCollector
{
    class KinectServer
    {
        DataStorage dataStore;

        MessagePackSerializer serializer = SerializationContext.Default.GetSerializer<List<Surface>>();
        //MessagePackSerializer mobjserializer = SerializationContext.Default.GetSerializer<MessagePackObject>();

        public KinectServer(DataStorage dataStore)
        {
            this.dataStore = dataStore;
        }

        public void StartListener()
        {
            bool done = false;

            IPEndPoint KinectIP = new IPEndPoint(IPAddress.Any, KinectSettings.listenPort);
            UdpClient listener = new UdpClient(KinectSettings.listenPort);
            try
            {
                Trace.WriteLine("Listening");
                while (!done)
                {
                    if(listener.Available > 0)
                    {
                        byte[] msgBytes = listener.Receive(ref KinectIP);
                        Trace.WriteLine("msgLength = " + msgBytes.Length);
                        List<Surface> msg = (List<Surface>)serializer.UnpackSingleObject(msgBytes);

                        //MessagePackObject mobj = (MessagePackObject)serializer.UnpackSingleObject(msgBytes);
                        //testing
                        //Trace.WriteLine(mobj);
                        //PrintSurfaces(msg);
                        UpdateDataStore(msg);
                    }


                }
            }catch(Exception e)
            {
                Trace.WriteLine(e);
            }
           
        }

        //public void processQueue()
        //{
        //    throw new NotImplementedException();
        //}

        //for testing 

        private void UpdateDataStore(List<Surface> surfaces)
        {
            foreach(Surface surface in surfaces)
                this.dataStore.ModifySurface(surface.i, surface.v);
        }
        private void PrintSurfaces(List<Surface> surfaces)
        {
            foreach(Surface surface in surfaces)
            {
                Trace.WriteLine(surface.i);                
            }
        }
    }
}
