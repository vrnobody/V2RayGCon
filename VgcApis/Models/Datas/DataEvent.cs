using System;

namespace VgcApis.Models.Datas
{
    public class BoolEvent : EventArgs
    {
        public bool Data;
        public BoolEvent(bool data)
        {
            Data = data;
        }
    }

    public class StrEvent : EventArgs
    {
        public string Data;
        public StrEvent(string data)
        {
            Data = data;
        }
    }

    public class IntEvent : EventArgs
    {
        public int Data;
        public IntEvent(int data)
        {
            Data = data;
        }
    }
}
