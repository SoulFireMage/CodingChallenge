using ConsoleApplication.nsLoader;

using ConsoleApplication.nsSolver;

namespace ConsoleApplication.nsFactory
{
    internal static class Factory
    {
        private static ILoader _loader;
        public static ILoader GetDataLoader()
        {
            return _loader ?? (_loader = new Loader());
        }

        private static ISolver _solver;
        public static ISolver GetSolver()
        {
            return _solver ?? (_solver = new Solver());
        }

       
    }
}
