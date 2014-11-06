using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConsoleApplication.nsDataObjects.nsData;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using ConsoleApplication.nsDataObjects.nsResults;
using ConsoleApplication.nsFactory;
using ConsoleApplication.nsLoader;

namespace ConsoleApplication.nsSolver
{
    class Solver : ISolver
    {
      
        internal static ILoader Loader
        {
            get { return Factory.GetDataLoader() ; }
        }
       
        private IEnumerable<Tuple<string, decimal>> _spendPerPractice;  
        public IEnumerable<Tuple<string, decimal>> SpendPerPractice 
        {
            get 
            {
                return _spendPerPractice ?? (_spendPerPractice = DataCache.PrescriptionData.GroupBy(x => x.PRACTICE).
                                                                   Select(
                                                                       x =>
                                                                       new Tuple<string, decimal>(x.Key,
                                                                                                  x.Sum(y => y.ACTCOST))));
            }
        }
        
    
        private Dictionary<string, decimal> _results;
        public Dictionary<string, Decimal> Results
        {
            get
            {
                if (_results == null)
                {
                    _results = new Dictionary<string, decimal>(new SpecialComparator());

                    foreach (Tuple<string, decimal> practice in SpendPerPractice)
                    {
                        //Probably not important if we don't have it
                        if (!PostCodeMap.ContainsKey(practice.Item1))
                        {
                            continue;
                        }

                        string postCodeOfPractice = PostCodeMap[practice.Item1];

                        if (!_results.ContainsKey(postCodeOfPractice))
                        {
                            _results.Add(postCodeOfPractice, 0);
                        }

                        _results[postCodeOfPractice] += practice.Item2;
                    }
                  
                }
                return _results;
            }
        }

        /// <summary>
        /// Primitive joining of foo and bar via the practice code used in the latter
        /// </summary>
        /// <param name="code">String practice code</param>
        /// <returns>String Practice name</returns>
        public static String GetPracticeViaCode(String code)
        {
            return (from c in DataCache.PracticeDetails
                               where c.PracticeCode == code
                               select c.Practicename).FirstOrDefault()?? "Unknown Practice";
        }

       
       /// <summary>
       /// Return a canonical list of treatments. Candidate for background loading!
       /// </summary>
        private List<Treatment> _treatments;
        public List<Treatment> Treatments { get
        {
            return _treatments ??
                   (_treatments =
                    DataCache.PrescriptionData.GroupBy(t => t.BNFCODE, t => t.BNFNAME).Select(x => new Treatment { Name = x.FirstOrDefault(), BNFCode = x.Key }).ToList());
        }
        }

       //TODO Why doesn't this return more than 5 results?
   
        /// <summary>
        /// Grouping by treatment type, allowing analysis of what practices use the most of any one treatment. 
        /// A more complete version would also allow for ordering choice.
        /// </summary>
        private Dictionary<string, List<GpPrescriptionData>> _dataPerBnfCode;
        
        public Dictionary<string, List<GpPrescriptionData>> GetDataPerBnfCode  (int count)
        {
            if (_dataPerBnfCode == null)
            {
               
                var groups =
                    DataCache.PrescriptionData.GroupBy(g => g.BNFCODE);

                var tempDictionary =
                    new Dictionary<string, List<GpPrescriptionData>>();

                foreach (var element in groups)
                {
                    List<GpPrescriptionData> list = element.OrderBy(o => o.ACTCOST).Take(count).ToList();

                    if (tempDictionary.ContainsKey(element.Key))
                        tempDictionary[element.Key] = list;
                    else
                    {
                        tempDictionary.Add(element.Key, list);
                    }

                }
                _dataPerBnfCode = tempDictionary;
            }
            return _dataPerBnfCode ;

        }

        //Refactor naming and purpose !
        private IEnumerable<GpPrescriptionData> GetPrescriptionDataViaBnf(String code)
        {
            return DataCache.PrescriptionData.Where(g => g.BNFCODE == code).Select(g => g).ToList();
        }

        private Dictionary<string, string> _postCodeMap; 
        public Dictionary<string, string> PostCodeMap 
        {
            get
            {
                return _postCodeMap ?? (_postCodeMap = DataCache.PracticeDetails.ToDictionary(x => x.PracticeCode, x => x.Postcode));
            }
        }

        /// <summary>
        /// Pick a treatment type, get back who spends the most and least.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public String Highest_LowestSpendPerBNF(Treatment code)
        {
            var data = getHighest_LowestSpendPerBNF(code);
            Func<GpPrescriptionData, string> createValue =
                x => String.Format("{0} Cost: {1:c}", GetPracticeViaCode(x.PRACTICE), x.ACTCOST);
            return String.Format("{2}Lowest: {0}{2}Highest: {1}", createValue(data.Item1), createValue(data.Item2), Environment.NewLine);
        }

        //Temporarily public to simplify the unit testing! 
        public Tuple <GpPrescriptionData,GpPrescriptionData> getHighest_LowestSpendPerBNF(Treatment code)
        {
            var _data = GetPrescriptionDataViaBnf(code.BNFCode).OrderBy(p => p.ACTCOST);
            return Tuple.Create(_data.First(), _data.Last());
        }

        private static readonly IDataCache DataCache = Loader.Data;

        public List<IResult> Solve()
        {
            List<IResult> result = new List<IResult>();

            //Count of practices in London - created a combined property in PracticeDetails, 
            
            var countOfPracticesInLondon = DataCache.PracticeDetails.OrderBy(x => x.Postcode).Count((x => x.CombinedAddress.ToUpper(CultureInfo.CurrentCulture).Contains("LONDON")));
            result.Add(new Result(1, "Count of practices in London", countOfPracticesInLondon));
           
            var pepperMintPrescriptions = GetPrescriptionDataViaBnf("0102000T0");

            var averageActCosts = pepperMintPrescriptions.Select(x => x.ACTCOST).Average(); //Assuming it's average cost to each practice!
            var pepperMint = new Treatment {Name = "PepperMint", BNFCode = "0102000T0"};
            
            result.Add(new Result(2, "What was the average actual cost of all peppermint oil prescriptions? ", string.Format("{0:c}", Math.Round(averageActCosts,2)))); 
            
            result.Add(new Result(3, "Which 5 post codes have the highest actual spend? See graph ", ""));
            result.Add(new Result(4, String.Format("Who spent the most and least on {0}", pepperMint), Highest_LowestSpendPerBNF(pepperMint)));
            
            //Question 4 - Do something interesting with the data if you fancy...
            //Answer 5: You can return the highest spender of any one treatment with the treatment picker

            return result;
        }

    }


    //TODO:  (RG)  Clarify this?
    internal class SpecialComparator : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x != null && y != null)
                return x.Equals(y);
            return false;
        }

        public int GetHashCode(string obj)
        {
            return 1;
        }
    }
}

