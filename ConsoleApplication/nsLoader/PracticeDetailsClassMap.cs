using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using CsvHelper.Configuration;

namespace ConsoleApplication.nsLoader
{
    internal sealed class PracticeDetailsClassMap : CsvClassMap<PracticeDetails>
    {
        private PracticeDetailsClassMap()
        {
            Map(m => m.Address1).Index(3);
            Map(m => m.Address2).Index(4);
            Map(m => m.Address3).Index(5);
            Map(m => m.Address4).Index(6);
            Map(m => m.Date).Index(0);
            Map(m => m.Postcode).Index(7);
            Map(m => m.Practicename).Index(2);
            Map(m => m.PracticeCode).Index(1);
          
        }
    }
}