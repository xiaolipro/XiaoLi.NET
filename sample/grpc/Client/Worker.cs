#region Copyright notice and license

// Copyright 2019 The gRPC Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Diagnostics;
using Test;

namespace Client;

#region snippet_Worker
public class Worker : BackgroundService
{
    private readonly Tester.TesterClient _client;
    private readonly IGreetRepository _greetRepository;

    public Worker(Tester.TesterClient client, IGreetRepository greetRepository)
    {
        _client = client;
        _greetRepository = greetRepository;
    }

    [DebuggerStepThrough]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var count = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            count++;

            try
            {
                var reply = await _client.SayHelloUnaryAsync(new HelloRequest { Name = $"Worker {count}" }, cancellationToken: stoppingToken);
                
                _greetRepository.SaveGreeting(reply.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}  
#endregion