One More Lib For MediatR
========================

As the name suggests, a new library that extends MediatR with integrated PipelineBehavior :
- Performance
- MemoryCache
- Validation with FluentValidation
- Retry Pattern with Polly

## Installing One.More.Lib.For.MediatR

[Link to the nuget package](https://www.nuget.org/packages/One.More.Lib.For.MediatR/ "nuget package")

We recommend using NuGet:
```
Install-Package One.More.Lib.For.MediatR
```
Or via the .NET Core command line interface:
```
dotnet add package One.More.Lib.For.MediatR
```

## Performance PipelineBehavior

### How to configure it?

The simplest way:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.PerformanceSupport = true);
```

If you want to configure it:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.AddPerformanceSupport(500));
```

### How yo use it?
Nothing to do. Requests will be loggued.

##  MemoryCache PipelineBehavior

### How to configure it?

The simplest way:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.MemoryCacheSupport = true);
```

If you want to configure it:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.AddMemoryCacheSupport(slidingExpiration: TimeSpan.FromMinutes(10), priority: CacheItemPriority.Low));
```

### How yo use it?

Just add [MemoryCache] attribute on requests classes or records.

```csharp
[MemoryCache]
public class GetUserByName : IRequest<User?>
{
    public string Name { get; set; }
}
```

##  Validation with FluentValidation

### How to configure it?

There is not simplest way because you need to enter the assemblies to be scanned for FluentValidation:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.AddFluentValidationSupport(new[] { typeof(Program).Assembly }));
```

### How to use it?
Nothing to do, just create Validation rules. If a rule is not respected a ValidationException will be thrown.

## Retry Pattern with Polly

### How to configure it?

The simplest way:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.RetrySupport = true);
```

If you want to configure it:
```csharp
builder.Services.AddMediatRExtensions(configuration => configuration.AddRetrySupport(retryCount: 3, retryDelay: 100));
```

### How to use it?

Just add [RetryPolicy] attribute on requests classes or records.

```csharp
[RetryPolicy]
public class GetUserByName : IRequest<User?>
{
    public string Name { get; set; }
}
```