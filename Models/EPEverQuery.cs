using System;
namespace RaspTracer_AN_Modbus.Models
{
    public class EPEverQuery
    {
        public string Name  { get; set; }
        public byte[] ByteArray { get; set; }
        public bool HasHighRegister { get; set; }
        public int Factor { get; set; }
    }
}

