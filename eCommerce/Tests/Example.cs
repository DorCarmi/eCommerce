using System;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Unittest example");
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}