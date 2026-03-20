---
name: code-review
version: 1.0
description: Analyzes code snippets for bugs, security vulnerabilities, performance problems, and style inconsistencies. Returns a structured markdown report per category. Use this skill whenever someone says "review this code", "check this for bugs", "find security issues", "analyze this snippet", or provides code and asks for feedback on quality, safety, or style.
---

# Code Review Skill

A specialized skill that analyzes code snippets for potential issues in four categories: bugs, security vulnerabilities, performance problems, and style inconsistencies. The skill follows the ABP Framework conventions described in AGENTS.md and returns structured markdown reports.

## Skill Overview

The code review skill accepts code snippets and a review type to generate detailed analysis reports. It's designed to help developers identify issues early in the development process while maintaining security best practices.

## Usage Examples

```bash
# Review for bugs
/code-review --code "function calculateTotal(price, quantity) { return price * quantity; }" --type bug

# Review for security issues
/code-review --code "userInput = window.location.search; eval(userInput)" --type security

# Review for performance problems
/code-review --code "List.All(x => x > 0).Count()" --type performance

# Review for style improvements
/code-review --code "public void DoWork() { var temp = new List(); ... }" --type style
```

## Implementation Structure

The skill follows a clean architecture pattern:

```
src/
└── Amnil.AccessControlManagementSystem.Skills/
    └── CodeReviewSkill/
        ├── CodeReviewSkill.csproj
        └── CodeReviewSkill.cs
```

### Input Parameters

| Parameter | Description | Required | Example Values |
|-----------|-------------|----------|----------------|
| `--code` | The code snippet to analyze | Yes | `function calculateTotal() { ... }` |
| `--type` | Review category | Yes | `bug`, `security`, `performance`, `style` |
| `--language` | Programming language | No | `C#`, `JavaScript`, `TypeScript` |

### Output Format

The skill returns a structured markdown report:

```markdown
## Code Review Report

### {Category}

- **Issue**: description of the issue
  ```code
  // Code snippet demonstrating the issue
  ```
```

## Analysis Process

The skill performs automated static analysis following these principles:

1. **Static Analysis**: Analyzes code without executing it
2. **ABP Compliance**: Follows ABP Framework naming conventions and architecture patterns
3. **Security First**: Prioritizes security vulnerability detection
4. **Performance Monitoring**: Identifies potential performance bottlenecks
5. **Style Adherence**: Checks against established code style guidelines

## Example Reports

### Bug Detection
```markdown
## Bug Detection

- **Potential Null Reference**: 'price' parameter could be null
  ```javascript
  function calculateTotal(price, quantity) {
    return price * quantity; // Missing null check
  }
  ```
```

### Security Issues
```markdown
## Security Issues

- **Code Injection Risk**: User input executed via eval()
  ```csharp
  string userInput = GetUserInput();
  eval(userInput); // Vulnerable to code injection
  ```
```

### Performance Concerns
```markdown
## Performance Issues

- **Inefficient Looping**: Using LINQ Count() on large datasets
  ```csharp
  long count = myCollection.All(x => x > 0).Count(); // Consider pagination
  ```
```

### Style Improvements
```markdown
## Style Improvements

- **Constructor Usage**: Use primary constructor instead of property assignment
  ```csharp
  public CustomService(ICurrentTenant currentTenant)
  {
      _currentTenant = currentTenant;
  }
  ```
```

## Configuration

The skill can be configured via `appsettings.json` with these settings:

```json
{
  "CodeReviewSettings": {
    "MaxIssueComplexity": 5,
    "SensitivityLevel": "High"
  }
}
```

## Implementation Roadmap

1. **Basic Analysis Engine**: Implement core static analysis for bug detection
2. **Security Scanner**: Add vulnerability detection rules
3. **Performance Analyzer**: Integrate performance profiling capabilities
4. **Style Checker**: Configure style rule set based on AGENTS.md conventions
5. **Report Generator**: Build markdown report formatter
6. **CLI Interface**: Create command-line interface following ABP CLI patterns

## Contribution Guidelines

Developers can extend the skill with additional analysis rules by:

1. Adding new rule classes implementing `IAnalysisRule`
2. Updating the rule registry
3. Extending the report format with new sections
4. Following existing code patterns and conventions

All contributions must adhere to the coding standards in AGENTS.md and pass existing tests.