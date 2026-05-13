using Marqdouj.DotNet.General.CsDoc;
using Sandbox.XmlLib;
using Sandbox.XmlLib.Models;

namespace Sandbox.Tests
{
    [TestClass]
    public sealed class CsDocCreateTests
    {
        private static readonly CSDocumentReader cdReader = new("Sandbox.XmlLib");

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            cdReader.LoadXml();
        }

        [TestMethod]
        public void CdReader_LoadXml()
        {
            var loaded = cdReader.LoadXml();

            if (cdReader.LoadXmlException != null)
                Console.WriteLine($"{cdReader.LoadXmlException.Message}. {cdReader.LoadXmlException.InnerException?.Message}");

            Assert.IsTrue(loaded);
            Assert.IsTrue(cdReader.XmlLoaded);
            Assert.IsNull(cdReader.LoadXmlException);
        }

        [TestMethod]
        public void CreateDocument_Interface()
        {
            var doc = cdReader.CreateDocument<IMyFullModel>();
            Assert.IsNotNull(doc);

            foreach (var item in doc.Items)
            {
                Console.WriteLine($"{item.MemberType}:{item.Name} {item.Parameters} {item.Comment?.Summary}");
                Assert.IsNotNull(item.Comment?.Summary);
            }
        }

        [TestMethod]
        public void CreateDocument_Interface_Nested()
        {
            var doc = cdReader.CreateDocument(typeof(IMyModelClass));
            Assert.IsNotNull(doc);

            foreach (var item in doc.Items)
            {
                Console.WriteLine($"{item.MemberType}:{item.Name} {item.Parameters} {item.Comment?.Summary}");
                Assert.IsNotNull(item.Comment?.Summary);
            }
        }

        [TestMethod]
        public void CreateDocument_Enum()
        {
            var doc = cdReader.CreateDocument<MyModelEnum>();
            Assert.IsNotNull(doc);

            foreach (var item in doc.Items)
            {
                //Console.WriteLine($"{item.MemberType}:{item.Name} {item.Parameters} {item.Comment?.Summary}");
                Assert.IsNotNull(item.Comment?.Summary);
            }
        }

        [TestMethod]
        public void CreateDocument_Class()
        {
            var doc = cdReader.CreateDocument<MyFullModel>();
            Assert.IsNotNull(doc);

            foreach (var item in doc.Items)
            {
                Console.WriteLine($"{item.MemberType}:{item.Name} {item.Parameters} {item.Comment?.Summary}");
                Assert.IsNotNull(item.Comment?.Summary);
            }
        }

        [TestMethod]
        public void CreateDocument_Class_Typed()
        {
            var doc = cdReader.CreateDocument<MyClass<MyClass>>();
            Assert.IsNotNull(doc);

            foreach (var item in doc.Items)
            {
                Console.WriteLine($"{item.MemberType}:{item.Name} {item.Parameters} {item.Comment?.Summary}");
                Assert.IsNotNull(item.Comment?.Summary);
            }
        }

        [TestMethod]
        public void CreateDocument_Class_WithField()
        {
            var doc = cdReader.CreateDocument<MyFieldClass>();
            Assert.IsNotNull(doc);

            foreach (var item in doc.Items)
            {
                Console.WriteLine($"{item.MemberType}:{item.Name} {item.Parameters} {item.Comment?.Summary}");
                Assert.IsNotNull(item.Comment?.Summary);
            }
        }
    }
}
