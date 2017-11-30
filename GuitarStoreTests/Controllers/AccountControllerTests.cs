using Microsoft.VisualStudio.TestTools.UnitTesting;
using GuitarStore.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Security.Principal;
using System.Web;
using Moq;

namespace GuitarStore.Controllers.Tests
{
    [TestClass()]
    public class AccountControllerTests
    {
        [TestMethod()]
        public void IndexTest()
        {
            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("admin")).Returns(true);
            userMock.Setup(p => p.Identity).Returns(new GenericIdentity("test@matt.com"));
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(ctx => ctx.User)
                       .Returns(userMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(con => con.HttpContext)
                                 .Returns(contextMock.Object);

            Moq.Mock<IPaymentService> paymentService = new Moq.Mock<IPaymentService>();
            paymentService.Setup(m => m.GetCustomer(Moq.It.IsAny<string>())).Returns(new Mock<Braintree.Customer>().Object);
            AccountController controller = new AccountController(paymentService.Object);
            controller.ControllerContext = controllerContextMock.Object;
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}