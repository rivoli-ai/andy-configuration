# Contributing to Andy.Configuration

Thank you for your interest in contributing to Andy.Configuration! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Pull Request Process](#pull-request-process)
- [Reporting Issues](#reporting-issues)

## Code of Conduct

We are committed to providing a welcoming and inclusive environment for all contributors. Please:

- Be respectful and constructive in all interactions
- Welcome newcomers and help them get started
- Focus on what is best for the community
- Show empathy towards other community members

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A C# IDE such as:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/)
  - [Visual Studio Code](https://code.visualstudio.com/) with C# extension
  - [JetBrains Rider](https://www.jetbrains.com/rider/)
- Git for version control

### Setting Up Your Development Environment

1. **Fork the repository**
   ```bash
   # Fork via GitHub UI, then clone your fork
   git clone https://github.com/YOUR-USERNAME/andy-configuration.git
   cd andy-configuration
   ```

2. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/rivoli-ai/andy-configuration.git
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Run tests with coverage**
   ```bash
   dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
   ```

## Development Process

### Branching Strategy

We use a feature branch workflow:

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Keep your branch up to date**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

3. **Branch naming conventions**
   - `feature/` - New features
   - `fix/` - Bug fixes
   - `docs/` - Documentation updates
   - `refactor/` - Code refactoring
   - `test/` - Test additions or updates

### Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
type(scope): subject

body

footer
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Test additions or modifications
- `chore`: Build process or auxiliary tool changes

**Examples:**
```bash
feat(validation): add support for async validators
fix(options): correct range validation for Temperature property
docs(api): update ConfigurationValidator documentation
test(integration): add tests for JSON configuration loading
```

## Coding Standards

### C# Coding Conventions

We follow the [.NET coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions):

1. **Naming**
   - Use PascalCase for public members, types, and namespaces
   - Use camelCase for private fields (with underscore prefix) and local variables
   - Use meaningful and descriptive names

2. **Formatting**
   - Use 4 spaces for indentation (no tabs)
   - Place opening braces on new lines
   - Use `var` when the type is obvious
   - Add blank lines between method definitions

3. **Documentation**
   - Add XML documentation comments to all public APIs
   - Include examples in documentation when helpful
   - Keep comments concise and meaningful

### Example Code Style

```csharp
namespace Andy.Configuration;

/// <summary>
/// Validates configuration objects using data annotations.
/// </summary>
public class ConfigurationValidator<T> : IValidateOptions<T> where T : class
{
    private readonly ILogger<ConfigurationValidator<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationValidator{T}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public ConfigurationValidator(ILogger<ConfigurationValidator<T>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ValidateOptionsResult Validate(string? name, T options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("Configuration object cannot be null");
        }

        // Validation logic here
        return ValidateOptionsResult.Success;
    }
}
```

### File Organization

- One type per file
- File name matches the type name
- Organize files in logical folders:
  - `Options/` - Configuration option classes
  - `Validation/` - Validation-related classes
  - `Extensions/` - Extension methods

## Testing Guidelines

### Test Organization

- Unit tests: `tests/Andy.Configuration.Tests/Unit/`
- Integration tests: `tests/Andy.Configuration.Tests/Integration/`
- Test data: `tests/Andy.Configuration.Tests/TestData/`

### Writing Tests

1. **Test naming convention**
   ```
   MethodName_StateUnderTest_ExpectedBehavior
   ```

2. **Use AAA pattern**
   ```csharp
   [Fact]
   public void Validate_WithNullOptions_ReturnsFailure()
   {
       // Arrange
       var validator = new ConfigurationValidator<TestOptions>();
       
       // Act
       var result = validator.Validate(null, null!);
       
       // Assert
       result.Succeeded.Should().BeFalse();
   }
   ```

3. **Test coverage requirements**
   - Maintain at least 90% code coverage
   - Cover edge cases and error scenarios
   - Include both positive and negative test cases

### Test Categories

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **Configuration Tests**: Test configuration loading and binding

## Pull Request Process

### Before Creating a PR

1. **Ensure all tests pass**
   ```bash
   dotnet test
   ```

2. **Check code coverage**
   ```bash
   dotnet test /p:CollectCoverage=true
   ```

3. **Format your code**
   ```bash
   dotnet format
   ```

4. **Update documentation** if you've:
   - Added new public APIs
   - Changed existing behavior
   - Added configuration options

### Creating a Pull Request

1. **Push your branch**
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create PR via GitHub UI**
   - Use a clear, descriptive title
   - Reference any related issues
   - Provide a detailed description
   - Include screenshots for UI changes

3. **PR description template**
   ```markdown
   ## Description
   Brief description of the changes

   ## Type of Change
   - [ ] Bug fix
   - [ ] New feature
   - [ ] Breaking change
   - [ ] Documentation update

   ## Testing
   - [ ] Unit tests pass
   - [ ] Integration tests pass
   - [ ] Code coverage maintained/improved

   ## Checklist
   - [ ] My code follows the project's style guidelines
   - [ ] I have performed a self-review
   - [ ] I have added tests for my changes
   - [ ] I have updated documentation as needed
   ```

### Code Review Process

1. **Automated checks** must pass:
   - Build success
   - All tests passing
   - Code coverage maintained

2. **Reviewer feedback**:
   - Address all feedback constructively
   - Ask questions if something is unclear
   - Update your PR based on feedback

3. **Merging**:
   - PRs require at least one approval
   - Maintainers will merge approved PRs
   - We use squash and merge to keep history clean

## Reporting Issues

### Before Creating an Issue

1. **Search existing issues** to avoid duplicates
2. **Check the documentation** for answers
3. **Verify you're using the latest version**

### Creating an Issue

Use the appropriate issue template:

#### Bug Report
```markdown
**Describe the bug**
A clear description of the bug

**To Reproduce**
Steps to reproduce the behavior:
1. Configure '...'
2. Call method '...'
3. See error

**Expected behavior**
What you expected to happen

**Actual behavior**
What actually happened

**Environment:**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 8.0.100]
- Andy.Configuration Version: [e.g., 1.0.0]

**Additional context**
Any other relevant information
```

#### Feature Request
```markdown
**Is your feature request related to a problem?**
Description of the problem

**Describe the solution**
What you'd like to see implemented

**Alternatives considered**
Other solutions you've considered

**Additional context**
Any other relevant information
```

## Development Tips

### Running Specific Tests

```bash
# Run a specific test class
dotnet test --filter "FullyQualifiedName~ConfigurationValidatorTests"

# Run tests in a specific namespace
dotnet test --filter "FullyQualifiedName~Andy.Configuration.Tests.Unit"

# Run a specific test method
dotnet test --filter "FullyQualifiedName~Validate_WithNullOptions_ReturnsFailure"
```

### Debugging Tests

1. **In Visual Studio**: Set breakpoints and use Test Explorer
2. **In VS Code**: Use the C# extension's test debugging features
3. **Command line**: Use `--logger "console;verbosity=detailed"`

### Performance Considerations

- Avoid unnecessary allocations in hot paths
- Use `StringComparison.Ordinal` for string comparisons
- Profile performance-critical code changes
- Consider using `Span<T>` and `Memory<T>` where appropriate

## Getting Help

If you need help:

1. Check the [documentation](../README.md)
2. Search [existing issues](https://github.com/rivoli-ai/andy-configuration/issues)
3. Ask in [discussions](https://github.com/rivoli-ai/andy-configuration/discussions)
4. Create an issue with your question

## Recognition

Contributors will be:
- Listed in the project's contributors file
- Mentioned in release notes for significant contributions
- Given credit in commit messages and PRs

Thank you for contributing to Andy.Configuration! 🎉