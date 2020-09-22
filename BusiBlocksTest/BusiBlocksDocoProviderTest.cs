using BusiBlocks.DocoBlock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BusiBlocksTest
{
    
    
    /// <summary>
    ///This is a test class for BusiBlocksDocoProviderTest and is intended
    ///to contain all BusiBlocksDocoProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BusiBlocksDocoProviderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CreateCategory
        ///</summary>
        [TestMethod()]
        public void CreateCategoryTest()
        {
            BusiBlocksDocoProvider target = new BusiBlocksDocoProvider(); // TODO: Initialize to an appropriate value
            string displayName = "Unit Test Category"; // TODO: Initialize to an appropriate value
            Category expected = DocoManager.CreateCategory(displayName); // TODO: Initialize to an appropriate value
            Category actual;
            actual = target.CreateCategory(displayName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Able to create a test category");


        }
    }
}
