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
        private Criminal _criminal { get { return _mockCriminal.Object; } }

        [SetUp]
        public void Setup()
        {
            _mockCriminal = new Mock<Criminal>();

            _mockCriminal.Object.ID = Guid.NewGuid();
            _mockCriminal.Object.Name = "Al Capone";
            _mockCriminal.Object.Description = "Gangster";
            _mockCriminal.Object.Reward = 1000000.00M;

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
            var result = _criminalApiController.GetAll() as OkNegotiatedContentResult<Criminal[]>;

            Assert.That(result.Content.Length, Is.EqualTo(_mockCriminals.Object.Count));
        }

        [Test]
        public void GetByIdShouldReturnCorrectItem()
        {
            var result = _criminalApiController.Get(_mockCriminal.Object.ID) as OkNegotiatedContentResult<Criminal>;

            Assert.That(result.Content, Is.Not.Null);

            Assert.That(result.Content.ID, Is.EqualTo(_criminal.ID));
            Assert.That(result.Content.Name, Is.EqualTo(_criminal.Name));
            Assert.That(result.Content.Reward, Is.EqualTo(_criminal.Reward));
            Assert.That(result.Content.Description, Is.EqualTo(_criminal.Description));
        }

        [Test]
        public void GetByIdShouldReturnNotFoundResult()
        {
            var result = _criminalApiController.Get(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf(typeof(NotFoundResult)));
        }

        [Test]
        public void DeleteByIdShouldCallDeleteOfCollectionOnce()
        {
            _criminalApiController.Delete(_criminal.ID);

            _mockCriminals.Verify(l => l.Remove(It.Is<Criminal>(c => c.ID == _criminal.ID)), Times.Once());
        }

        [Test]
        public void DeleteByIdShouldNotCallDeleteOfCollection()
        {
            _criminalApiController.Delete(Guid.NewGuid());

            _mockCriminals.Verify(l => l.Remove(It.IsAny<Criminal>()), Times.Never());
        }

        [Test]
        public void PostShouldCallAddInCollectionOnce()
        {
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

            _criminalApiController.Create(postCriminal);

            _mockCriminals.Verify(l => l.Add(It.IsAny<Criminal>()), Times.Once());
        }

        [Test]
        public void UpdateShouldUpdateEntity()
        {
            _mockCriminal.Object.Name = "Another Name";

            _criminalApiController.Request = new HttpRequestMessage();
            _criminalApiController.Request.RequestUri = new Uri("http://localhost:49457/api/criminal");
            _criminalApiController.Configuration = new HttpConfiguration();

            var response = _criminalApiController.Replace(_mockCriminal.Object);

#pragma warning disable CS0618 // Type or member is obsolete
            _mockCriminal.VerifySet(c => c.ID, Times.Once());
            _mockCriminal.VerifySet(c => c.Name, Times.Exactly(3));
            _mockCriminal.VerifySet(c => c.Description, Times.Exactly(2));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Test]
        public void UpdateShouldReturnNotFoundStatusCode()
        {
            _criminalApiController.Request = new HttpRequestMessage();
            _criminalApiController.Request.RequestUri = new Uri("http://localhost:49457/api/criminal");
            _criminalApiController.Configuration = new HttpConfiguration();

            var response = _criminalApiController.Replace(new Criminal { ID = Guid.NewGuid() });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public void DeleteAllShouldCallClearOfCollection()
        {
            _criminalApiController.DeleteAll();

            _mockCriminals.Verify(l => l.Clear(), Times.AtLeastOnce());
        }
    }
}
