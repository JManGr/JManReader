using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable All

namespace JManReader.Model
{


// HINWEIS: Für den generierten Code ist möglicherweise mindestens .NET Framework 4.5 oder .NET Core/Standard 2.0 erforderlich.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.idpf.org/2007/opf", IsNullable = false)]
    public partial class package
    {

        private packageMetadata metadataField;

        private packageItem[] manifestField;

        private packageSpine spineField;

        private packageReference[] guideField;

        private string uniqueidentifierField;

        private decimal versionField;

        /// <remarks/>
        public packageMetadata metadata
        {
            get => this.metadataField;
            set => this.metadataField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("item", IsNullable = false)]
        public packageItem[] manifest
        {
            get => this.manifestField;
            set => this.manifestField = value;
        }

        /// <remarks/>
        public packageSpine spine
        {
            get => this.spineField;
            set => this.spineField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("reference", IsNullable = false)]
        public packageReference[] guide
        {
            get => this.guideField;
            set => this.guideField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unique-identifier")]
        public string uniqueidentifier
        {
            get => this.uniqueidentifierField;
            set => this.uniqueidentifierField = value;
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageMetadata
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("creator", typeof(creator), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("date", typeof(date), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("identifier", typeof(identifier), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("language", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("publisher", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("rights", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("title", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [System.Xml.Serialization.XmlElementAttribute("meta", typeof(packageMetadataMeta))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get => this.itemsField;
            set => this.itemsField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get => this.itemsElementNameField;
            set => this.itemsElementNameField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class creator
    {

        private string fileasField;

        private string roleField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("file-as", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string fileas
        {
            get => this.fileasField;
            set => this.fileasField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string role
        {
            get => this.roleField;
            set => this.roleField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class date
    {

        private string eventField;

        private System.DateTime valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string @event
        {
            get => this.eventField;
            set => this.eventField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "date")]
        public System.DateTime Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class identifier
    {

        private string idField;

        private string schemeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string scheme
        {
            get => this.schemeField;
            set => this.schemeField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageMetadataMeta
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.idpf.org/2007/opf", IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:creator")] creator,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:date")] date,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:identifier")] identifier,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:language")] language,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:publisher")] publisher,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:rights")] rights,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://purl.org/dc/elements/1.1/:title")] title,

        /// <remarks/>
        meta,
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageItem
    {

        private string hrefField;

        private string idField;

        private string mediatypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
        {
            get => this.hrefField;
            set => this.hrefField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get => this.idField;
            set => this.idField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("media-type")]
        public string mediatype
        {
            get => this.mediatypeField;
            set => this.mediatypeField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageSpine
    {

        private packageSpineItemref[] itemrefField;

        private string tocField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("itemref")]
        public packageSpineItemref[] itemref
        {
            get => this.itemrefField;
            set => this.itemrefField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string toc
        {
            get => this.tocField;
            set => this.tocField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageSpineItemref
    {

        private string idrefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string idref
        {
            get => this.idrefField;
            set => this.idrefField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageReference
    {

        private string hrefField;

        private string titleField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
        {
            get => this.hrefField;
            set => this.hrefField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title
        {
            get => this.titleField;
            set => this.titleField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get => this.typeField;
            set => this.typeField = value;
        }
    }
}

