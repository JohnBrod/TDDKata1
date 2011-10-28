using System;
using System.Linq;
using NUnit.Framework;

namespace TDDKata1
{
    [TestFixture]
    public class TestCalculatorTests
    {
        [Test]
        public void Add_ForEmtyString_ReturnZero()
        {
            int result = GetCalculator().Add("");
            Assert.AreEqual(0, result);
        }

        [TestCase(12, "12")]
        [TestCase(33, "33")]
        public void Add_ForSingleNumber_ReturnThisNumber(int expected, string input)
        {
            int result = GetCalculator().Add(input);
            Assert.AreEqual(expected, result);
        }

        [TestCase(123, "100,20,3")]
        [TestCase(45, "20,10,15")]
        public void Add_ForMoreNumbers_ReturnSumOf(int expected, string input)
        {
            int result  = GetCalculator().Add(input);
            Assert.AreEqual(expected, result);
        }

        [TestCase(7, "3\n4")]
        [TestCase(10, "3\n4,3")]
        public void Add_ForNewLinesAsSeparators_ReturnSumOf(int expected, string input)
        {
            int result = GetCalculator().Add(input);
            Assert.AreEqual(expected, result);
        }

        [TestCase("1,\n")]
        public  void Add_ForTwoSeparatorsInARow_ExpectException(string input)
        {
            Assert.Throws<FormatException>(() => GetCalculator().Add(input));
        }

        [TestCase(44, "//$\n40$4")]
        [TestCase(3, "//#\n1#2")]
        public void Add_ForDelimiterChangeExpr_ReturnSumOf(int expected, string input)
        {
            int result = GetCalculator().Add(input);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void AnyNegativeNumberShouldThrownAnArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => GetCalculator().Add("-1"));
            Assert.AreEqual("negatives not allowed, -1", exception.Message);
        }

        [Test]
        public void AllNegativeNumbersShouldBeShownInArgumentExceptionMessage()
        {
            var exception = Assert.Throws<ArgumentException>(() => GetCalculator().Add("-1,-2"));
            Assert.AreEqual("negatives not allowed, -1,-2", exception.Message);
        }

        private static StringCalculator GetCalculator()
        {
            return new StringCalculator();
        }
    }

    public class StringCalculator
    {
        private static string NewLineSymbol = "\n";
        private string[] _separators = {NewLineSymbol, DefaultSeparator};
        private const string DefaultSeparator = ",";

        public int Add(string input)
        {
            if (input.StartsWith("//"))
            {
                var separator = input.Substring(input.IndexOf("//") + 2, 1);
                _separators = new[] {separator, NewLineSymbol};
                input = input.Substring(input.IndexOf(NewLineSymbol) + 1);
            }

            if(IsEmpty(input))
                return 0;

            return HandleMultipleNumbers(input);
        }

        private int HandleMultipleNumbers(string input)
        {
            var numbers = input.Split(_separators, StringSplitOptions.None).Select(i => int.Parse(i));

            if (numbers.Any(i => i < 0))
            {
                var message = string.Format("negatives not allowed, {0}", string.Join(",", numbers.Where(i => i < 0)));
                throw new ArgumentException(message);
            }

            return numbers.Sum();
        }

        private static bool IsEmpty(string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }
    }
}
