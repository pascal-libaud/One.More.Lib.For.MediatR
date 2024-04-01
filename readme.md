
One More Lib For MediatR
========================
[![CI](https://github.com/pascal-libaud/One.More.Lib.For.MediatR/actions/workflows/ci.yml/badge.svg)](https://github.com/pascal-libaud/One.More.Lib.For.MediatR/actions/workflows/ci.yml) [![NuGet](https://img.shields.io/nuget/v/One.More.Lib.For.MediatR.svg)](https://www.nuget.org/packages/One.More.Lib.For.MediatR/)

As the name suggests, a new library that extends MediatR with integrated PipelineBehavior:
- Performance Logger
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

## Performance Logger PipelineBehavior

### How to configure it?

The simplest way:
```csharp
builder.Services.AddMediatRExtensions(cfg => cfg.PerformanceLoggerSupport = true);
```

If you want to configure it:
```csharp
builder.Services.AddMediatRExtensions(cfg => cfg.AddPerformanceLoggerSupport(500));
```

### How yo use it?
Nothing to do. Requests exceeding th threshold will be logged.

##  MemoryCache PipelineBehavior

### How to configure it?

The simplest way:
```csharp
builder.Services.AddMediatRExtensions(cfg => cfg.MemoryCacheSupport = true);
```

If you want a complete configuration:
```csharp
builder.Services.AddMediatRExtensions(cfg => cfg.AddMemoryCacheSupport(slidingExpiration: TimeSpan.FromMinutes(10), priority: CacheItemPriority.Low));
```
All the possible options are:

 -     DateTimeOffset? AbsoluteExpiration
 -     TimeSpan? AbsoluteExpirationRelativeToNow
 -     TimeSpan? SlidingExpiration
 -     CacheItemPriority Priority

You can also create a specific configuration with the same options for a given request:
```csharp
builder.Services.AddMediatRExtensions(cfg =>
{
  cfg.AddMemoryCacheSupport(slidingExpiration: TimeSpan.FromMinutes(10), priority: CacheItemPriority.Low);
  cfg.AddMemoryCacheSupportOverrideFor<GiveRequest>(priority: CacheItemPriority.High);
}
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
builder.Services.AddMediatRExtensions(cfg => cfg.AddFluentValidationSupport(new[] { typeof(Program).Assembly }));
```

### How to use it?
Nothing to do, just create Validation rules. If a rule is not respected a ValidationException will be thrown.

## Retry Pattern with Polly

### How to configure it?

The simplest way:
```csharp
builder.Services.AddMediatRExtensions(cfg => cfg.RetrySupport = true);
```

If you want to configure it:
```csharp
builder.Services.AddMediatRExtensions(cfg => cfg.AddRetrySupport(retryCount: 3, retryDelay: 100));
```
### When to use it?

 - For example, when you call on external resources and the infrastructure can sometimes have temporarey failures.
 - Or when you want to deal with the database and there may be temporary conflicts.

### How to use it?

Just add [RetryPolicy] attribute on requests classes or records.

```csharp
[RetryPolicy]
public class GetProduct : IRequest<Product>
{
    public Guid ProductId { get; set; }
}
```
Or you can override default values by passing them in the RetryPolicy Attribute

```csharp
[RetryPolicy(OverrideRetryCount = true, RetryCount = 5)]
public class GetProduct : IRequest<Product>
{
    public Guid ProductId { get; set; }
}
```

```csharp
[RetryPolicy(OverrideRetryDelay = true, RetryDelay = 200)]
public class GetProduct : IRequest<Product>
{
    public Guid ProductId { get; set; }
}
```
