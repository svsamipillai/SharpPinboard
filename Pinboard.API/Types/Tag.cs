using System.Collections.Generic;
using System.Xml.Serialization;

namespace Pinboard.Types
{
    public class Tag
    {
        private string _tag;

        public Tag(string _tag)
        {
            this._tag = _tag;
        }

        /// <summary>
        /// Needed for Xml Serialization
        /// </summary>
        public Tag()
        {
        }

        public string count { get; set; }
        public string tag { get; set; }
    }

    [XmlRoot(ElementName = "tag")]
    public class XmlTag
    {
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
    }

    [XmlRoot(ElementName = "tags")]
    public class XmlTags
    {
        [XmlElement(ElementName = "tag")]
        public List<XmlTag> Tag { get; set; }
    }

}