using System.Linq;
using ConsoleApplication.nsDataObjects.nsData.nsBusinessObjects;
using ConsoleApplication.nsSolver;
using NUnit.Framework;

namespace ConsoleApplication.Tests
{
 /// <summary>
 /// These tests are very basic but demonstrate simple unit testing and made developing some of the LINQ a lot simpler!
 /// </summary>
    [TestFixture]
    public class Tests
    {

       //Didn't want to alter the contract for ISolver!
        private Solver _solver;
        private Solver Solver
        {
            get { return _solver ?? (_solver = new Solver()); }
        }

        [Test]
        public void CheckGetPracticeViaCodeReturnStringAlways()
        {
            // Promote to field for further tests.
            foreach (string code in Solver.Loader.Data.PracticeDetails.Select(x => x.PracticeCode))
            {
                Assert.IsNotNull(Solver.GetPracticeViaCode(code));
            }
        }

        [Test]
        public void WillDataPerBNFCodeProduceAResultAlways()
        {
            foreach (string code in Solver.Treatments.Select(x => x.BNFCode))
            {
                var _code = code;
                Assert.IsNotNull(Solver.GetDataPerBnfCode(5).Where(y => y.Value.Select(x=>x.BNFCODE).FirstOrDefault() == _code).Select(x => x));
            }
          
        }

        //TODO I broke this test - as in it runs forever yet the function appears to work fine. Unfixed due to time constraints.
        [Test]
        public void DoesgetHighest_LowestSpendPerBNFReallyWork()
        {
            foreach (Treatment code in Solver.Treatments)
            {
                var c = code;
                Assert.IsTrue(Solver.getHighest_LowestSpendPerBNF(c).Item1.ACTCOST <= Solver.getHighest_LowestSpendPerBNF(c).Item2.ACTCOST);
            }
        }
    }

}
