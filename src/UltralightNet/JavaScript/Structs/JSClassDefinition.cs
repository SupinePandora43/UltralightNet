// JSObjectRef.h

namespace UltralightNet
{
	public unsafe struct JSClassDefinition
	{
		public int version;
		public JSClassAttributes attributes;
		public byte* className;
		public void* parentJsClass;
		public void* staticValues;
		public void* staticFunctions;

		public void* initialize;
		public void* finalize;
		public void* hasProperty;
		public void* getProperty;
		public void* setProperty;
		public void* deleteProperty;
		public void* getPropertyNames;
		public void* callAsFunction;
		public void* callAsConstructor;
		public void* hasInstance;
		public void* convertToType;

		public void* privateData;
	}
}
