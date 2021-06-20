using Microsoft.VisualStudio.TestTools.UnitTesting;
using InteractionLayer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;

namespace InteractionLayer.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        [TestMethod()]
        public void IndexTest()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void AboutTest()
        {

        }

        [TestMethod()]
        public void ContactTest()
        {

        }

        [TestMethod()]
        public void IndexTest1()
        {

        }

        [TestMethod()]
        public void LandingPageTest()
        {
            // Arrange
            HomeController controller = new HomeController();

            Mock<TypeToMock> mockObjectType = new Mock<TypeToMock>();

            // Act
            ViewResult result = controller.LandingPage() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}