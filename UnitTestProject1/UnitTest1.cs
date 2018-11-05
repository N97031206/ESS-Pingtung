using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.ESS;


namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestESSInitializer()
        {
            ESSInitializer ESSInit = new ESSInitializer();
        }
    }
}