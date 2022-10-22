// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using BenchmarkTests;

BenchmarkRunner.Run<ChannelBenchmark>();

Console.ReadKey();