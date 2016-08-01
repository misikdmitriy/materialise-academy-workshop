using AngularDemo.Models;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AngularDemoTests
{
    [TestFixture]
    public class CriminalApiControllerTests
    {
        private const string _projectURL = "http://localhost:49457/";
        private const string _apiPath = "api/criminal/";
        private HttpResponseMessage _resultResponse;

        [OneTimeSetUp]
        public void Setup()
        {
            _resultResponse = null;
        }

        [Test, Order(0)]
        public async Task GetAllShouldReturnArrayOfAllElements()
        {
            using (var httpClient = new HttpClient())
            {
                _resultResponse = await httpClient.GetAsync(_projectURL + _apiPath);
            }

            var result = await _resultResponse.Content.ReadAsAsync<Criminal[]>();

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.IsNotNull(result);
            Assert.That(result.Length, Is.EqualTo(4));
        }

        [Test, Order(1)]
        public async Task GetShouldReturnCriminalByItsGuid()
        {
            Criminal[] array = null;

            using (var httpClient = new HttpClient())
            {
                var getAllResponse = await httpClient.GetAsync(_projectURL + _apiPath);
                array = await getAllResponse.Content.ReadAsAsync<Criminal[]>();
                _resultResponse = await httpClient.GetAsync(_projectURL + _apiPath + array[0].ID);
            }

            var result = await _resultResponse.Content.ReadAsAsync<Criminal>();

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.IsNotNull(result);
            Assert.That(result.ID, Is.EqualTo(array[0].ID));
            Assert.That(result.Name, Is.EqualTo(array[0].Name));
            Assert.That(result.Reward, Is.EqualTo(array[0].Reward));
            Assert.That(result.Description, Is.EqualTo(array[0].Description));
        }

        [Test]
        public async Task GetShouldReturnNotFoundIfElementDoesNotExist()
        {
            using (var httpClient = new HttpClient())
            {
                _resultResponse = await httpClient.GetAsync(_projectURL + _apiPath + Guid.NewGuid());
            }
            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateShouldCreateNewElement()
        {
            var criminal = new Criminal
            {
                Name = "Dimasik",
                Reward = 1,
                Description = "Idiot"
            };

            HttpResponseMessage getResponse = null;

            using (var httpClient = new HttpClient())
            {
                var json = new JavaScriptSerializer().Serialize(criminal);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                _resultResponse = await httpClient.PostAsync(_projectURL + _apiPath, stringContent);

                getResponse = await httpClient.GetAsync(_resultResponse.Headers.Location);
            }

            var result = await getResponse.Content.ReadAsAsync<Criminal>();

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(result.Name, Is.EqualTo(criminal.Name));
            Assert.That(result.Reward, Is.EqualTo(criminal.Reward));
            Assert.That(result.Description, Is.EqualTo(criminal.Description));
        }

        [Test]
        public async Task ReplaceShouldReplaceElementWithSameId()
        {
            HttpResponseMessage getResponse = null;

            using (var httpClient = new HttpClient())
            {
                var criminal = new Criminal
                {
                    Name = "Dimasik",
                    Reward = 1,
                    Description = "Idiot"
                };

                var json = new JavaScriptSerializer().Serialize(criminal);
                var stringContent1 = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var postResponse = await httpClient.PostAsync(_projectURL + _apiPath, stringContent1);

                var locationString = postResponse.Headers.Location.ToString();
                criminal.ID = Guid.Parse(locationString.Substring(locationString.LastIndexOf('/') + 1));
                criminal.Name = "Petr";
                criminal.Reward = 2;
                criminal.Description = "Something";

                json = new JavaScriptSerializer().Serialize(criminal);
                var stringContent2 = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                _resultResponse = await httpClient.PutAsync(_projectURL + _apiPath, stringContent2);

                getResponse = await httpClient.GetAsync(_resultResponse.Headers.Location);
            }

            var result = await getResponse.Content.ReadAsAsync<Criminal>();

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(result.Name, Is.EqualTo("Petr"));
            Assert.That(result.Reward, Is.EqualTo(2));
            Assert.That(result.Description, Is.EqualTo("Something"));
        }

        [Test]
        public async Task ReplaceShouldReturnNotFound()
        {
            using (var httpClient = new HttpClient())
            {
                var criminal = new Criminal
                {
                    ID = Guid.NewGuid()
                };

                var json = new JavaScriptSerializer().Serialize(criminal);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                _resultResponse = await httpClient.PutAsync(_projectURL + _apiPath, stringContent);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteAllShouldClearData()
        {
            HttpResponseMessage getAllResponse = null;

            using (var httpClient = new HttpClient())
            {
                _resultResponse = await httpClient.DeleteAsync(_projectURL + _apiPath);

                getAllResponse = await httpClient.GetAsync(_projectURL + _apiPath);
            }

            var result = await getAllResponse.Content.ReadAsAsync<Criminal[]>();

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Length, Is.Zero);
        }

        [Test, Order(2)]
        public async Task DeleteShouldDeleteElementByGuid()
        {
            Criminal[] array = null;
            HttpResponseMessage getResponse = null;

            using (var httpClient = new HttpClient())
            {
                var getAllResponse = await httpClient.GetAsync(_projectURL + _apiPath);
                array = await getAllResponse.Content.ReadAsAsync<Criminal[]>();

                _resultResponse = await httpClient.DeleteAsync(_projectURL + _apiPath + array[0].ID);

                getResponse = await httpClient.GetAsync(_projectURL + _apiPath + array[0].ID);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteShouldDoNothing()
        {
            using (var httpClient = new HttpClient())
            {
                _resultResponse = await httpClient.DeleteAsync(_projectURL + _apiPath + Guid.NewGuid().ToString());
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
