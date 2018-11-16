using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolTest
{
    public class Define
    {
        public string Preamble = string.Empty;  // PRT3
        public int Lenght = 0; // 길이
        public int SeqNo = 0; // 시퀀스 넘버
        public byte[] OpCode = new byte[4]; // 장치코드
        public Train LData;
        public byte[] Reserved;
        public byte[] Checksum;
        public string UpPos = string.Empty;
        public string DownPos = string.Empty;
    }
    public class Train
    {
        public string ScrDevName = string.Empty;
        public string DstDevName = string.Empty;
        public string[] TimeStamp = new string[8];
        public int TrainNum;
        public List<TrainDetail> TrainData = new List<TrainDetail>();
        public bool[] Finish = new bool[29];

    }

    public class TrainDetail
    {
        public string TrainID;
        public int UpDown;
        public string TrainKind;
        public string TrainLast_space_ID;
        public string[] LastTime = new string[8];
        public int Position;
    }


}
