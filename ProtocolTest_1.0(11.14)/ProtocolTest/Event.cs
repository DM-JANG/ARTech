using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolTest
{
    class Event
    {
        byte[] byteValues;
        TCPIP MBmaster;
        public string IP = string.Empty;
        public int Port;
        public bool Connected = false;
        public bool GetValue( int Prepos, int Preid, ref List<byte[]> Datas, TcpClient tcpSynClient)
        {
            if (MBmaster == null )
                MBmaster = new TCPIP(IP, Port, tcpSynClient);
          
            //if (MBmaster != null)
            //    MBmaster.connection(IP, Port);

            bool ResponeEvnet = MBmaster.ReadHoldingRegister(ref byteValues, Prepos, Preid, ref Datas);
            if (MBmaster.connected)
            {
                Connected = true;
            }
            else
            {
                Connected = false;
            }
            return Connected;
        }

    }
}
