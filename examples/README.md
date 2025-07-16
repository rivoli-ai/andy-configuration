# Andy.Configuration Examples

This directory contains example programs demonstrating various features of the Andy.Configuration library.

## Examples

### 1. [BasicConfiguration](BasicConfiguration/)
A simple console application showing basic configuration setup and usage.

### 2. [ValidationExample](ValidationExample/)
Demonstrates configuration validation features including required fields and range validation.

### 3. [NestedConfiguration](NestedConfiguration/)
Shows how to work with nested configuration objects and hierarchical settings.

### 4. [EnvironmentVariables](EnvironmentVariables/)
Demonstrates loading configuration from environment variables.

### 5. [CustomValidation](CustomValidation/)
Shows how to implement custom validation attributes and validators.

### 6. [WebApiExample](WebApiExample/)
A complete ASP.NET Core Web API example with configuration validation on startup.

## Running the Examples

Each example is a standalone console or web application. To run an example:

```bash
cd examples/BasicConfiguration
dotnet run
```

## Prerequisites

- .NET 8.0 SDK or later
- The Andy.Configuration library (built from source or installed via NuGet)

## Building All Examples

From the examples directory:

```bash
dotnet build
```

This will build all example projects.