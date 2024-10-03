// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using UrlShortener.CodeGenerator.Benchmarks;

var summary = BenchmarkRunner.Run<CodeGeneratorBenchmarks>();
