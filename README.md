# dotnet.general.csdoc

## Summary
Resolves some of the documentation for a Class/Enum and it's members.
Documentation is dervied from `DisplayAttribute` and the assembly xml documentation file (if it exists).

## Features
- `Classes`.
  - `CSDocumentReader`. Creates instances of `CSDocument`.
  - `CSDocument`. Manages a collection of `CSDocumentItem` for a class or enum.
  - `CSDocumentItem`. Container for `DisplayAttribute` and `CSDocumentXml`'s for a class/enum type or one of it's supported members.
  - `CSDocumentXml`. Values extracted from the xml documentation file for a class/enum type or one of it's supported members.

- `Extensions`
  - `GetDisplayAttribute()`. Get the `DisplayAttribute` for supported `type`s.

## Releases
- `v10.0.0`
  - Initial release.
