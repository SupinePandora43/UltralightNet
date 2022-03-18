namespace UltralightNet
{
	public unsafe struct JSStaticValue
	{
		public byte* name;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void**, void*> getProperty;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void**, bool> setProperty;
		public JSPropertyAttributes attributes;
	}
	public unsafe struct JSStaticValueEx
	{
		public byte* name;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void**, void*> getPropertyEx;
		public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void*, void**, bool> setPropertyEx;
		public JSPropertyAttributes attributes;
	}
}
