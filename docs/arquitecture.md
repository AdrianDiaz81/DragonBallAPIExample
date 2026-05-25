1. Vertical Slice Architecture (VSA)
Cohesion: Check if the endpoint, request, response, and logic are in the same folder/slice.
Independence: Ensure this slice doesn't reuse Request/Response models from other slices.
MinApiLib: Verify the use of PostEndpoint<T>, GetEndpoint<T>, etc., and that Configure() sets the correct OpenAPI metadata.
2. C# & .NET 9 Standards
Async/Await: Verify that CancellationToken is passed to all asynchronous calls (DB, API, etc.).
Logging: Ensure LoggerMessageAttribute is used for structured logging instead of simple _logger.LogInformation().
Validation: Check if WithValidation() is called in the slice configuration and a FluentValidation class exists.
DI: Ensure [RegisterInIServiceCollection] is present on the handler record.
3. Testing
Integration: Verify there is a test exercising the slice via DelegateAsync.
Mocks: Check that Moq is used only for external infrastructure, not for domain logic.
