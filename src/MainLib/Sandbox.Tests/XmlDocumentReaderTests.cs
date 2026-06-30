using Marqdouj.DotNet.General.CsDoc;
using Sandbox.XmlLib;
using Sandbox.XmlLib.Models;

namespace Sandbox.Tests
{
    [TestClass]
    public sealed class XmlDocumentReaderTests
    {
        private static readonly XmlDocumentReader docReader = new("Sandbox.XmlLib");

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            docReader.LoadXml();
        }

        [TestMethod]
        public void XmlDocumentReader_LoadXml()
        {
            XmlDocumentReader docReader = new("Sandbox.XmlLib");
            docReader.LoadXml(throwAnyException: true);
        }

        [TestMethod]
        public void XmlDocumentReader_GetMembers_ByType()
        {
            var members = docReader.GetMembers<IMyFullModel>();

            foreach (var item in members)
            {
                Console.WriteLine($"{item.Fullname} - {item.DisplayName}");
            }
            Assert.IsNotNull(members);
            Assert.HasCount(11, members);
        }

        [TestMethod]
        public void XmlDocumentReader_GetMembers_ByType_T()
        {
            var members = docReader.GetMembers(typeof(MyModelClass<>));

            foreach (var item in members)
            {
                Console.WriteLine($"{item.Fullname} - {item.DisplayName}");
            }
            Assert.IsNotNull(members);
            Assert.HasCount(4, members);
        }

        [TestMethod]
        public void XmlDocumentReader_GetMembers_ByType_Enum()
        {
            var members = docReader.GetMembers<MyModelEnum>();

            Assert.IsNotNull(members);
            Assert.HasCount(4, members);
        }

        [TestMethod]
        public void XmlDocumentReader_GetMembers_ByType_Enum_AddMissing()
        {
            var members = docReader.GetMembers<MyEnum>(true);

            Assert.IsNotNull(members);
            Assert.HasCount(4, members);
        }

        [TestMethod]
        public void XmlDocumentReader_GetMembers_ByType_Nested()
        {
            var members = docReader.GetMembers<MyClassWithNested>();

            Assert.IsNotNull(members);
            Assert.HasCount(2, members);
        }

        [TestMethod]
        public void XmlDocumentReader_GetMembers_ByTypeName()
        {
            var type = typeof(IMyFullModel);
            var members = docReader.GetMembers(type.FullName!);

            Assert.IsNotNull(members);
            Assert.HasCount(11, members);
        }

        [TestMethod]
        public void XmlDocumentReader_Members_GetMemberByName()
        {
            var members = docReader.GetMembers<IMyFullModel>();
            var member = members.GetMemberByName(nameof(IMyFullModel.Name));

            Assert.IsNotNull(member);
        }

        [TestMethod]
        public void XmlDocumentReader_Members_GetMembersWithName_Multiple()
        {
            var members = docReader.GetMembers<IMyFullModel>();
            var myTestMembers = members.GetMembersWithName(nameof(IMyFullModel.MyTest));

            Assert.IsNotNull(myTestMembers);
            Assert.HasCount(6, myTestMembers);
        }

        [TestMethod]
        public void XmlDocumentReader_Members_GetMembersWithName_Single()
        {
            var members = docReader.GetMembers<IMyFullModel>();
            var myTestMembers = members.GetMembersWithName(nameof(IMyFullModel.Name));

            Assert.IsNotNull(myTestMembers);
            Assert.HasCount(1, myTestMembers);
        }
    }
}
