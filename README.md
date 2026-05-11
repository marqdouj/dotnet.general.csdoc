# dotnet.general.csdoc

## Summary
Resolves some of the documentation for a Class/Enum and it's members.
Documentation is dervied from `DisplayAttribute` and the assembly xml documentation file (if it exists).

## Features
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
- `v10.4.2`
 - `ICSDocument`.
	- Added logic to handle generic method parameters, i.e. MyMethod<T, P>(T v1, bool flag, P p1), etc.

- `v10.4.1`
 - `ICSDocument`.
	- `GetItem`. Will now return the method that has parameters if it is the only method with the name. Parameters dot not need to be supplied.

- `v10.4.0`
  - `General`.
	- `Parameters`. Most items now have a `Parameters` property; which represents the XmlDocument parameters signature.
 - `ICSDocument`.
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
	```

- `v10.3.0`
  - `ICSDocumentReader`.
	- `ICSDocument CreateDocument(Type type, bool allMembers = true)`. Added method to handle types	that can't be used with `<T>` i.e. static classes.

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
