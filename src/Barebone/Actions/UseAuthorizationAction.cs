using System;
using ExtCore.Infrastructure.Actions;
using Microsoft.AspNetCore.Builder;

namespace Barebone.Actions
{
    public class UseAuthorizationAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
        {
            applicationBuilder.UseAuthorization();
        }

        public int Priority => 10001;
    }
}