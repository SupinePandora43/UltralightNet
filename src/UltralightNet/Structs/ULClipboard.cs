using System;
using System.Runtime.InteropServices;

namespace UltralightNet;
public unsafe struct ULClipboard : IDisposable, IEquatable<ULClipboard>
{
	public ULClipboardClearCallback? Clear
	{
		set => ULPlatform.Handle(ref this, this with { __Clear = value is null ? null : (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __Clear;
			return p is null ? null : () => p();
		}
	}
	public ULClipboardReadPlainTextCallback? ReadPlainText
	{
		set => _ReadPlainText = value is null ? null : (result) =>
			{
				value(out string managedResult);

				using ULString managedResultUL = new(managedResult.AsSpan());
				result->Assign(managedResultUL);
			};
		readonly get
		{
			var c = _ReadPlainText;
			return c is null ? null : (out string managedResult) =>
			{
				using ULString resultUL = new();
				c(&resultUL);
				managedResult = (string)resultUL;
			};
		}
	}
	public ULClipboardReadPlainTextCallback__PInvoke__? _ReadPlainText
	{
		set => ULPlatform.Handle(ref this, this with { __ReadPlainText = value is null ? null : (delegate* unmanaged[Cdecl]<ULString*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __ReadPlainText;
			return p is null ? null : (result) => p(result);
		}
	}
	public ULClipboardWritePlainTextCallback? WritePlainText
	{
		set => _WritePlainText = value is null ? null : (text) => value(ULString.NativeToManaged(text));
		readonly get
		{
			var c = _WritePlainText;
			return c is null ? null : (in string text) =>
			{
				using ULString textUL = new(text.AsSpan());
				c(&textUL);
			};
		}
	}
	public ULClipboardWritePlainTextCallback__PInvoke__? _WritePlainText
	{
		set => ULPlatform.Handle(ref this, this with { __WritePlainText = value is null ? null : (delegate* unmanaged[Cdecl]<ULString*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __WritePlainText;
			return p is null ? null : (text) => p(text);
		}
	}

	public delegate* unmanaged[Cdecl]<void> __Clear;
	public delegate* unmanaged[Cdecl]<ULString*, void> __ReadPlainText;
	public delegate* unmanaged[Cdecl]<ULString*, void> __WritePlainText;

	public void Dispose()
	{
		ULPlatform.Free(this);
	}
#pragma warning disable CS8909
	public readonly bool Equals(ULClipboard clipboard) => __Clear == clipboard.__Clear && __ReadPlainText == clipboard.__ReadPlainText && __WritePlainText == clipboard.__WritePlainText;
#pragma warning restore CS8909
	public readonly override bool Equals(object? obj) => obj is ULClipboard clipboard ? Equals(clipboard) : false;

	public readonly override int GetHashCode() =>
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
		HashCode.Combine((nuint)__Clear, (nuint)__ReadPlainText, (nuint)__WritePlainText);
#else
		base.GetHashCode();
#endif
}
