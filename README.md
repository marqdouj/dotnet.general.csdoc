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
