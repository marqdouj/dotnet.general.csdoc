using Marqdouj.DotNet.General.CsDoc;
using Sandbox.XmlLib;
using Sandbox.XmlLib.Models;

namespace Tests
{
    [TestClass]
    public sealed class ClassDocTests
    {
        private static readonly CSDocumentReader cdReader = new("Sandbox.XmlLib");

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            cdReader.LoadXml();
        }

        [TestMethod]
        public void CDM_LoadXml()
        {
            var loaded = cdReader.LoadXml();

            if (cdReader.LoadXmlException != null)
                Console.WriteLine($"{cdReader.LoadXmlException.Message}. {cdReader.LoadXmlException.InnerException?.Message}");
            
            Assert.IsTrue(loaded);
            Assert.IsTrue(cdReader.XmlLoaded);
            Assert.IsNull(cdReader.LoadXmlException);
        }

        [TestMethod]
        public void CDocument_Add()
        {
            const string displayName = "MyClass Name";
            const string summary = "My test class.";

            var doc = cdReader.CreateDocument<MyClass>();

            Assert.IsNotNull(doc);
            Assert.HasCount(2, doc.Items);
            Assert.AreEqual(summary, doc.Items.First().Comment?.Summary);
            Assert.IsNotNull(doc.Items.First().DisplayAttribute);
            Assert.AreEqual(displayName, doc.Items.First().DisplayAttribute?.Name);
        }

        [TestMethod]
        public void CDocument_Add_NotAll()
        {
            const string displayName = "MyClass Name";
            const string summary = "My test class.";

            var doc = cdReader.CreateDocument<MyClass>(false);

            Assert.IsNotNull(doc);
            Assert.HasCount(2, doc.Items);
            Assert.AreEqual(summary, doc.Items.First().Comment?.Summary);
            Assert.IsNotNull(doc.Items.First().DisplayAttribute);
            Assert.AreEqual(displayName, doc.Items.First().DisplayAttribute?.Name);
        }

        [TestMethod]
        public void CDocument_Add_Generic()
        {
            const string displayName = "MyClass{T} Name";
            const string summary = "Typed version of MyClass.";

            var doc = cdReader.CreateDocument<MyClass<MyClass>>();

            Assert.IsNotNull(doc);
            Assert.HasCount(4, doc.Items);
            Assert.AreEqual(summary, doc.Items.First().Comment?.Summary);
            Assert.IsNotNull(doc.Items.First().DisplayAttribute);
            Assert.AreEqual(displayName, doc.Items.First().DisplayAttribute?.Name);

            var docItem = doc.GetItem(".ctor");
            Assert.IsNotNull(docItem);
            Assert.AreEqual("The constructor.", docItem.Comment?.Summary);

            docItem = doc.GetItem(nameof(MyClass<>.Name));
            Assert.IsNotNull(docItem);
            Assert.AreEqual("The Name", docItem.DisplayAttribute?.Name);
            Assert.AreEqual("The name.", docItem.Comment?.Summary);

            docItem = doc.GetItem(nameof(MyClass<>.DoStuff));
            Assert.IsNotNull(docItem);
            Assert.AreEqual("Do some stuff.", docItem.Comment?.Summary);
            Assert.AreEqual("Do stuff remarks.", docItem.Comment?.Remarks);
            Assert.AreEqual("Do stuff returns.", docItem.Comment?.Returns);
        }

        [TestMethod]
        public void CDocument_Add_Generic_Enum()
        {
            var doc = cdReader.CreateDocument<MyEnum>();

            Assert.IsNotNull(doc);
            Assert.HasCount(4, doc.Items);

        }

        [TestMethod]
        public void CDocument_Add_Generic_Enum_NotAll()
        {
            var doc = cdReader.CreateDocument<MyEnum>(false);

            Assert.IsNotNull(doc);
            Assert.HasCount(3, doc.Items);

        }

        [TestMethod]
        public void CDocument_Add_InvalidAssembly()
        {
            Assert.Throws<Exception>(() => cdReader.CreateDocument<CSDocumentReader>());
        }

        [TestMethod]
        public void CDocument_Add_Interface()
        {
            var doc = cdReader.CreateDocument(typeof(IMyModelClass));
            var docItem = doc.GetItem(nameof(IMyModelClass.Alias));

            Assert.IsNotNull(doc);
            Assert.HasCount(7, doc.Items);
            Assert.IsNotNull(docItem);
            Assert.AreEqual("The Alias value.", docItem.Comment?.Summary);
        }
    }
}
