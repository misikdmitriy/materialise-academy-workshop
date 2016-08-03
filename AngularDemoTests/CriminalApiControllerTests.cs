using AngularDemo.Controllers;
using AngularDemo.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace AngularDemoTests
{
    [TestFixture]
    public class CriminalApiControllerTests
    {
        private CriminalApiController _criminalApiController;
        private Mock<IList<Criminal>> _mockCriminals;
        private Mock<Criminal> _mockCriminal;
        private Criminal _criminal { get { return _mockCriminal.Ob; } }

        [SetUp]
        public void Setup()
        {
            _mockCriminal = new Mock<Criminal>();

            _criminal.ID = Guid.NewGuid();
            _criminal.Name = "Al Capone";
            _criminal.Description = "Gangster";
            _criminal.Reward = 1000000.00M;

            var arrayCriminals = new []
            {
                _criminal,
                new Criminal
                {
                    ID = Guid.NewGuid(),
                    Name = "Charles Manson",
                    Description = "Leader of the Manson Family",
                    Reward = 3000000.00M
                }
            };

            _mockCriminals = new Mock<IList<Criminal>>();

            _mockCriminals
                .Setup(l => l.GetEnumerator())
                .Returns(((IEnumerable<Criminal>)arrayCriminals).GetEnumerator());

            _mockCriminals
                .Setup(l => l.Count)
                .Returns(arrayCriminals.Length);

            _criminalApiController = new CriminalApiController(_mockCriminals.Object);
        }

        [Test]
        public void GetAllShouldReturnAllElements()
        {
            // Arrange
            // Act
            var result = _criminalApiController.GetAll() as OkNegotiatedContentResult<Criminal[]>;

            // Assert
            Assert.That(result.Content.Length, Is.EqualTo(_mockCriminals.Object.Count));
        }

        [Test]
        public void GetByIdShouldReturnCorrectItem()
        {
            // Arrange
            // Act
            var result = _criminalApiController.Get(_criminal.ID) as OkNegotiatedContentResult<Criminal>;

            //Assert
            Assert.That(result.Content, Is.Not.Null);

            Assert.That(result.Content.ID, Is.EqualTo(_criminal.ID));
            Assert.That(result.Content.Name, Is.EqualTo(_criminal.Name));
            Assert.That(result.Content.Reward, Is.EqualTo(_criminal.Reward));
            Assert.That(result.Content.Description, Is.EqualTo(_criminal.Description));
        }

        [Test]
        public void GetByIdShouldReturnNotFoundResult()
        {
            // Arrange
            // Act
            var result = _criminalApiController.Get(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(NotFoundResult)));
        }

        [Test]
        public void DeleteByIdShouldCallDeleteOfCollectionOnce()
        {
            // Arrange
            // Act
            _criminalApiController.Delete(_criminal.ID);

            // Assert
            _mockCriminals.Verify(l => l.Remove(It.Is<Criminal>(c => c.ID == _criminal.ID)), Times.Once());
        }

        [Test]
        public void DeleteByIdShouldNotCallDeleteOfCollection()
        {
            // Arrange
            // Act
            _criminalApiController.Delete(Guid.NewGuid());

            // Assert
            _mockCriminals.Verify(l => l.Remove(It.IsAny<Criminal>()), Times.Never());
        }

        [Test]
        public void PostShouldCallAddInCollectionOnce()
        {
            // Arrange
            var postCriminal = new Criminal
            {
                ID = Guid.NewGuid(),
                Name = "Ted Kaczynski",
                Description = "Terrorist",
                Reward = 2500000.00M
            };

            _criminalApiController.Request = new HttpRequestMessage();
            _criminalApiController.Request.RequestUri = new Uri("http://localhost:49457/api/criminal");
            _criminalApiController.Configuration = new HttpConfiguration();

            // Act
            _criminalApiController.Create(postCriminal);

            // Assert
            _mockCriminals.Verify(l => l.Add(It.IsAny<Criminal>()), Times.Once());
        }

        [Test]
        public void UpdateShouldUpdateEntity()
        {
            // Arrange
            _criminal.Name = "Another Name";

            _criminalApiController.Request = new HttpRequestMessage();
            _criminalApiController.Request.RequestUri = new Uri("http://localhost:49457/api/criminal");
            _criminalApiController.Configuration = new HttpConfiguration();

            // Act
            var response = _criminalApiController.Replace(_criminal);

            // Assert
#pragma warning disable CS0618 // Type or member is obsolete
            _mockCriminal.VerifySet(c => c.ID, Times.Once());
            _mockCriminal.VerifySet(c => c.Name, Times.Exactly(3));
            _mockCriminal.VerifySet(c => c.Description, Times.Exactly(2));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Test]
        public void UpdateShouldReturnNotFoundStatusCode()
        {
            // Arrange
            _criminalApiController.Request = new HttpRequestMessage();
            _criminalApiController.Request.RequestUri = new Uri("http://localhost:49457/api/criminal");
            _criminalApiController.Configuration = new HttpConfiguration();

            // Act
            var response = _criminalApiController.Replace(new Criminal { ID = Guid.NewGuid() });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void DeleteAllShouldCallClearOfCollection()
        {
            // Arrange
            // Act
            _criminalApiController.DeleteAll();

            // Assert
            _mockCriminals.Verify(l => l.Clear(), Times.AtLeastOnce());
        }
    }
}
