### Roslyn analyzer

<img src="https://github.com/blowin/BlowinCleanCode/blob/master/icon.png" width="80" height="80">

![Build](https://github.com/blowin/BlowinCleanCode/actions/workflows/dotnet.yml/badge.svg)

| Source      | Link |
| ----------- | ----------- |
| VSIX        | [![VSIX](https://img.shields.io/visual-studio-marketplace/i/Blowin.1)](https://marketplace.visualstudio.com/items?itemName=Blowin.1)       |
| VSIX(VS22)  | [![VSIX](https://img.shields.io/visual-studio-marketplace/i/Blowin.BlowinCleanCodeVS22)](https://marketplace.visualstudio.com/items?itemName=Blowin.BlowinCleanCodeVS22)     |
| Nuget       | [![NUGET package](https://img.shields.io/nuget/v/Blowin.CleanCode.svg)](https://www.nuget.org/packages/Blowin.CleanCode/)        |

## Introduction

BlowinCleanCode is a Roslyn-based C# code analyzer that aims to provide a set of rules that helps to simplify code and make it cleaner.

[Changelog](https://github.com/blowin/BlowinCleanCode/blob/master/changelog.md)

## Available analyses

### Single responsibility
* Method contain 'And'
* Control flag
* Long method
* Many parameter in method
* Method contains a lot of declaration
* Too many chained references
* Large class
* Large number of fields in types
* Lambda have too many lines

### Encapsulation
* Don't use public static field

### Good practice
* Don't return null                                    
* Don't use static class                               
* Disposable member in non disposable class            
* Switch statements should have at least 2 case clauses
* Finalizers should not be empty                       
* Type that provide Equals should implement IEquatable
* 'ThreadStatic" fields should not be initialized.

### Code smell
* Nested ternary operator                  
* Complex condition                        
* Magic value                              
* Preserve whole object                    
* Hollow type name                         
* Deeply nested                            
* Switch should not have a lot of cases    
* Switch statements should not be nested   
* Catch should do more than rethrow        
* Empty 'default' clauses should be removed
* [Middle man](https://refactoring.guru/smells/middle-man)
