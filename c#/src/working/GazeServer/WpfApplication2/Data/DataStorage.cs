using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GazeCollector
{
    public class DataStorage
    {
        Dictionary<IPEndPoint, int> gazeDict;
        Dictionary<int, PointF[]> surfaceDict;
        Dictionary<int, int> markerCountDict;

        public DataStorage()
        {
            this.gazeDict = new Dictionary<IPEndPoint, int>();
            this.surfaceDict = new Dictionary<int, PointF[]>();
            this.markerCountDict = new Dictionary<int, int>();
        }

        // Needs to be revised        
        public void ModifyGaze(IPEndPoint ipEnd, int markerID)
        {
            lock (this.gazeDict)
            {
                if (this.gazeDict.ContainsKey(ipEnd))
                {
                    if(this.gazeDict[ipEnd] != markerID)
                    {
                        this.gazeDict[ipEnd] = markerID;
                        this.markerCountDict[markerID] -= 1;
                    }                   
                }
                else
                {
                    this.gazeDict.Add(ipEnd, markerID);
                    if (this.markerCountDict.ContainsKey(markerID))
                    {
                        this.markerCountDict[markerID] += 1;
                    }
                    else
                    {
                        this.markerCountDict.Add(markerID, 1);
                    }
                }
            }
                       
        }
        
        public void ModifySurface(int markerID, PointF[] corners)
        {
            lock (this.surfaceDict)
            {
                if (this.surfaceDict.ContainsKey(markerID))
                {
                    this.surfaceDict[markerID] = corners;
                }
                else
                {
                    this.surfaceDict.Add(markerID, corners);
                }
            }        
        }

        public PointF[] getSurface(int markerID)
        {
            if (this.surfaceDict.ContainsKey(markerID))
            {
                return this.surfaceDict[markerID];
            }
            return null;
        }

        public int getGaze(IPEndPoint ipEnd)
        {
            if (this.gazeDict.ContainsKey(ipEnd))
            {
                return this.gazeDict[ipEnd];
            }
            return (-1);
        }

        public bool CheckSurfaceEmpty()
        {
            return this.surfaceDict.Count > 0 ? false : true;
        }

        public List<int> GetIDsSurface()
        {
            return this.surfaceDict.Keys.ToList<int>();
        }

        public bool CheckGazeEmpty()
        {
            return this.gazeDict.Count > 0 ? false : true;
        }

        public List<IPEndPoint> GetIPsGaze()
        {
            return this.gazeDict.Keys.ToList<IPEndPoint>();
        }
    }
}
