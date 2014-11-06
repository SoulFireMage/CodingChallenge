namespace ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects
{
    public class PracticeDetails
    {
      
        public string Date { get;   set; }
        public string PracticeCode { get;   set; }
        public string Practicename { get;   set; }
        public string Address1 { get;   set; }
        public string Address2 { get;   set; }
        public string Address3 { get;   set; }
        public string Address4 { get;   set; }
        public string Postcode { get;   set; }
        public string CombinedAddress { 
            get { return Address1 + Address2 + Address3 + Address4; }
            
        }
    }
}
