### Roslyn analyzer

![Build](https://github.com/blowin/BlowinCleanCode/actions/workflows/dotnet.yml/badge.svg)

| Source      | Link |
| ----------- | ----------- |
| VSIX        | [![VSIX](https://img.shields.io/visual-studio-marketplace/i/Blowin.1)](https://marketplace.visualstudio.com/items?itemName=Blowin.1)       |
| Nuget       | [![NUGET package](https://img.shields.io/nuget/v/Blowin.CleanCode.svg)](https://www.nuget.org/packages/Blowin.CleanCode/)        |

## Introduction

BlowinCleanCode is a Roslyn-based C# code analyzer that aims to provide a set of rules that helps to simplify code and make it cleaner.

## Available analyses

| Single responsibility                | Encapsulation                 | Good practice                                         | Code smell                                       |
| :----------------------------------- | ----------------------------- |-------------------------------------------------------|--------------------------------------------------|
| Method contain And                   | Don't use public static field | Don't return null                                     | Nested ternary operator                          |
| Control flag                         |                               | Don't use static class                                | Complex condition                                |
| Long method                          |                               | Disposable member in non disposable class             | Magic value                                      |
| Many parameter in method             |                               | Switch statements should have at least 2 case clauses | Preserve whole object                            |
| Method contains a lot of declaration |                               | Finalizers should not be empty                        | Hollow type name                                 |
| Too many chained references          |                               | Type that provide Equals should implement IEquatable  | Deeply nested                                    |
| Large class                          |                               | "ThreadStatic" fields should not be initialized.      | Switch should not have a lot of cases            |
| Large number of fields in types      |                               |                                                       | Switch statements should not be nested           |
| Lambda have too many lines           |                               |                                                       | Catch should do more than rethrow                |
|                                      |                               |                                                       | Empty 'default' clauses should be removed        |