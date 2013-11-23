using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Pinboard.Types;
using System.IO;
using System.Reflection;

namespace Pinboard.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod] 
        public void DeserializePosts()
        {
            string testpath = new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
            string content = new StreamReader(testpath + "\\data\\recent.json").ReadToEnd();
            ServiceStack.Text.JsonSerializer<List<Post>> serializer = new ServiceStack.Text.JsonSerializer<List<Post>>();
            var posts = serializer.DeserializeFromString(content);
            
        
        }
    }
}
