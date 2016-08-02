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
    [Ignore("")]
    public class RequestTests
    {
        private const string _projectURL = "http://localhost:49457/";
        private const string _apiPath = "api/criminal/";
        private HttpResponseMessage _resultResponse;
        private Criminal _criminal;
        private Criminal[] _allCriminals;

        [OneTimeSetUp]
        public void Setup()
        {
            _resultResponse = null;

            _criminal = new Criminal
            {
                Name = "Dimasik",
                Reward = 1,
                Description = "Idiot"
            };

            _allCriminals = null;
        }

        [Test]
        [Order(0)]
        public async Task GetAllShouldReturnAllElements()
        {
            using (var httpClient = new HttpClient())
            {
                _resultResponse = await httpClient.GetAsync(_projectURL + _apiPath);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetShouldReturnCriminalByItsGuid()
        {
            using (var httpClient = new HttpClient())
            {
                var json = new JavaScriptSerializer().Serialize(_criminal);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var postResponse = await httpClient.PostAsync(_projectURL + _apiPath, stringContent);

                var getAllResponse = await httpClient.GetAsync(_projectURL + _apiPath);
                _allCriminals = await getAllResponse.Content.ReadAsAsync<Criminal[]>();
                _resultResponse = await httpClient.GetAsync(_projectURL + _apiPath + _allCriminals[0].ID);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
            using (var httpClient = new HttpClient())
            {
                var json = new JavaScriptSerializer().Serialize(_criminal);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                _resultResponse = await httpClient.PostAsync(_projectURL + _apiPath, stringContent);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task ReplaceShouldReplaceElementWithSameId()
        {
            using (var httpClient = new HttpClient())
            {
                var json = new JavaScriptSerializer().Serialize(_criminal);
                var stringContent1 = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var postResponse = await httpClient.PostAsync(_projectURL + _apiPath, stringContent1);

                var locationString = postResponse.Headers.Location.ToString();
                _criminal.ID = Guid.Parse(locationString.Substring(locationString.LastIndexOf('/') + 1));
                _criminal.Name = "Petr";
                _criminal.Reward = 2;
                _criminal.Description = "Something";

                json = new JavaScriptSerializer().Serialize(_criminal);
                var stringContent2 = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                _resultResponse = await httpClient.PutAsync(_projectURL + _apiPath, stringContent2);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task ReplaceShouldReturnNotFound()
        {
            using (var httpClient = new HttpClient())
            {
                _criminal = new Criminal
                {
                    ID = Guid.NewGuid()
                };

                var json = new JavaScriptSerializer().Serialize(_criminal);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                _resultResponse = await httpClient.PutAsync(_projectURL + _apiPath, stringContent);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteAllShouldClearData()
        {
            using (var httpClient = new HttpClient())
            {
                _resultResponse = await httpClient.DeleteAsync(_projectURL + _apiPath);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task DeleteShouldDeleteElementByGuid()
        {
            using (var httpClient = new HttpClient())
            {
                var json = new JavaScriptSerializer().Serialize(_criminal);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var postResponse = await httpClient.PostAsync(_projectURL + _apiPath, stringContent);

                var getAllResponse = await httpClient.GetAsync(_projectURL + _apiPath);
                _allCriminals = await getAllResponse.Content.ReadAsAsync<Criminal[]>();

                _resultResponse = await httpClient.DeleteAsync(_projectURL + _apiPath + _allCriminals[0].ID);
            }

            Assert.That(_resultResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
