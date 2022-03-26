using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public unsafe struct JSStaticValue
	{
		public byte* name;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void**, void*> getProperty;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void**, bool> setProperty;
		private uint _Attributes;
		public JSPropertyAttributes Attributes { get => Unsafe.As<uint, JSPropertyAttributes>(ref _Attributes); set => _Attributes = Unsafe.As<JSPropertyAttributes, uint>(ref value); }
	}
	[BlittableType]
	public unsafe struct JSStaticValueEx
	{
		public byte* name;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void**, void*> getPropertyEx;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void*, void**, bool> setPropertyEx;
		private uint _Attributes;
		public JSPropertyAttributes Attributes { get => Unsafe.As<uint, JSPropertyAttributes>(ref _Attributes); set => _Attributes = Unsafe.As<JSPropertyAttributes, uint>(ref value); }
	}
}
