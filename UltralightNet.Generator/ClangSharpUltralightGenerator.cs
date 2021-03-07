using ClangSharp;
using ClangSharp.Interop;
using System;
using System.IO;
using System.Reflection;

namespace UltralightNet.Generator
{
	internal static class ClangSharpUltralightGenerator
	{
		internal static string[] files = new[] { "Ultralight/CAPI.h" };
		internal static void Generate()
		{
			string path = Path.Combine(Path.GetDirectoryName(typeof(ClangSharpUltralightGenerator).Assembly.Location), "Ultralight-API-master");

			PInvokeGeneratorConfiguration configuration = new(
				libraryPath: "Ultralight",
				namespaceName: "UltralightNet",
				outputLocation: Path.GetDirectoryName(typeof(ClangSharpUltralightGenerator).Assembly.Location) + "/idk.cs",
				testOutputLocation: string.Empty
			);

			var clangCommandLineArgs = new string[]
			{
				$"--language=c++",               // Treat subsequent input files as having type <language>
                $"--std=c++17",                         // Language standard to compile for
                "-Wno-pragma-once-outside-header",       // We are processing files which may be header files
				$"--include-directory={path}"
			};
			var translationFlags = CXTranslationUnit_Flags.CXTranslationUnit_None;

			translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes;               // Include attributed types in CXType
			translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;              // Implicit attributes should be visited

			PInvokeGenerator generator = new(configuration);

			foreach (var file in files)
			{
				var filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory() + "/Ultralight-API-master", file));

				var translationUnitError = CXTranslationUnit.TryParse(generator.IndexHandle, filePath, clangCommandLineArgs, Array.Empty<CXUnsavedFile>(), translationFlags, out CXTranslationUnit handle);
				var skipProcessing = false;

				if (translationUnitError != CXErrorCode.CXError_Success)
				{
					Console.WriteLine($"Error: Parsing failed for '{filePath}' due to '{translationUnitError}'.");
					skipProcessing = true;
				}
				else if (handle.NumDiagnostics != 0)
				{
					Console.WriteLine($"Diagnostics for '{filePath}':");

					for (uint i = 0; i < handle.NumDiagnostics; ++i)
					{
						using var diagnostic = handle.GetDiagnostic(i);

						Console.Write("    ");
						Console.WriteLine(diagnostic.Format(CXDiagnostic.DefaultDisplayOptions).ToString());

						skipProcessing |= (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Error);
						skipProcessing |= (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Fatal);
					}
				}

				if (skipProcessing)
				{
					Console.WriteLine($"Skipping '{filePath}' due to one or more errors listed above.");
					Console.WriteLine();

					continue;
				}

				try
				{
					using var translationUnit = TranslationUnit.GetOrCreate(handle);
					Console.WriteLine($"Processing '{filePath}'");

					generator.GenerateBindings(translationUnit, filePath, clangCommandLineArgs, translationFlags);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
			if (generator.Diagnostics.Count != 0)
			{
				Console.WriteLine("Diagnostics for binding generation:");

				foreach (var diagnostic in generator.Diagnostics)
				{
					Console.Write("    ");
					Console.WriteLine(diagnostic);
				}
			}
		}
	}
}
