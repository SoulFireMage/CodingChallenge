using System.Collections.Generic;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using ConsoleApplication.nsLoader;

namespace ConsoleApplication.nsDataObjects.nsData
{
    public interface IDataCache
    {
        IEnumerable<GpPrescriptionData> PrescriptionData { get; }
        
        IEnumerable<PracticeDetails> PracticeDetails { get; } 
    }
}
