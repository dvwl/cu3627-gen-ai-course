---
layout: default
title: Module 1: Effective Prompting in C#
parent: Exercises
nav_order: 1
---

# Module 1: Effective Prompting in C#
{: .no_toc }

**Duration**: 3 hours (09:00am - 12:30am)
{: .label .label-blue }

This module introduces practical skills for GenAI-powered C# development using GitHub Copilot. Learn prompting basics, OOP, LINQ, and unit testing.

<details open markdown="block">
   <summary>
      Table of contents
   </summary>
   {: .text-delta }
1. TOC
{:toc}
</details>

---

## Learning Outcomes

By the end of this module, you will be able to:
- Write effective prompts for Copilot in C#
- Apply OOP principles with GenAI assistance
- Use LINQ for data manipulation
- Write and run unit tests with NUnit
- Compare Copilot and IntelliSense
- Explore MCP context for GenAI workflows

## Coding Style & Guidelines

- Use PascalCase for class and method names
- Use camelCase for local variables and parameters
- Include XML comments for public members
- Prefer explicit types over `var` unless type is obvious
- Organize code into namespaces and folders by feature
- Validate inputs and log errors; use exceptions for error cases

## Prompting Tips

- Be clear about your intent
- Provide input, output, and constraints
- Use comments and method stubs to guide Copilot
- Iterate and refine prompts for better results
- Experiment

### Example Prompts

```csharp
// Write a C# method to sort a list of strings by length
// Define a C# class 'Employee' with Name, Role, and Salary properties
// Use LINQ to select all books with more than 300 pages
```

## Labs

1. **Prompting Basics**: Learn how to write clear prompts for Copilot in C#
2. **Generate Classes & Interfaces**: Use Copilot to scaffold C# types
3. **LINQ in Action**: Practice querying collections with Copilot-generated LINQ
4. **Add Tests with NUnit**: Prompt Copilot to create and run unit tests
5. **Prompt for Refactoring Existing Code**: Use Copilot to improve and refactor C# code

## References

- [Creating dotnet Console Applicatios](https://learn.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates)
- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)

---

Ready to get started? Dive into the labs below!

---

## Lab 1: Prompting Basics

**Objective:** Learn how to write clear prompts for Copilot in C#.

**Setup Instructions:**
1. Open VS Code and ensure the GitHub Copilot extension is installed and enabled.
2. Create a new folder for your lab work (e.g., `Module1Lab1`).
3. Add a new file named `PromptingBasics.cs`.
4. To run a modern .NET (C#) file without a project, you can use the "top-level statements" feature with the .NET CLI. However, this only works for simple scripts and single files, not for files that require project references, NuGet packages, or ASP.NET features.
If you don't have a project, you can use:
```
dotnet script PromptingBasics.cs
```
But you need the dotnet-script tool installed:
```
dotnet tool install -g dotnet-script
dotnet script PromptingBasics.cs
```
If you are in the workspace root:
```
dotnet script demos\Module1Lab1\PromptingBasics.cs
```

**Task:**
- Write a comment prompt in your C# file and observe Copilot's suggestions.

**Sample Code:**
```csharp
// Write a C# method to reverse a string
```

---

## Lab 2: Generate Classes & Interfaces

**Objective:** Use Copilot to scaffold C# classes and interfaces.

**Setup Instructions:**
1. In your lab folder, create a new file named `Product.cs`.

**Task:**
- Write a comment describing a class and its properties. Let Copilot generate the code.

**Sample Code:**
```csharp
// Define a C# class 'Product' with Id, Name, and Price properties
// Add an IEntity interface with Id property.
// Create a Product class that implements IEntity
// Include constructor and ToString() override
```

---

## Lab 3: LINQ in Action

**Objective:** Practice querying collections with Copilot-generated LINQ.

**Setup Instructions:**
1. Create a new file named `BooksQuery.cs`.

**Task:**
- Use Copilot to help write a LINQ query for a list of books.

**Sample Code:**
```csharp
// Use LINQ to select all books with more than 300 pages
// Sort by Title
// Print results
```

---

## Lab 4: Add Tests with NUnit

**Objective:** Prompt Copilot to create and run unit tests using NUnit.

**Setup Instructions:**
1. Ensure you have the NUnit package installed (`dotnet add package NUnit`).
2. Create a new file named `ProductTests.cs`.

**Task:**
- Write a comment prompt for a test method. Let Copilot generate the test code.

**Sample Code:**
```csharp
// Write an NUnit test for the Product class to check if Price is set correctly
// Write NUnit tests for the Product class constructor and ToString() method
```

---

## Lab 5: Prompt for Refactoring Existing Code

**Objective:** Use Copilot to improve and refactor C# code.

**Setup Instructions:**
1. Create a file named `LegacyCode.cs` and paste in the sample code below.

**Task:**
- Write a comment prompt asking Copilot to refactor the code for readability and performance.

**Sample Code:**
```csharp
// Refactor this method to use LINQ and improve readability
public List<int> GetEvenNumbers(List<int> numbers)
{
   List<int> result = new List<int>();
   foreach (var n in numbers)
   {
      if (n % 2 == 0)
      {
         result.Add(n);
      }
   }
   return result;
}
```

---
