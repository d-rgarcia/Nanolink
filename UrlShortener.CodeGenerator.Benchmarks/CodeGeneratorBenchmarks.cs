using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace UrlShortener.CodeGenerator.Benchmarks
{
    [MemoryDiagnoser]
    public class CodeGeneratorBenchmarks
    {
        private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random Random = new Random();
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        [Params(6, 10, 20)] // We'll test with different code lengths
        public int CodeLength { get; set; }

        [Benchmark]
        public string GenerateWithRandom_Array()
        {
            return new string(Enumerable.Repeat(AllowedChars, CodeLength)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        [Benchmark]
        public string GenerateWithRandomNumberGenerator_Array()
        {
            var bytes = new byte[CodeLength];
            Rng.GetBytes(bytes);
            return new string(bytes.Select(b => AllowedChars[b % AllowedChars.Length]).ToArray());
        }

        [Benchmark]
        public string GenerateWithRandom_ElementAt()
        {
            return new string(Enumerable.Repeat(AllowedChars, CodeLength)
                .Select(s => s.ElementAt(Random.Next(0, s.Length))).ToArray());
        }

        [Benchmark]
        public string GenerateWithRandomNumberGenerator_ElementAt()
        {
            return new string(Enumerable.Repeat(AllowedChars, CodeLength)
                .Select(s => s.ElementAt(RandomNumberGenerator.GetInt32(0, s.Length))).ToArray());
        }

        [Benchmark]
        public string GenerateWithRandom_ConcatElementAt()
        {
            return string.Concat(Enumerable.Repeat(AllowedChars, CodeLength)
                .Select(s => s.ElementAt(Random.Next(0, s.Length))).ToArray());
        }

        [Benchmark]
        public string GenerateWithRandomNumberGenerator_ConcatElementAt()
        {
            return string.Concat(Enumerable.Repeat(AllowedChars, CodeLength)
                .Select(s => s.ElementAt(RandomNumberGenerator.GetInt32(0, s.Length))).ToArray());
        }

        [Benchmark]
        public string GenerateWithRandom_CharsLoop()
        {
            var chars = new char[CodeLength];
            for (int i = 0; i < CodeLength; i++)
            {
                chars[i] = AllowedChars[Random.Next(AllowedChars.Length)];
            }
            return new string(chars);
        }

    }
}