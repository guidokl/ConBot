using System;
using System.Collections.Generic;
using System.Text;

namespace ConBot.Providers
{
    public interface IAiProvider
    {
        Task<string> GetCommandAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
