using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;

namespace One.More.Lib.For.MediatR;

public partial class MediatRExtensionConfiguration
{
    internal bool FluentValidationSupport { get; set; } = false;

    internal IEnumerable<Assembly> Assemblies { get; private set; } = new List<Assembly>();

    internal Func<AssemblyScanner.AssemblyScanResult, bool> Filter { get; private set; } = null;

    internal bool IncludeInternalTypes { get; private set; } = false;

    public MediatRExtensionConfiguration AddFluentValidationSupport(IEnumerable<Assembly> assemblies, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, bool includeInternalTypes = false)
    {
        FluentValidationSupport = true;
        Assemblies = assemblies;
        Filter = filter;
        IncludeInternalTypes = includeInternalTypes;

        return this;
    }
}