using BibliotecaApi.Tests.Mocks;
using BibliotecaAPI.Controllers.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApi.Tests.PruebasUnitarias {

    [TestClass]
    public class RootControllerTests {

        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos5Links() {
            var authorization_service = new AuthorizationServiceMock();
            authorization_service.Resultado = AuthorizationResult.Success();
            var root_controller = new RootController ( authorization_service );
            root_controller.Url = new URLHelperMock();
            var resultado = await root_controller.Get();
            Assert.AreEqual ( 5, resultado.Value.Count() );
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos3Links () {
            var authorization_service = new AuthorizationServiceMock();
            authorization_service.Resultado = AuthorizationResult.Failed();
            var root_controller = new RootController ( authorization_service );
            root_controller.Url = new URLHelperMock ();
            var resultado = await root_controller.Get ();
            Assert.AreEqual ( 3, resultado.Value.Count () );
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos3Links_UsandoMoq () {
            var mock_authorization_service = new Mock<IAuthorizationService>();

            mock_authorization_service.Setup ( x => x.AuthorizeAsync ( 
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
            ) ).Returns ( Task.FromResult ( AuthorizationResult.Failed() ) );

            mock_authorization_service.Setup ( x => x.AuthorizeAsync (
                It.IsAny<ClaimsPrincipal> (),
                It.IsAny<object> (),
                It.IsAny<string> ()
            ) ).Returns ( Task.FromResult ( AuthorizationResult.Failed () ) );

            var root_controller = new RootController ( mock_authorization_service.Object );
            var mock_url_helper = new Mock<IUrlHelper>();

            mock_url_helper.Setup ( x => x.Link ( 
                It.IsAny<string>(), 
                It.IsAny<object>()
            ) ).Returns ( string.Empty );

            root_controller.Url = mock_url_helper.Object;
            var resultado = await root_controller.Get ();
            Assert.AreEqual ( 3, resultado.Value.Count () );
        }
    }
}
