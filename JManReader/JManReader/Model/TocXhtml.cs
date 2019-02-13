using System;
using System.Collections.Generic;
using System.Text;

namespace JManReader.Model.toc
{


    // HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
    /// <remarks/>
    
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/1999/xhtml", IsNullable = false,ElementName = "html")]
    public partial class TocXhtml
    {

        private htmlHead headField;

        private htmlBody bodyField;

        /// <remarks/>
        public htmlHead head
        {
            get
            {
                return this.headField;
            }
            set
            {
                this.headField = value;
            }
        }

        /// <remarks/>
        public htmlBody body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlHead
    {

        private string titleField;

        private htmlHeadMeta metaField;

        private htmlHeadLink[] linkField;

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public htmlHeadMeta meta
        {
            get
            {
                return this.metaField;
            }
            set
            {
                this.metaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("link")]
        public htmlHeadLink[] link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlHeadMeta
    {

        private string httpequivField;

        private string contentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("http-equiv")]
        public string httpequiv
        {
            get
            {
                return this.httpequivField;
            }
            set
            {
                this.httpequivField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlHeadLink
    {

        private string hrefField;

        private string relField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
        {
            get
            {
                return this.hrefField;
            }
            set
            {
                this.hrefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rel
        {
            get
            {
                return this.relField;
            }
            set
            {
                this.relField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlBody
    {

        private htmlBodyH1 h1Field;

        private htmlBodyDiv divField;

        private string classField;

        /// <remarks/>
        public htmlBodyH1 h1
        {
            get
            {
                return this.h1Field;
            }
            set
            {
                this.h1Field = value;
            }
        }

        /// <remarks/>
        public htmlBodyDiv div
        {
            get
            {
                return this.divField;
            }
            set
            {
                this.divField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlBodyH1
    {

        private string classField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlBodyDiv
    {

        private htmlBodyDivOL olField;

        private string classField;

        /// <remarks/>
        public htmlBodyDivOL ol
        {
            get
            {
                return this.olField;
            }
            set
            {
                this.olField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlBodyDivOL
    {

        private htmlBodyDivOLLI[] liField;

        private string classField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("li")]
        public htmlBodyDivOLLI[] li
        {
            get
            {
                return this.liField;
            }
            set
            {
                this.liField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlBodyDivOLLI
    {

        private htmlBodyDivOLLIA aField;

        private string classField;

        private string idField;

        /// <remarks/>
        public htmlBodyDivOLLIA a
        {
            get
            {
                return this.aField;
            }
            set
            {
                this.aField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/xhtml")]
    public partial class htmlBodyDivOLLIA
    {

        private string hrefField;

        private string classField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
        {
            get
            {
                return this.hrefField;
            }
            set
            {
                this.hrefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }



}
