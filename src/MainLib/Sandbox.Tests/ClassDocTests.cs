using Marqdouj.DotNet.General.CsDoc;
using Sandbox.XmlLib;
using Sandbox.XmlLib.Models;

namespace Sandbox.Tests
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
            Assert.HasCount(5, doc.Items);

            var docItem = doc.Items.First();

            Assert.IsNotNull(docItem);
            Assert.AreEqual(summary, docItem.Comment?.Summary);
            Assert.IsNotNull(doc.Items.First().DisplayAttribute);
            Assert.AreEqual(displayName, doc.Items.First().DisplayAttribute?.Name);

            docItem = doc.GetItem(".ctor");
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
            Assert.HasCount(10, doc.Items);
            Assert.IsNotNull(docItem);
            Assert.AreEqual("The Alias value.", docItem.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_InheritCRef_MyItem()
        {
            var doc = cdReader.CreateDocument<MyModelClass>();
            var docItem = doc.GetItem(nameof(MyModelClass.MyItem));

            Assert.IsNotNull(docItem);
            Assert.AreEqual("MyItem that does stuff. This is MyModelClassItem.", docItem.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_InheritCRef_MyItem2()
        {
            var doc = cdReader.CreateDocument<MyModelClass>();
            var docItem = doc.GetItem(nameof(MyModelClass.MyItem2));

            Assert.IsNotNull(docItem);
            Assert.AreEqual("MyItem that does other stuff. This is MyModelClassItem.", docItem.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_InheritCRef_MyItem3()
        {
            var doc = cdReader.CreateDocument<MyModelClass>();
            var docItem = doc.GetItem(nameof(MyModelClass.MyItem3));

            Assert.IsNotNull(docItem);
            Assert.AreEqual("This is MyModelClassItem.", docItem.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_InheritCRef_MyItem4()
        {
            var doc = cdReader.CreateDocument<MyModelClass>();
            var docItem = doc.GetItem(nameof(MyModelClass.MyItem4));

            Assert.IsNotNull(docItem);
            Assert.AreEqual("MyItem4 does stuff. This is MyModelClassItem.", docItem.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_NameAlias()
        {
            var doc = cdReader.CreateDocument<MyModelEnum>();
            var docItem = doc.GetItem(nameof(MyModelEnum));

            Assert.IsNotNull(docItem);
            Assert.AreEqual(nameof(MyModelEnum), docItem.Name);
            Assert.AreEqual(nameof(MyModelEnum), docItem.DisplayName);

            var aliasName = "My Alias Name";
            docItem.NameAlias = aliasName;
            Assert.AreEqual(aliasName, docItem.NameAlias);
            Assert.AreEqual(aliasName, docItem.DisplayName);
        }

        [TestMethod]
        public void CDocument_NameAlias_WithDisplay()
        {
            var doc = cdReader.CreateDocument<MyModelEnum>();
            var docItem = doc.GetItem(nameof(MyModelEnum.FirstValue));
            var firstValue = "First Value";

            Assert.IsNotNull(docItem);
            Assert.AreEqual(firstValue, docItem.DisplayName);

            var aliasName = "My Alias Name";
            docItem.NameAlias = aliasName;
            Assert.AreEqual(aliasName, docItem.NameAlias);
            Assert.AreEqual(firstValue, docItem.DisplayName);
        }

        [TestMethod]
        public void CDocument_Static_Class()
        {
            var doc = cdReader.CreateDocument(typeof(MyStaticClass));
            var docItem = doc.GetItem(nameof(MyStaticClass.GetMyString));
            var docItem2 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), "(System.Double,System.Double)");
            var docItem3 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), "(System.Double,System.Double,System.Double)");

            Assert.IsNotNull(doc);
            Assert.HasCount(5, doc.Items);
            Assert.IsNotNull(docItem);
            Assert.IsNotNull(docItem2);
            Assert.IsNotNull(docItem3);
            Assert.AreEqual("Gets my string.", docItem.Comment?.Summary);
            Assert.AreEqual("Gets my other string.", docItem2.Comment?.Summary);
            Assert.AreEqual("Gets my other string with z.", docItem3.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_Static_GetItems()
        {
            var doc = cdReader.CreateDocument(typeof(MyStaticClass));
            var items = doc.GetItems(nameof(MyStaticClass.GetMyOtherString));
            var docItem1 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), items[0].Parameters);
            var docItem2 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), items[1].Parameters);

            Assert.HasCount(2, items);
            Assert.IsNotNull(docItem1);
            Assert.IsNotNull(docItem2);
            Assert.AreEqual("Gets my other string.", docItem1.Comment?.Summary);
            Assert.AreEqual("Gets my other string with z.", docItem2.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_Static_Class_SingleWithParameters()
        {
            var doc = cdReader.CreateDocument(typeof(MyStaticClass));
            var docItem = doc.GetItem(nameof(MyStaticClass.GetMySingleMethodWithParams));

            Assert.IsNotNull(doc);
            Assert.IsNotNull(docItem);
            Assert.AreEqual("Gets my other string.", docItem.Comment?.Summary);
        }

        [TestMethod]
        public void CDocument_Interface_Methods()
        {
            var doc = cdReader.CreateDocument<IMyFullModel>();
            var items = doc.GetItems(nameof(IMyFullModel.MyTest));

            Assert.HasCount(7, items);

            foreach (var item in items)
            {
                Assert.IsNotNull(item.Comment);
                Console.WriteLine($"{item.Name} {item.Parameters}");
                Console.WriteLine($"{item.Name} {item.Comment.Summary}");
            }

            var docItem = doc.GetItem(nameof(IMyFullModel.MyTest));
            Assert.IsNotNull(docItem);
        }
    }
}
