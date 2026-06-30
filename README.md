# dotnet.general.csdoc

## Features (New)
-  A simpler implementation for reading an XML documentation file.
	- `XmlDocumentReader`
	  - Loads all member nodes found in the file. They can be accessed using the 'Members' property. 
	  - Search for members using the `GetMembers` method for a `Type`, or implement your own custom search for items in the `Members` collection.
	  - Improved logic for resolving comments that contain items like `cref`, `inheritdoc`, `see`, etc.

## Features (Legacy)
-  Resolves some of the documentation for a Class/Enum and it's members. 
   Documentation is dervied from `DisplayAttribute` and the assembly xml documentation file (if it exists).

- `Classes`.
  - `CSDocumentReader`. Creates instances of `CSDocument`.
  - `CSDocument`. Manages a collection of `CSDocumentItem` for a class or enum.
  - `CSDocumentItem`. Container for `DisplayAttribute` and `CSDocumentXml`'s for a class/enum type or one of it's supported members.
  - `CSDocumentXml`. Values extracted from the xml documentation file for a class/enum and of it's supported members.
  - Currently supported `MemberTypes` are `Constructor`, `Method`, and `Property`, 

- `Extensions`
  - `GetDisplayAttribute()/GetDisplayName()`. Get the `DisplayAttribute/Name` for supported objects.
	- Currently supported objects are `Enum`, `MemberInfo`, `PropertyInfo`, `Type`.

## Releases
- `v10.6.4`
  - `XmlDocumentReader`.
	- `GetMembers`. Added flag to the method for adding a `XmlDocumentMember` instance for missing type members.
  - `XmlDocumentMember`.
	- Added extension method:
	  - `GetParameterByName`. Gets the first parameter that matches the `name`. Case in-sensitive.
    - New Properties:
	  - `NameAlias`.
	  - `DisplayName`.
	  - `DisplayAttribute`.
  - `Sandbox`. Fixed issue with missing xml file when loading `DocumentReader` page.
- `v10.6.3`
  - `XmlDocumentReader`.
	- Added check for circular references when parsing comments.
	- Updated logic for parsing comments.
- `v10.6.2`
  - `XmlDocumentReader`.
	- Fixed issue with parsing `XmlDocumentMember.Name` where the method has parameters.
  - `XmlDocumentMember`.
	- Added extension methods:
	  - `GetMemberByName`. Gets the first member that matches the `name` parameter. Case in-sensitive.
	  - `GetMembersWithName`. Gets all members that start with the `name` parameter. Case in-sensitive.
- `v10.6.1`
  - `XmlDocumentReader`.
	- `LoadXml`. Added optional flag `throwAnyException`. Default is false.
  - `XmlDocumentMember`.
	- `Type`. New property for `TypeInfo` members.
	- `TypeName`. New property for `TypeInfo` members.
	- `GetMembers(string typeName)`. New method for getting members based on the `TypeName`.
- `v10.6.0`
  - `XmlDocumentReader (New)`.
	- A simpler implementation for reading an XML documentation file.
- `v10.5.0`
  - Full re-write on resolving the xml document member element names.
	- The member types now supported are:
	  - MemberTypes.Constructor or MemberTypes.Method => 'M'
	  - MemberTypes.TypeInfo or MemberTypes.NestedType => 'T'
	  - MemberTypes.Event => 'E'
	  - MemberTypes.Field => 'F'
	  - MemberTypes.Property => 'P'

- `v10.4.3`
  - `General`.
	- `Parameters`. Most items now have a `Parameters` property; which represents the XmlDocument parameters signature.
  - `ICSDocument`.
	- Added logic to handle generic method parameters, i.e. MyMethod<T, P>(T v1, bool flag, P p1), etc.
	- `GetItem`. Will now return the method that has parameters if it is the only method with the name. Parameters dot not need to be supplied.
	- `GetItems`. New method. Returns all items with the same name.
	- `GetItem`. Now supports search by name and parameters signature.
	- `Example`:
	```csharp
	var doc = cdReader.CreateDocument(typeof(MyStaticClass));
     var items = doc.GetItems(nameof(MyStaticClass.GetMyOtherString));
     var docItem1 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), items[0].Parameters);
     var docItem2 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), items[1].Parameters);
	```
	You can also hard-code the parameters:
	```csharp
	 var docItem1 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), "(System.Double,System.Double)");
      var docItem2 = doc.GetItem(nameof(MyStaticClass.GetMyOtherString), "(System.Double,System.Double,System.Double)");
  - `ICSDocumentReader`.
	- `ICSDocument CreateDocument(Type type, bool allMembers = true)`. Added method to handle types	that can't be used with `<T>` i.e. can be used with static classes.

- `v10.2.0`
  - `CSDocumentItem`.
	- `NameAlias`. Added property to override the `Name` when resolving `DisplayName`.
	This is useful for providing a name for Enum types (which don't allow a `Display` attribute), i.e. Name = "MyEnumTypeName", NameAlias = "My Name".

- `v10.1.1`
  - Added support for `cref=` within the same xml documentation file.

- `v10.1.0`
  - Added support for Interface types. Includes derived types and their members.

- `v10.0.1`
  - Update project link in package.

- `v10.0.0`
  - Initial release.
