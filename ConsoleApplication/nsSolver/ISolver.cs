using System;
using System.Collections.Generic;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using ConsoleApplication.nsDataObjects.nsResults;

namespace ConsoleApplication.nsSolver
{
    internal interface ISolver
    {
        List<IResult> Solve();
        Dictionary<string, Decimal> Results { get; }
        List<Treatment> Treatments { get; }
        Dictionary<string, List<GpPrescriptionData>> GetDataPerBnfCode(int count);
     
    }

}
