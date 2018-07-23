using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace GazeCollector
{
    public class Settings
    {
        //machines that are streaming the gaze data
        private static ISet<IPEndPoint> streamerEndPoints = new HashSet<IPEndPoint>();
        //machines that want the gaze data
        private static ISet<IPEndPoint> serverEndPoints = new HashSet<IPEndPoint>();
        //list of gazeStreamer connections
        private static ISet<GazeStreamer> gazeStreamers = new HashSet<GazeStreamer>();

        public static int numberOfThreads { get; set; }

        public static IEnumerable<IPEndPoint> GetListOfStreamerEndPoints()
        {
            return streamerEndPoints;
        }

        public static IEnumerable<IPEndPoint> GetListOfServerEndPoints()
        {
            return serverEndPoints;
        }

        public static IEnumerable<GazeStreamer> GetListOfConnections()
        {
            return gazeStreamers;
        }

        public static void AddStreamerEndPoint(String ip, String port)
        {
            streamerEndPoints.Add(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));
        }

        public static void AddServerEndPoint(String ip, String port)
        {
            serverEndPoints.Add(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));
        }

        public static void AddGazeStreamer(GazeStreamer gs)
        {
           gazeStreamers.Add(gs);
        }
    }
}
