using System;

namespace ConsoleApplication.nsDataObjects.nsResults
{
    public class Result : IResult
    {
        private readonly int _testNumber;
        private readonly string _questionDescription;
        private readonly object _answer;

        public Result(int testNumber, string questionDescription, object answer)
        {
            _testNumber = testNumber;
            _questionDescription = questionDescription;
            _answer = answer;
        }

        public override string ToString()
        {
            return string.Format("Question {0}: {1}{3}Answer: {2}{3}", _testNumber, _questionDescription, _answer, Environment.NewLine);

        }
    }
}
