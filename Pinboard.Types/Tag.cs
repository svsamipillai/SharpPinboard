using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace Pinboard.Types
{
    public class Tag
    {
        public string tag { get; set; } //lowercased to remove name conflict
        public int? Count { get; set; }

        public Tag(string Name)
        {
           tag = Name;
        }

        public Tag()
        {
            //required for restsharp serializer
        }
    }
}
