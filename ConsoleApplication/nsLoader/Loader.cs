using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ConsoleApplication.nsDataObjects.nsData;

using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using CsvHelper;

namespace ConsoleApplication.nsLoader
{

    public class Loader : ILoader
    {
       //Lets load this once
        private static IDataCache _data;
        public IDataCache Data
        {
            get
            {
                return _data ?? (_data = LoadData());
            }
        }


        //This would be better private!
        public Loader()
        {
            LoadData();
        }

#region "Tinfoil hat - No c:\? No Web?"
        /// <summary>
        /// Demonstrating basic error management.
        /// Just in case it's tested on a system with D: or other drive, we need the root.
        /// If the internet doesn't work, better if we don't crash out.
        /// Fakes need to be in the current folder. 
        /// </summary>

        private static readonly string _getDataDirectory = GetDataDirectory();
        private static string GetDataDirectory()
        {
            String defaultRoot = "C:\\Temp";
            if (!Directory.Exists(defaultRoot))
                try
                {
                    Directory.CreateDirectory(defaultRoot);
                    
                }
                catch (DirectoryNotFoundException)
                {
                    defaultRoot = String.Format("{0}Temp", DriveInfo.GetDrives().First().Name);
                    Directory.CreateDirectory(defaultRoot);
                }
          return defaultRoot;
        }
        //What no internet?

        private static void GetDataFiles(string filename, string url)
        {
            using (WebClient myWebClient = new WebClient())
            {
                string fileToDownload = url;
                if (File.Exists(_getDataDirectory + "\\" + filename)) return;
                try
                {
                    myWebClient.DownloadFile(fileToDownload, _getDataDirectory + "\\" + filename);
                }
                catch (WebException)
                {

                    File.Copy(Directory.GetCurrentDirectory() + "\\Fakes\\" + filename, _getDataDirectory + "\\" + filename);
                }
            }
        }

#endregion

        /// <summary>
        /// Implemented CsvHelper in place of manual string split.
        /// A custom class map is not required if your class matches the headings used in the CSV.
        /// The parser splits each row into the strongly typed IEnumerable making later operations a lot easier.
        /// Should be no file handles left dangling at the end.
        /// </summary>
        /// <returns>IDataCache</returns>
        private   IDataCache LoadData()
        {
       
            GetDataFiles("foo", "http://datagov.ic.nhs.uk/T201202ADD%20REXT.CSV");
            GetDataFiles("bar", "http://datagov.ic.nhs.uk/T201109PDP%20IEXT.CSV");
            
            IEnumerable<GpPrescriptionData> prescriptionData;
            
            using (StreamReader streamReader = new StreamReader(_getDataDirectory + "\\" + "bar"))
            {
                CsvReader csv = new CsvReader(streamReader); 
                csv.Configuration.RegisterClassMap<GpPrescriptionDataClassMap>();
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.IgnoreHeaderWhiteSpace = true;
                prescriptionData = csv.GetRecords<GpPrescriptionData>().ToList();
           }

            IEnumerable<PracticeDetails> practices;
            using (StreamReader streamReader = new StreamReader(_getDataDirectory + "\\" + "foo"))
            {
                CsvReader csv = new CsvReader(streamReader);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<PracticeDetailsClassMap>();
                practices = csv.GetRecords<PracticeDetails>().ToList();
                
            }
            _data = new DataCache(prescriptionData, practices);
            return Data;
        }
    }
}
