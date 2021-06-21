using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLayer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DatabaseLayer.Controllers.Tests
{
    [TestClass()]
    public class MetricsControllerTests
    {
        [TestMethod()]
        public void GetClassByIdTest()
        {
            string JsonQuery = "{" +
                                "\"id\":\"1\"" +
                               "}";

            // Arrange
            MetricsController controller = new MetricsController();

            // Act
            //ActionResult result = controller.GetClassById(JsonConvert.DeserializeObject(JsonQuery)) as ActionResult;

            // Assert
            Assert.IsNotNull(JsonQuery);
        }

        [TestMethod()]
        public void ClassTest()
        {

        }

        [TestMethod()]
        public void CreateClassTest()
        {

        }

        [TestMethod()]
        public void UpdateClassTest()
        {

        }

        [TestMethod()]
        public void GetUnitByIdTest()
        {

        }

        [TestMethod()]
        public void UnitTest()
        {

        }

        [TestMethod()]
        public void GetUnitByTypeIdTest()
        {

        }

        [TestMethod()]
        public void CreateUnitTest()
        {

        }

        [TestMethod()]
        public void UpdateUnitTest()
        {

        }

        [TestMethod()]
        public void CreateTypeTest()
        {

        }

        [TestMethod()]
        public void TypeTest()
        {

        }

        [TestMethod()]
        public void GetTypeByIdTest()
        {

        }

        [TestMethod()]
        public void GetTypesByClassIdTest()
        {

        }

        [TestMethod()]
        public void UpdateTypeTest()
        {

        }

        [TestMethod()]
        public void UpdateTypeClassTest()
        {

        }

        [TestMethod()]
        public void UpdateTypeUnitTest()
        {

        }

        [TestMethod()]
        public void GetUserRecordsTest()
        {

        }

        [TestMethod()]
        public void CreateRecordTest()
        {

        }

        [TestMethod()]
        public void UpdateRecordTest()
        {

        }

        [TestMethod()]
        public void DeleteRecordTest()
        {

        }
    }
}