using MsgPack;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace GazeCollector
{
    class PupilProSurface
    {

        MessagePackSerializer serializer = SerializationContext.Default.GetSerializer<MessagePackObject>();

        public MessagePackObjectDictionary GetInitialDictionary(ZMessage message)
        {
            //ElementAt(1) is used as 0 indicates the topic name and 1 is the actual msg
            byte[] byteBuf = message.ElementAt(1).Read();
            Stream stream = new MemoryStream(byteBuf);
            try
            {
                MessagePackObject data = (MessagePackObject)this.serializer.Unpack(stream);
                MessagePackObjectDictionary dict = data.AsDictionary();
                return dict;
            }catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public MessagePackObjectDictionary GetGazeOnSrfMessageDictionary(MessagePackObjectDictionary dataDict)
        {
            try
            {
                MessagePackObjectDictionary gazeOnSrfDict = dataDict["gaze_on_srf"].AsEnumerable().ElementAt(0).AsDictionary();
                if ((bool)gazeOnSrfDict["on_srf"])
                {
                    return (gazeOnSrfDict);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Gaze is not on the surface");
            }
            return null;
        }

        public Gazepoint GetGazePointOnSurface(MessagePackObjectDictionary dict)
        {
            MessagePackObjectDictionary gazeDataDict = this.GetGazeOnSrfMessageDictionary(dict);
            if (gazeDataDict != null)
            {
                //Console.WriteLine(gazeOnSrfDict["norm_pos"]);
                float x = (float)gazeDataDict["norm_pos"].AsEnumerable().ElementAt(0);
                float y = (float)gazeDataDict["norm_pos"].AsEnumerable().ElementAt(1);
                Gazepoint gp = new Gazepoint(x, y);
                return gp;
            }
            return null;
        }

        public String GetSurfaceName(MessagePackObjectDictionary dict)
        {
            return dict["name"].ToString();
        }
    }
}