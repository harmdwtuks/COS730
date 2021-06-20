using Microsoft.VisualStudio.TestTools.UnitTesting;
using InteractionLayer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InteractionLayer.Controllers.Tests
{
    [TestClass()]
    public class MetricsControllerTests
    {
        [TestMethod()]
        public void IndexTest()
        {
            // Arrange
            MetricsController controller = new MetricsController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            Assert.Fail();
        }

        [TestMethod()]
        public void RecordMetricTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMetricTypesByClassTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMetricUnitByTypeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddMetricTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NewTypeAndClassTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NewMetricTypeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NewClassTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NewUnitTest()
        {
            Assert.Fail();
        }
    }
}