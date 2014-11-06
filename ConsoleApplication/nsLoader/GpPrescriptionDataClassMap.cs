using System.Globalization;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using CsvHelper.Configuration;

namespace ConsoleApplication.nsLoader
{
    internal sealed class GpPrescriptionDataClassMap : CsvClassMap<GpPrescriptionData>
    {
        

        private GpPrescriptionDataClassMap()
        {
            Map(m => m.SHA).Index(0);
            Map(m => m.PCT).Index(1);
            Map(m => m.PRACTICE).Index(2);
            Map(m => m.BNFCODE).Index(3);
            Map(m => m.BNFNAME).Index(4);
            Map(m => m.ITEMS).Index(5).TypeConverterOption(NumberStyles.Integer);
            Map(m => m.NIC).Index(6).TypeConverterOption(NumberStyles.Currency);
            Map(m => m.ACTCOST).Index(7).TypeConverterOption(NumberStyles.Currency);
            Map(m => m.PERIOD).Index(8).TypeConverterOption(NumberStyles.Integer);
            //}
        }
    }
}