#!/usr/bin/env dotnet-script
#r "System.Net.Http"
#nullable enable

using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

static string GetScriptFolder([CallerFilePath] string? path = null) => Path.GetDirectoryName(path) ?? throw new FileLoadException("Can't get script folder.");


var versionFileLines = File.ReadLinesAsync(Path.Combine(GetScriptFolder(), "UltralightVersion"));
var versionFileLineEnumerator = versionFileLines.GetAsyncEnumerator();
var UltralightVersion = await versionFileLineEnumerator.MoveNextAsync() ? versionFileLineEnumerator.Current : throw new FormatException("Couldn't read \"UltralightVersion\"");

string DepsUrl = $"https://raw.githubusercontent.com/ultralight-ux/Ultralight/{UltralightVersion}/Deps.cmake";

var client = new HttpClient();

var depsStream = await client.GetStreamAsync(DepsUrl);
var depsStreamReader = new StreamReader(depsStream);

static string ParseRev(ReadOnlySpan<char> line) => line[(line.IndexOf('"') + 1)..line.LastIndexOf('"')].ToString();
static Exception ParseException => new FormatException("Couldn't parse \"Deps.cmake\"");

string? UltralightCore_REV, WebCore_REV, Ultralight_REV, AppCore_REV;

while (true)
{
	var read = await depsStreamReader.ReadLineAsync() ?? throw ParseException;
	if (read.StartsWith("set(ULTRALIGHTCORE_REV"))
	{
		UltralightCore_REV = ParseRev(read);
		break;
	}
}
WebCore_REV = ParseRev(await depsStreamReader.ReadLineAsync() ?? throw ParseException);
Ultralight_REV = ParseRev(await depsStreamReader.ReadLineAsync() ?? throw ParseException);
AppCore_REV = ParseRev(await depsStreamReader.ReadLineAsync() ?? throw ParseException);

static string PlatformToString(OSPlatform platform) => platform.ToString() switch
{
	"Linux" => "linux",
	"OSX" => "mac",
	"Windows" => "win",
	_ => throw new PlatformNotSupportedException($"Not supported platform: {platform}")
};
static string BuildURL(string name, string rev, OSPlatform platform) => $"https://{name}.sfo2.cdn.digitaloceanspaces.com/{name}-{rev}-{PlatformToString(platform)}-x64.7z";


var tempFolder = Directory.CreateTempSubdirectory("UltralightDownloader");

Queue<Task> taskQueue = new(12);

foreach (var (lib, rev) in new[] {
	("ultralightcore-bin", UltralightCore_REV),
	("webcore-bin", WebCore_REV),
	("ultralight-bin", Ultralight_REV),
	("appcore-bin", AppCore_REV) })
{
	foreach (OSPlatform platform in new[] { OSPlatform.Linux, OSPlatform.OSX, OSPlatform.Windows })
	{
		var httpStream = await client.GetStreamAsync(BuildURL(lib, rev, platform));

		var archiveFileName = $"{lib}-{rev}-{PlatformToString(platform)}-x64.7z";

		var fileStream = File.Create(Path.Combine(tempFolder.ToString(), archiveFileName), checked((int)httpStream.Length));
		await httpStream.CopyToAsync(fileStream);

		// TODO: unarchive

		// TODO: Copy archive/bin/* to UltralightNet.Binaries
	}
}

await Task.WhenAll(taskQueue);
