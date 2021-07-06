### Roslyn analyzer

## Introduction

BlowinCleanCode is a Roslyn-based C# code analyzer that aims to provide a set of rules that helps to simplify code and make it cleaner.

## Available analyses

| Single responsibility                | Encapsulation                 | Good practice               | Code smell                  |
| :----------------------------------- | ----------------------------- | --------------------------- | --------------------------- |
| Method contain And                   | Don't use public static field | Don't return null           | Nested ternary operator     |
| Control flag                         |                               | Don't use static class      | Complex condition           |
| Long method                          |                               |                             | Magic value                 |
| Many parameter in method             |                               |                             |                             |
| Method contains a lot of declaration |                               |                             |                             |
| Too many chained references          |                               |                             |                             |
