using System;
using Catalog.Api.Models;
using Catalog.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Nuptiae.Controller.Base.Test.Unit
{
    public class CatalogControllerTest
    {
        private readonly CatalogController _catalogControllerMock;
        private readonly Mock<ICatalogRepo> _catalogRepoMock;
        private readonly Mock<ILogger<CatalogController>> _loggerCatalogMock;
        private readonly int idTravel = 8;
        private readonly string search = "search";
        private readonly CatalogTravel travel = new CatalogTravel
        {
            Country = "France",
            Departure = DateTime.Now,
            Description = "blablabla",
            Id = 13,
            Name = "Test",
            Price = 1230.99M,
            Town = "Bordeaux"
        };

        public CatalogControllerTest()
        {
            _catalogRepoMock = new Mock<ICatalogRepo>();
            _loggerCatalogMock = new Mock<ILogger<CatalogController>>();


            _catalogControllerMock = new CatalogController(_catalogRepoMock.Object, _loggerCatalogMock.Object);

        }
        [Fact]
        public void Ctor_ILoggerCatalogNull_ThrowsException()
        {
            // ARRANGE

            // ACT
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CatalogController(_catalogRepoMock.Object,
                    null
                    ));

            // Assert
            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public void Ctor_NullAgument_ThrowsException()
        {
            // ARRANGE

            // ACT
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new CatalogController(null,
                 _loggerCatalogMock.Object
                    ));

            // Assert
            Assert.Equal("repo", ex.ParamName);
        }
        [Fact]
        public async void GetReturnsProductWithSameId()
        {
            //ARRANGE //ACT 
            var result = await _catalogControllerMock.GetTravelId(idTravel);

            //ASSERT 

            Assert.IsType<ActionResult<CatalogTravel>>(result);

        }

        [Fact]
        public async void GetTravelById_ReturnsNotFound()
        {
            //ARRANGE 
            CatalogTravel expected = null;
            _catalogRepoMock
                .Setup(x => x.GetTravelByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expected);

            //ACT 
            var result = await _catalogControllerMock.GetTravelId(idTravel);

            //ASSERT 
            Assert.IsAssignableFrom<ActionResult<CatalogTravel>>(result.Result);
            var actual = result.Equals(expected);
            actual.CompareTo(expected);
        }
    }
}
