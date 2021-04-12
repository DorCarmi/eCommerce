using eCommerce;
using NUnit.Framework;

namespace Tests
{
    public class ExampleTests
    {
        [SetUp]
        public void Setup()
        {
            Item item = new Item();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}