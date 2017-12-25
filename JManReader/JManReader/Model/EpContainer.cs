using System;
using System.Collections.Generic;
using System.Text;

namespace JManReader.Model
{

    // HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:opendocument:xmlns:container")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:oasis:names:tc:opendocument:xmlns:container", IsNullable = false)]
    public partial class container
    {

        private containerRootfiles rootfilesField;

        private decimal versionField;

        /// <remarks/>
        public containerRootfiles rootfiles
        {
            get => this.rootfilesField;
            set => this.rootfilesField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get => this.versionField;
            set => this.versionField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:opendocument:xmlns:container")]
    public partial class containerRootfiles
    {

        private containerRootfilesRootfile rootfileField;

        /// <remarks/>
        public containerRootfilesRootfile rootfile
        {
            get => this.rootfileField;
            set => this.rootfileField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:oasis:names:tc:opendocument:xmlns:container")]
    public partial class containerRootfilesRootfile
    {

        private string fullpathField;

        private string mediatypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("full-path")]
        public string fullpath
        {
            get => this.fullpathField;
            set => this.fullpathField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("media-type")]
        public string mediatype
        {
            get => this.mediatypeField;
            set => this.mediatypeField = value;
        }
    }


}
