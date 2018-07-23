using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Diagnostics;

namespace GazeCollector
{
    public class Server
    {
        public DataStorage dataStore;

        public Server()
        {
            this.dataStore = new DataStorage();
        }

        public void StartServer(MainWindow window)
        {
            StartKinectServer();
            StartPupilServer(window);
        }

        private void StartPupilServer(MainWindow window)
        {
            Settings.numberOfThreads = 0;

            //Add IP and Ports of streamers.    
            Settings.AddStreamerEndPoint("10.100.230.186", "50020");
            //Settings.AddStreamerEndPoint("10.100.229.224", "50020");

            //Add IP and Ports of servers that need the gaze data if sending data to other machines.

            //Create and object to WPF and let each thread access it,
            //each thread creates a circle* which represents the users gaze point 
            // and manipulates ONLY the circle that it created.

            foreach (IPEndPoint ipep in Settings.GetListOfStreamerEndPoints())
            {
                Shape obj = window.CreateFilledCircle();
                GazeStreamer gs = new GazeStreamer(ipep, window, obj);
                Thread newThread = new Thread(gs.StartGazeStreamer);
                newThread.SetApartmentState(ApartmentState.STA);
                Settings.AddGazeStreamer(gs);
                newThread.Start();
            }
        }

        private void StartKinectServer()
        {
            KinectServer kn = new KinectServer(this.dataStore);
            Thread newThread = new Thread(kn.StartListener);
            newThread.Start();
        }
    }
}
