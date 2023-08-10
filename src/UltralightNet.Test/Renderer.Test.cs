using System;
using Xunit;

namespace UltralightNet.Test;

[Collection("Renderer")]
[Trait("Category", "Renderer")]
public sealed class RendererTest
{
	private Renderer Renderer { get; }
	public RendererTest(RendererFixture fixture) => Renderer = fixture.Renderer;

	[Fact]
	public void SessionTest()
	{
		using var session = Renderer.DefaultSession;
		Assert.Equal("default", session.Name);
		Assert.Equal(OperatingSystem.IsWindows() ? "default" : "/default", session.DiskPath);

		using var session1 = Renderer.CreateSession(false, "myses1");
		Assert.Equal("myses1", session1.Name);
		Assert.False(session1.IsPersistent);

		using var session2 = Renderer.CreateSession(true, "myses2");
		Assert.Equal("myses2", session2.Name);
		Assert.True(session2.IsPersistent);

		Assert.True(session.Id != session1.Id && session1.Id != session2.Id);
	}

	[Fact]
	public void InspectorTest()
	{
		Assert.True(Renderer.TryStartRemoteInspectorServer("127.0.0.1", 7676));
	}
	[Fact]
	public void MemoryTest()
	{
		Renderer.LogMemoryUsage();
		Renderer.PurgeMemory();
		Renderer.LogMemoryUsage();
	}

	[Fact]
	public void GPUDriverNotSet()
	{
		Assert.Throws<Exception>(() => Renderer.CreateView(128, 128, new() { IsAccelerated = true }));
	}
}
