``` ini

BenchmarkDotNet=v0.13.0, OS=manjaro 
Pentium Dual-Core CPU E5500 2.80GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|                   Method | length |        Mean |     Error |    StdDev |
|------------------------- |------- |------------:|----------:|----------:|
| **GlobalAllocEncodingBytes** |      **0** |   **116.16 ns** |  **0.224 ns** |  **0.198 ns** |
| CachedAllocEncodingBytes |      0 |    70.55 ns |  0.146 ns |  0.137 ns |
|  StackAllocEncodingBytes |      0 |    75.88 ns |  0.040 ns |  0.034 ns |
|          UltralightUTF16 |      0 |    96.38 ns |  0.296 ns |  0.262 ns |
| **GlobalAllocEncodingBytes** |      **1** |   **155.80 ns** |  **0.454 ns** |  **0.425 ns** |
| CachedAllocEncodingBytes |      1 |   107.03 ns |  0.585 ns |  0.519 ns |
|  StackAllocEncodingBytes |      1 |   111.89 ns |  0.797 ns |  0.707 ns |
|          UltralightUTF16 |      1 |   127.37 ns |  0.682 ns |  0.604 ns |
| **GlobalAllocEncodingBytes** |      **8** |   **195.67 ns** |  **0.324 ns** |  **0.271 ns** |
| CachedAllocEncodingBytes |      8 |   140.48 ns |  0.260 ns |  0.217 ns |
|  StackAllocEncodingBytes |      8 |   149.26 ns |  0.432 ns |  0.338 ns |
|          UltralightUTF16 |      8 |   142.77 ns |  0.535 ns |  0.474 ns |
| **GlobalAllocEncodingBytes** |     **64** |   **396.95 ns** |  **1.869 ns** |  **1.657 ns** |
| CachedAllocEncodingBytes |     64 |   346.90 ns |  3.170 ns |  2.647 ns |
|  StackAllocEncodingBytes |     64 |   346.61 ns |  0.890 ns |  0.743 ns |
|          UltralightUTF16 |     64 |   203.27 ns |  0.876 ns |  0.684 ns |
| **GlobalAllocEncodingBytes** |   **1024** | **3,967.92 ns** | **15.534 ns** | **12.971 ns** |
| CachedAllocEncodingBytes |   1024 | 3,823.68 ns | 30.103 ns | 26.685 ns |
|          UltralightUTF16 |   1024 | 1,268.70 ns | 24.889 ns | 25.559 ns |
