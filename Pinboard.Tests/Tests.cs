using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Pinboard.Types;
using System.IO;
using System.Reflection;
using RestSharp.Deserializers;

namespace Pinboard.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod] 
        public void DeserializePosts()
        {
            string testpath = new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
            string content = new StreamReader(testpath + "\\data\\delicious.xml").ReadToEnd();
            XmlDeserializer ds = new XmlDeserializer();
            var posts = ds.Deserialize<List<Post>>(new RestResponse() {Content = content});
            Debug.Write("something");


        }
    }
}
