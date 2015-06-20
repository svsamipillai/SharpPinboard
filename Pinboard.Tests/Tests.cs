using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinboard.Types;
using RestSharp;
using Moq;
using RestSharp.Deserializers;
using Pinboard.Helpers;
using Pinboard.BookmarkFile;

namespace Pinboard.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void Test403()
        {
            var client = new Mock<RestClient>();
            client.Setup(x => x.Execute<List<Post>>(It.IsAny<IRestRequest>()))
                .Returns(new RestResponse<List<Post>> { StatusCode = HttpStatusCode.Unauthorized });
            Pinboard.API api = new API("", "", "", client.Object);
            api.GetAllPostsAsync();
        }

        [TestMethod]
        public void TestTagDeserialization()
        {
            var client = new Mock<RestClient>();
            string tags = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
                <tags>
	                <tag count=""1"" tag="""" />
	                <tag count=""5"" tag="".net"" />
	                <tag count=""2"" tag=""accessibility"" />
	                <tag count=""1"" tag=""acer"" />
	                <tag count=""2"" tag=""ai"" />
	                <tag count=""11"" tag=""android"" />
                </tags>";
            client.Setup(
                x => x.Execute<List<Tag>>(It.IsAny<IRestRequest>()))
                      .Returns(new RestResponse<List<Tag>> { StatusCode = HttpStatusCode.OK, Content = tags, ContentType = "text/xml" }
                      );
            var response = client.Object.Execute<List<Tag>>(new RestRequest());
            XmlDeserializer s = new XmlDeserializer();
            var data = s.Deserialize<List<Tag>>(response);
            Assert.IsTrue(data.Count == 6);
            Assert.IsTrue(data[2].tag == "accessibility");
            Assert.IsTrue(data[2].Count == 2);
        }

        [TestMethod]
        public void TestValueToGet()
        {
            //var stringParam = ParameterHelpers.ValueToGetArgument("string", "string");
            var boolParam = ParameterHelpers.ValueToGetArgument("didThisWork", true);
            var dateTimeParam = ParameterHelpers.ValueToGetArgument("letsParty", new DateTime(1999, 12, 31));
            var tags = new List<Tag>();
            tags.Add(new Tag("something"));
            tags.Add(new Tag("else"));
            var tagParam = ParameterHelpers.ValueToGetArgument("tags", tags);
            Assert.AreEqual(boolParam.ToString(), "didThisWork=yes");
            Assert.AreEqual(dateTimeParam.ToString(), "letsParty=1999-12-31T00:00:00Z");
            Assert.AreEqual(tagParam.ToString(),"tags=something else");
        }


        [TestMethod]
        public void TestReadFromFile()
        {
            var reader = new Reader(@"C:\Userdata\Downloads\delicious.html");
            var tags = reader.GetTags();
        }
    }
}
