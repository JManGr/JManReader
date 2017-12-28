using System;
using System.Collections.Generic;
using System.Text;

namespace JManReader.Model
{
    // HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.daisy.org/z3986/2005/ncx/", IsNullable = false)]
    public partial class ncx
    {

        private ncxMeta[] headField;

        private ncxDocTitle docTitleField;

        private ncxNavPoint[] navMapField;

        private string versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("meta", IsNullable = false)]
        public ncxMeta[] head
        {
            get => this.headField;
            set => this.headField = value;
        }

        /// <remarks/>
        public ncxDocTitle docTitle
        {
            get => this.docTitleField;
            set => this.docTitleField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("navPoint", IsNullable = false)]
        public ncxNavPoint[] navMap
        {
            get => this.navMapField;
            set => this.navMapField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get => this.versionField;
            set => this.versionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxMeta
    {

        private string nameField;

        private string contentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string content
        {
            get => this.contentField;
            set => this.contentField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxDocTitle
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get => this.textField;
            set => this.textField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxNavPoint
    {

        private ncxNavPointNavLabel navLabelField;

        private ncxNavPointContent contentField;

        private ncxNavPointNavPoint[] navPointField;

        private string idField;

        private byte playOrderField;

        /// <remarks/>
        public ncxNavPointNavLabel navLabel
        {
            get => this.navLabelField;
            set => this.navLabelField = value;
        }

        /// <remarks/>
        public ncxNavPointContent content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("navPoint")]
        public ncxNavPointNavPoint[] navPoint
        {
            get => this.navPointField;
            set => this.navPointField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte playOrder
        {
            get => this.playOrderField;
            set => this.playOrderField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxNavPointNavLabel
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get => this.textField;
            set => this.textField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxNavPointContent
    {

        private string srcField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string src
        {
            get => this.srcField;
            set => this.srcField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxNavPointNavPoint
    {

        private ncxNavPointNavPointNavLabel navLabelField;

        private ncxNavPointNavPointContent contentField;

        private string idField;

        private byte playOrderField;

        /// <remarks/>
        public ncxNavPointNavPointNavLabel navLabel
        {
            get => this.navLabelField;
            set => this.navLabelField = value;
        }

        /// <remarks/>
        public ncxNavPointNavPointContent content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte playOrder
        {
            get => this.playOrderField;
            set => this.playOrderField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxNavPointNavPointNavLabel
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get => this.textField;
            set => this.textField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.daisy.org/z3986/2005/ncx/")]
    public partial class ncxNavPointNavPointContent
    {

        private string srcField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string src
        {
            get => this.srcField;
            set => this.srcField = value;
        }
    }


}
