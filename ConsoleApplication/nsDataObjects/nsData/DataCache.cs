using System.Collections.Generic;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using ConsoleApplication.nsLoader;

namespace ConsoleApplication.nsDataObjects.nsData
{
    //TODO: If time allows, restructure accessibility back into recommended private/internal rather than public. In order to get 
    //a working report I needed a repository. This is a quick fix (because I wrote it) rather than a horrible hack (someone else wrote it!)

    public class DataCache : IDataCache
    {

        private IEnumerable<GpPrescriptionData> _prescriptionData;
        private IEnumerable<PracticeDetails> _practices;
        private ILoader _loader;


        public DataCache(IEnumerable<GpPrescriptionData> prescriptionData, IEnumerable<PracticeDetails> practices)
        {
            _prescriptionData = prescriptionData;
            _practices = practices;
        }

        public DataCache()
        {
            if (_loader == null) _loader = nsFactory.Factory.GetDataLoader();
            _prescriptionData = _loader.Data.PrescriptionData;
            _practices = _loader.Data.PracticeDetails;
        }
        //Did we always use the constructor? 
        //Reports seems to need a public property - if we didn't fill our properties already, do so and return them
        public IEnumerable<GpPrescriptionData> PrescriptionData
        {
            get
            {

                if (_prescriptionData == null)
                {
                    ILoader loader = nsFactory.Factory.GetDataLoader();
                    _prescriptionData = loader.Data.PrescriptionData;
                }

                return
                    _prescriptionData;

            }
        }

        public IEnumerable<PracticeDetails> PracticeDetails
        {
            get
            {

                if (_practices == null)
                {
                    ILoader loader = new Loader();
                    _practices = loader.Data.PracticeDetails;
                }

                return
                    _practices;

            }
        }


    }
}

 