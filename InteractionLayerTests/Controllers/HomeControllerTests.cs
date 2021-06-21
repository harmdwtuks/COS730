using Microsoft.VisualStudio.TestTools.UnitTesting;
using InteractionLayer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using System.Web;

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
        public void IndexTestLogin()
        {
            // create the mock http context
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();

            context.Setup(m => m.HttpContext.Session).Returns(session.Object);

            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = context.Object;

            // Act
            ViewResult result = controller.Index(new Models.Login() { Username = "Admin", Password = "mraH12345^&*(" }) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void LandingPageTest()
        {
            // create the mock http context
            var context = new Mock<ControllerContext>();
            var session = new Mock<HttpSessionStateBase>();

            context.Setup(m => m.HttpContext.Session).Returns(session.Object);
            
            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = context.Object;
            
            // Act
            ViewResult result = controller.LandingPage() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void LandingPageTestWithSession()
        {
            // create the mock http context
            var context = new Mock<ControllerContext>();

            var session = new MockHttpSession();

            context.Setup(m => m.HttpContext.Session).Returns(session);

            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = context.Object;
            controller.Session["ClientId"] = 1;
            controller.Session["ClientName"] = "John";
            
            // Act
            ViewResult result = controller.LandingPage() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }

    public class MockHttpSession : HttpSessionStateBase
    {
        Dictionary<string, object> _sessionDictionary = new Dictionary<string, object>();

        public override object this[string name]
        {
            get { return _sessionDictionary[name]; }
            set { _sessionDictionary[name] = value; }
        }
    }
}