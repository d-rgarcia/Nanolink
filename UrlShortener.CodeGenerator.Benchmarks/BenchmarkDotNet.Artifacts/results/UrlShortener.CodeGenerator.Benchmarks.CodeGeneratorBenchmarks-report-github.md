```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4894/22H2/2022Update)
Intel Core i7-4510U CPU 2.00GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.105
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method                                            | CodeLength | Mean        | Error     | StdDev    | Gen0   | Allocated |
|-------------------------------------------------- |----------- |------------:|----------:|----------:|-------:|----------:|
| **GenerateWithRandom_Array**                          | **6**          |   **124.25 ns** |  **1.517 ns** |  **1.419 ns** | **0.0842** |     **176 B** |
| GenerateWithRandomNumberGenerator_Array           | 6          |   174.07 ns |  1.224 ns |  1.145 ns | 0.0763 |     160 B |
| GenerateWithRandom_ElementAt                      | 6          |   650.28 ns |  3.049 ns |  2.852 ns | 0.1755 |     368 B |
| GenerateWithRandomNumberGenerator_ElementAt       | 6          | 1,274.83 ns | 20.219 ns | 15.786 ns | 0.1755 |     368 B |
| GenerateWithRandom_ConcatElementAt                | 6          |   707.75 ns |  4.201 ns |  3.930 ns | 0.1907 |     400 B |
| GenerateWithRandomNumberGenerator_ConcatElementAt | 6          | 1,388.88 ns |  7.809 ns |  7.305 ns | 0.1907 |     400 B |
| GenerateWithRandom_Chars                          | 6          |    44.58 ns |  0.401 ns |  0.375 ns | 0.0382 |      80 B |
| **GenerateWithRandom_Array**                          | **10**         |   **154.95 ns** |  **1.052 ns** |  **0.984 ns** | **0.0918** |     **192 B** |
| GenerateWithRandomNumberGenerator_Array           | 10         |   184.11 ns |  0.999 ns |  0.885 ns | 0.0880 |     184 B |
| GenerateWithRandom_ElementAt                      | 10         | 1,040.03 ns |  6.465 ns |  6.047 ns | 0.2441 |     512 B |
| GenerateWithRandomNumberGenerator_ElementAt       | 10         | 2,057.02 ns | 14.674 ns | 13.008 ns | 0.2441 |     512 B |
| GenerateWithRandom_ConcatElementAt                | 10         | 1,098.42 ns |  8.546 ns |  7.136 ns | 0.2594 |     544 B |
| GenerateWithRandomNumberGenerator_ConcatElementAt | 10         | 2,197.08 ns | 24.480 ns | 22.899 ns | 0.2594 |     544 B |
| GenerateWithRandom_Chars                          | 10         |    63.46 ns |  0.605 ns |  0.566 ns | 0.0459 |      96 B |
| **GenerateWithRandom_Array**                          | **20**         |   **240.98 ns** |  **4.018 ns** |  **3.758 ns** | **0.1068** |     **224 B** |
| GenerateWithRandomNumberGenerator_Array           | 20         |   229.88 ns |  2.099 ns |  1.964 ns | 0.1068 |     224 B |
| GenerateWithRandom_ElementAt                      | 20         | 2,005.71 ns | 19.606 ns | 17.380 ns | 0.4120 |     864 B |
| GenerateWithRandomNumberGenerator_ElementAt       | 20         | 4,012.15 ns | 32.739 ns | 30.625 ns | 0.4120 |     864 B |
| GenerateWithRandom_ConcatElementAt                | 20         | 2,090.37 ns | 17.685 ns | 16.542 ns | 0.4272 |     896 B |
| GenerateWithRandomNumberGenerator_ConcatElementAt | 20         | 4,242.04 ns | 27.727 ns | 25.936 ns | 0.4272 |     896 B |
| GenerateWithRandom_Chars                          | 20         |   110.48 ns |  0.956 ns |  0.799 ns | 0.0612 |     128 B |
