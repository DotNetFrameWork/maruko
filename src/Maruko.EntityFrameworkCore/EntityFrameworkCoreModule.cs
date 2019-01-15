﻿using Autofac;
using Maruko.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Maruko.EntityFrameworkCore
{
    public class EntityFrameworkCoreModule : MarukoModule
    {
        public override double Order { get; set; } = 0.3;
    }
}
