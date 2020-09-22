using BusiBlocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.IO;
using System.Web;

namespace BusiBlocksTest
{
    
    
    /// <summary>
    ///This is a test class for XHTMLTextTest and is intended
    ///to contain all XHTMLTextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XHTMLTextTest
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
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            XHTMLText target = new XHTMLText();
            string folder = PathHelper.CombineUrl(Environment.CurrentDirectory, PathHelper.CombineUrl("Data", "Xhtml"));
            
            string xhtml = PathHelper.CombineUrl(folder, "sop-1.1.001-management-meetings-v1.1.html");
            target.Load(xhtml);

            xhtml = "<p>The purpose of this Procedure is to specify requirements for scheduling and performing Internal Quality " +
                "and OH&amp;S Audits of the Sigma Bravo Business Management System (BMS) to verify its maintenance, effectiveness and continual improvement.</p>";
            target.Load(xhtml);

            xhtml = "  <h1>Internal &amp; Audits</h1>";
            target.Load(xhtml);

            xhtml = "<p>Doc &amp; Data Develop &amp; Control</p>";
            target.Load(xhtml);

            xhtml = "<p>&amp;</p>";
            target.Load(xhtml);
            xhtml = target.Xhtml;
            target.Load(xhtml);

            // This one fails.
            //xhtml = "<p>&lt;h1&gt;&amp;&lt;h1&gt;</p>";
            //xhtml = HttpUtility.HtmlDecode(xhtml);
            //target.Load(xhtml);

            xhtml = PathHelper.CombineUrl(folder, "sop-1.1.002-internal-audits.html");
            target.Load(xhtml);

            xhtml = PathHelper.CombineUrl(folder, "sop-1.1.003-problems-and-enhancements-v1.1.html");
            target.Load(xhtml);
        }
    }
}
