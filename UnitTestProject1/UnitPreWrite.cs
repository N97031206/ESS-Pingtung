using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.ESS;


namespace UnitTestProject
{
    [TestClass]
    public class UnitPreWrite
    {
        [TestMethod]
        public void TestESSInitializer()
        {
            ESSInitializer ESSInit = new ESSInitializer();
        }
    }
}