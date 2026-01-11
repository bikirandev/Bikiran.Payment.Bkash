# Contributing Guidelines

Thank you for considering contributing to Bikiran.Payment.Bkash!

## How to Contribute

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Make your changes**
4. **Write or update tests**
5. **Update documentation**
6. **Commit your changes** (`git commit -m 'Add amazing feature'`)
7. **Push to the branch** (`git push origin feature/amazing-feature`)
8. **Open a Pull Request**

## Code Standards

### C# Style

- Follow .NET 9 best practices
- Use modern C# patterns
- Add XML documentation for public APIs
- Use meaningful variable names
- Keep methods focused and small (< 50 lines)

### Example

```csharp
/// <summary>
/// Creates a new payment request.
/// </summary>
/// <param name="request">The payment request details.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>The created payment response.</returns>
/// <exception cref="BkashPaymentException">Thrown when payment creation fails.</exception>
public async Task<BkashCreatePaymentResponse> CreatePaymentAsync(
    BkashCreatePaymentRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(request);
    request.Validate();
    
    // Implementation...
}
```

## Testing

- Write unit tests for new features
- Ensure all tests pass before submitting PR
- Test with sandbox credentials

## Documentation

- Update README.md if adding new features
- Add XML documentation for public APIs
- Update relevant documentation files

## Pull Request Process

1. Update documentation with details of changes
2. Update the CHANGELOG.md
3. The PR will be merged once reviewed and approved

## Areas for Contribution

- Unit tests
- Integration tests
- Additional features (Agreement API, Search API)
- Performance improvements
- Documentation improvements
- Bug fixes

## Questions?

Open an issue or discussion on GitHub.

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
