using System;

namespace ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects
{
    public class Treatment
    {
        public String Name { get; set; }
        public string BNFCode { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

}
