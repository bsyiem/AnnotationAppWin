using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ZeroMQ;
using System.Threading.Tasks;
using MsgPack;
using MsgPack.Serialization;
using System.IO;
using System.Runtime.Remoting;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace GazeCollector
{
    public class GazeStreamer
    {
        private IPEndPoint ipep;
        MainWindow window;
        Shape obj;
        private bool term = false;
        PupilProSurface pupilProSurface;
        //MessagePackSerializer serializer = SerializationContext.Default.GetSerializer<MessagePackObject>();

        public GazeStreamer(IPEndPoint ipep, MainWindow window,Shape obj)
        {
            this.ipep = ipep;
            this.window = window;
            this.obj = obj;
            this.pupilProSurface = new PupilProSurface();
        }

        public void StartGazeStreamer()
        { 
            Settings.numberOfThreads += 1;
            Console.WriteLine("starting thread: "+ Settings.numberOfThreads);
            ISet<Sender> senders = initConnections();

            using (var context = new ZContext())
            using (var subReq = new ZSocket(context, ZSocketType.REQ))
            {
                subReq.Connect("tcp://" + ipep.Address.ToString() + ":" + ipep.Port.ToString());
                Console.WriteLine("Connecting to tcp://" + ipep.Address.ToString() + ":" + ipep.Port.ToString());
                subReq.Send(new ZFrame("SUB_PORT"));

                string subPort = subReq.ReceiveFrame().ReadString();

                using (var gazeStreamer = new ZSocket(context, ZSocketType.SUB))
                {
                    
                    gazeStreamer.Connect("tcp://" + ipep.Address.ToString() + ":" + subPort);
                    gazeStreamer.Subscribe("surface");
                    while (!term)
                    {
                        using (ZMessage message = gazeStreamer.ReceiveMessage())
                        {
                            MessagePackObjectDictionary dict = this.pupilProSurface.GetInitialDictionary(message);

                            //for testing
                            String surfaceName = this.pupilProSurface.GetSurfaceName(dict);
                            Console.WriteLine(surfaceName);
                            
                            try
                            {
                                Gazepoint gp = this.pupilProSurface.GetGazePointOnSurface(dict);
                                this.SetPositionUsingDispatcher(gp.x,gp.y);
                                Console.WriteLine(gp.x + " " + gp.y);
                            }catch(Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            

                            //this.BroadcastExcept(senders,gp);                    
                        }
                    }
                    this.term = true;
                }
            }
        }

        public void SetPositionUsingDispatcher(float x,float y)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                window.SetPosition(this.obj,(float)(x*window.Width),(float)(y*window.Height));
                
            }));

        } 
       
        public ISet<Sender> initConnections()
        {
            ISet<Sender> senders = new HashSet<Sender>();
            foreach (IPEndPoint ep in Settings.GetListOfServerEndPoints())
            {
                Sender sender = new Sender();
                sender.initConnection(ep);
                senders.Add(sender);
            }
            return senders;
        }

        public void BroadcastExcept(ISet<Sender> senders,Object obj)
        {
            foreach(Sender sender in senders)
            {
                sender.Send(obj);
            }
        }
    }
}
