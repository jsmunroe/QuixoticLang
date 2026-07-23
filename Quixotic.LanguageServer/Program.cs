using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Server;
using Quixotic.LanguageServer.Services;

namespace Quixotic.LanguageServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = await OmniSharp.Extensions.LanguageServer.Server.LanguageServer.From(options =>
            {
                options
                    .WithServices(services =>
                    {
                        services.AddSingleton<DocumentManager>();
                        services.AddSingleton<CompilationService>();
                    })

                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())

                    .OnInitialize(async (server, request, cancellationToken) =>
                    {
                        await Console.Error.WriteLineAsync("Initialize");
                    })

                    .OnInitialized(async (server, request, response, cancellationToken) =>
                    {
                        await Console.Error.WriteLineAsync("Initialized");
                    })

                    .OnStarted(async (languageServer, cancellationToken) =>
                    {
                        await Console.Error.WriteLineAsync("Started");
                    });
            });

            await server.WaitForExit;
        }
    }
}
