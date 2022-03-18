// Until https://github.com/dotnet/runtimelab/issues/925
// + (side story) https://github.com/dotnet/runtimelab/issues/938


#if !GENERATED

#pragma warning disable IDE0059

namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSCheckScriptSyntax(void* context, void* script, void* sourceURL, int startingLineNumber, void** exception)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSCheckScriptSyntax__PInvoke__(context, script, sourceURL, startingLineNumber, exception);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSCheckScriptSyntax")]
        extern private static unsafe byte JSCheckScriptSyntax__PInvoke__(void* context, void* script, void* sourceURL, int startingLineNumber, void** exception);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSClassSetPrivate(void* jsClass, void* data)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSClassSetPrivate__PInvoke__(jsClass, data);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSClassSetPrivate")]
        extern private static unsafe byte JSClassSetPrivate__PInvoke__(void* jsClass, void* data);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectHasProperty(void* jsClass, void* jsObject, void* propertyName)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectHasProperty__PInvoke__(jsClass, jsObject, propertyName);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectHasProperty")]
        extern private static unsafe byte JSObjectHasProperty__PInvoke__(void* jsClass, void* jsObject, void* propertyName);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectDeleteProperty(void* context, void* jsObject, void* propertyName, void** exception)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectDeleteProperty__PInvoke__(context, jsObject, propertyName, exception);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectDeleteProperty")]
        extern private static unsafe byte JSObjectDeleteProperty__PInvoke__(void* context, void* jsObject, void* propertyName, void** exception);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectHasPropertyForKey(void* context, void* jsObject, void* propertyKey, void** exception)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectHasPropertyForKey__PInvoke__(context, jsObject, propertyKey, exception);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectHasPropertyForKey")]
        extern private static unsafe byte JSObjectHasPropertyForKey__PInvoke__(void* context, void* jsObject, void* propertyKey, void** exception);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectDeletePropertyForKey(void* context, void* jsObject, void* propertyKey, void** exception)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectDeletePropertyForKey__PInvoke__(context, jsObject, propertyKey, exception);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectDeletePropertyForKey")]
        extern private static unsafe byte JSObjectDeletePropertyForKey__PInvoke__(void* context, void* jsObject, void* propertyKey, void** exception);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectSetPrivate(void* jsObject, void* data)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectSetPrivate__PInvoke__(jsObject, data);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectSetPrivate")]
        extern private static unsafe byte JSObjectSetPrivate__PInvoke__(void* jsObject, void* data);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectIsFunction(void* context, void* jsObject)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectIsFunction__PInvoke__(context, jsObject);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectIsFunction")]
        extern private static unsafe byte JSObjectIsFunction__PInvoke__(void* context, void* jsObject);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectIsConstructor(void* context, void* jsObject)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectIsConstructor__PInvoke__(context, jsObject);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectIsConstructor")]
        extern private static unsafe byte JSObjectIsConstructor__PInvoke__(void* context, void* jsObject);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectSetPrivateProperty(void* context, void* jsObject, void* propertyName, void* value)
        {
            unsafe
            {
                bool __retVal = default;
                int __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectSetPrivateProperty__PInvoke__(context, jsObject, propertyName, value);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectSetPrivateProperty")]
        extern private static unsafe int JSObjectSetPrivateProperty__PInvoke__(void* context, void* jsObject, void* propertyName, void* value);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSObjectDeletePrivateProperty(void* context, void* jsObject, void* propertyName)
        {
            unsafe
            {
                bool __retVal = default;
                int __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSObjectDeletePrivateProperty__PInvoke__(context, jsObject, propertyName);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSObjectDeletePrivateProperty")]
        extern private static unsafe int JSObjectDeletePrivateProperty__PInvoke__(void* context, void* jsObject, void* propertyName);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSStringIsEqual(void* a, void* b)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSStringIsEqual__PInvoke__(a, b);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSStringIsEqual")]
        extern private static unsafe byte JSStringIsEqual__PInvoke__(void* a, void* b);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSStringIsEqualToUTF8CString(void* str, byte* characters)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSStringIsEqualToUTF8CString__PInvoke__(str, characters);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSStringIsEqualToUTF8CString")]
        extern private static unsafe byte JSStringIsEqualToUTF8CString__PInvoke__(void* str, byte* characters);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsUndefined(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsUndefined__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsUndefined")]
        extern private static unsafe byte JSValueIsUndefined__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsNull(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsNull__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsNull")]
        extern private static unsafe byte JSValueIsNull__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsBoolean(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsBoolean__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsBoolean")]
        extern private static unsafe byte JSValueIsBoolean__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsNumber(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsNumber__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsNumber")]
        extern private static unsafe byte JSValueIsNumber__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsString(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsString__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsString")]
        extern private static unsafe byte JSValueIsString__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsSymbol(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsSymbol__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsSymbol")]
        extern private static unsafe byte JSValueIsSymbol__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsObject(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsObject__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsObject")]
        extern private static unsafe byte JSValueIsObject__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsObjectOfClass(void* context, void* jsValue, void* jsClass)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsObjectOfClass__PInvoke__(context, jsValue, jsClass);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsObjectOfClass")]
        extern private static unsafe byte JSValueIsObjectOfClass__PInvoke__(void* context, void* jsValue, void* jsClass);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsArray(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsArray__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsArray")]
        extern private static unsafe byte JSValueIsArray__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsDate(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsDate__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsDate")]
        extern private static unsafe byte JSValueIsDate__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsEqual(void* context, void* a, void* b, void** exception)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsEqual__PInvoke__(context, a, b, exception);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsEqual")]
        extern private static unsafe byte JSValueIsEqual__PInvoke__(void* context, void* a, void* b, void** exception);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsStrictEqual(void* context, void* a, void* b)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsStrictEqual__PInvoke__(context, a, b);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsStrictEqual")]
        extern private static unsafe byte JSValueIsStrictEqual__PInvoke__(void* context, void* a, void* b);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueIsInstanceOfConstructor(void* context, void* jsValue, void* constructor, void** exception)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueIsInstanceOfConstructor__PInvoke__(context, jsValue, constructor, exception);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueIsInstanceOfConstructor")]
        extern private static unsafe byte JSValueIsInstanceOfConstructor__PInvoke__(void* context, void* jsValue, void* constructor, void** exception);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void* JSValueMakeBoolean(void* context, bool boolean)
        {
            unsafe
            {
                byte __boolean_gen_native = default;
                void* __retVal = default;
                //
                // Marshal
                //
                __boolean_gen_native = (byte)(boolean ? 1 : 0);
                //
                // Invoke
                //
                __retVal = JSValueMakeBoolean__PInvoke__(context, __boolean_gen_native);
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueMakeBoolean")]
        extern private static unsafe void* JSValueMakeBoolean__PInvoke__(void* context, byte boolean);
    }
}
namespace UltralightNet
{
    unsafe partial class JavaScriptMethods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool JSValueToBoolean(void* context, void* jsValue)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = JSValueToBoolean__PInvoke__(context, jsValue);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("WebCore", EntryPoint = "JSValueToBoolean")]
        extern private static unsafe byte JSValueToBoolean__PInvoke__(void* context, void* jsValue);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial string ulVersionString()
        {
            unsafe
            {
                string __retVal = default;
                byte* __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.UTF8Marshaller __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulVersionString__PInvoke__();
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulVersionString")]
        extern private static unsafe byte* ulVersionString__PInvoke__();
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial global::System.IntPtr ulCreateRenderer(in global::UltralightNet.ULConfig config)
        {
            unsafe
            {
                global::UltralightNet._ULConfig __config_gen_native = default;
                global::System.IntPtr __retVal = default;
                try
                {
                    //
                    // Marshal
                    //
                    __config_gen_native = new global::UltralightNet._ULConfig(config);
                    //
                    // Invoke
                    //
                    __retVal = ulCreateRenderer__PInvoke__(&__config_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __config_gen_native.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateRenderer")]
        extern private static unsafe global::System.IntPtr ulCreateRenderer__PInvoke__(global::UltralightNet._ULConfig* config);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial global::System.IntPtr ulCreateSession(global::System.IntPtr renderer, bool is_persistent, string name)
        {
            unsafe
            {
                byte __is_persistent_gen_native = default;
                global::UltralightNet.ULString* __name_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __is_persistent_gen_native = (byte)(is_persistent ? 1 : 0);
                    __name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(name);
                    __name_gen_native = __name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    __retVal = ulCreateSession__PInvoke__(renderer, __is_persistent_gen_native, __name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __name_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateSession")]
        extern private static unsafe global::System.IntPtr ulCreateSession__PInvoke__(global::System.IntPtr renderer, byte is_persistent, global::UltralightNet.ULString* name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulSessionIsPersistent(global::System.IntPtr session)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulSessionIsPersistent__PInvoke__(session);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulSessionIsPersistent")]
        extern private static unsafe byte ulSessionIsPersistent__PInvoke__(global::System.IntPtr session);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial string ulSessionGetName(global::System.IntPtr session)
        {
            unsafe
            {
                string __retVal = default;
                global::UltralightNet.ULString* __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulSessionGetName__PInvoke__(session);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulSessionGetName")]
        extern private static unsafe global::UltralightNet.ULString* ulSessionGetName__PInvoke__(global::System.IntPtr session);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial string ulSessionGetDiskPath(global::System.IntPtr session)
        {
            unsafe
            {
                string __retVal = default;
                global::UltralightNet.ULString* __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulSessionGetDiskPath__PInvoke__(session);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulSessionGetDiskPath")]
        extern private static unsafe global::UltralightNet.ULString* ulSessionGetDiskPath__PInvoke__(global::System.IntPtr session);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static unsafe partial global::System.IntPtr ulCreateBitmapFromPixels(uint width, uint height, global::UltralightNet.ULBitmapFormat format, uint row_bytes, void* pixels, nuint size, bool should_copy)
        {
            unsafe
            {
                byte __should_copy_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Marshal
                //
                __should_copy_gen_native = (byte)(should_copy ? 1 : 0);
                //
                // Invoke
                //
                __retVal = ulCreateBitmapFromPixels__PInvoke__(width, height, format, row_bytes, pixels, size, __should_copy_gen_native);
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateBitmapFromPixels")]
        extern private static unsafe global::System.IntPtr ulCreateBitmapFromPixels__PInvoke__(uint width, uint height, global::UltralightNet.ULBitmapFormat format, uint row_bytes, void* pixels, nuint size, byte should_copy);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulBitmapOwnsPixels(global::System.IntPtr bitmap)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulBitmapOwnsPixels__PInvoke__(bitmap);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulBitmapOwnsPixels")]
        extern private static unsafe byte ulBitmapOwnsPixels__PInvoke__(global::System.IntPtr bitmap);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulBitmapIsEmpty(global::System.IntPtr bitmap)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulBitmapIsEmpty__PInvoke__(bitmap);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulBitmapIsEmpty")]
        extern private static unsafe byte ulBitmapIsEmpty__PInvoke__(global::System.IntPtr bitmap);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulBitmapWritePNG(global::System.IntPtr bitmap, string path)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.UTF8Marshaller __path_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    byte* path__stackptr = stackalloc byte[global::UltralightNet.UTF8Marshaller.StackBufferSize];
                    __path_gen_native__marshaler = new global::UltralightNet.UTF8Marshaller(path, new System.Span<byte>(path__stackptr, global::UltralightNet.UTF8Marshaller.StackBufferSize));
                    //
                    // Invoke
                    //
                    fixed (byte* __path_gen_native = &__path_gen_native__marshaler.GetPinnableReference())
                        __retVal_gen_native = ulBitmapWritePNG__PInvoke__(bitmap, __path_gen_native);
                    //
                    // Unmarshal
                    //
                    __retVal = __retVal_gen_native != 0;
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __path_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulBitmapWritePNG")]
        extern private static unsafe byte ulBitmapWritePNG__PInvoke__(global::System.IntPtr bitmap, byte* path);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulConfigSetCachePath(global::System.IntPtr config, string cache_path)
        {
            unsafe
            {
                global::UltralightNet.ULString* __cache_path_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __cache_path_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __cache_path_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(cache_path);
                    __cache_path_gen_native = __cache_path_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulConfigSetCachePath__PInvoke__(config, __cache_path_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __cache_path_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetCachePath")]
        extern private static unsafe void ulConfigSetCachePath__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* cache_path);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulConfigSetUserStylesheet(global::System.IntPtr config, string font_name)
        {
            unsafe
            {
                global::UltralightNet.ULString* __font_name_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
                    __font_name_gen_native = __font_name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulConfigSetUserStylesheet__PInvoke__(config, __font_name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __font_name_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetUserStylesheet")]
        extern private static unsafe void ulConfigSetUserStylesheet__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* font_name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulConfigSetForceRepaint(global::System.IntPtr config, bool enabled)
        {
            unsafe
            {
                byte __enabled_gen_native = default;
                //
                // Marshal
                //
                __enabled_gen_native = (byte)(enabled ? 1 : 0);
                //
                // Invoke
                //
                ulConfigSetForceRepaint__PInvoke__(config, __enabled_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetForceRepaint")]
        extern private static unsafe void ulConfigSetForceRepaint__PInvoke__(global::System.IntPtr config, byte enabled);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial global::System.IntPtr ulCreateKeyEvent(global::UltralightNet.ULKeyEventType type, global::UltralightNet.ULKeyEventModifiers modifiers, int virtual_key_code, int native_key_code, string text, string unmodified_text, bool is_keypad, bool is_auto_repeat, bool is_system_key)
        {
            unsafe
            {
                global::UltralightNet.ULString* __text_gen_native = default;
                global::UltralightNet.ULString* __unmodified_text_gen_native = default;
                byte __is_keypad_gen_native = default;
                byte __is_auto_repeat_gen_native = default;
                byte __is_system_key_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __text_gen_native__marshaler = default;
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __unmodified_text_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __text_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(text);
                    __text_gen_native = __text_gen_native__marshaler.Value;
                    __unmodified_text_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(unmodified_text);
                    __unmodified_text_gen_native = __unmodified_text_gen_native__marshaler.Value;
                    __is_keypad_gen_native = (byte)(is_keypad ? 1 : 0);
                    __is_auto_repeat_gen_native = (byte)(is_auto_repeat ? 1 : 0);
                    __is_system_key_gen_native = (byte)(is_system_key ? 1 : 0);
                    //
                    // Invoke
                    //
                    __retVal = ulCreateKeyEvent__PInvoke__(type, modifiers, virtual_key_code, native_key_code, __text_gen_native, __unmodified_text_gen_native, __is_keypad_gen_native, __is_auto_repeat_gen_native, __is_system_key_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __text_gen_native__marshaler.FreeNative();
                    __unmodified_text_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateKeyEvent")]
        extern private static unsafe global::System.IntPtr ulCreateKeyEvent__PInvoke__(global::UltralightNet.ULKeyEventType type, global::UltralightNet.ULKeyEventModifiers modifiers, int virtual_key_code, int native_key_code, global::UltralightNet.ULString* text, global::UltralightNet.ULString* unmodified_text, byte is_keypad, byte is_auto_repeat, byte is_system_key);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public unsafe static partial global::UltralightNet.ULString* ulCreateString(string str)
        {
            unsafe
            {
                global::UltralightNet.ULString* __retVal = default;
                //
                // Setup
                //
                global::UltralightNet.UTF8Marshaller __str_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    byte* str__stackptr = stackalloc byte[global::UltralightNet.UTF8Marshaller.StackBufferSize];
                    __str_gen_native__marshaler = new global::UltralightNet.UTF8Marshaller(str, new System.Span<byte>(str__stackptr, global::UltralightNet.UTF8Marshaller.StackBufferSize));
                    //
                    // Invoke
                    //
                    fixed (byte* __str_gen_native = &__str_gen_native__marshaler.GetPinnableReference())
                        __retVal = ulCreateString__PInvoke__(__str_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __str_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateString")]
        extern private static unsafe global::UltralightNet.ULString* ulCreateString__PInvoke__(byte* str);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static unsafe partial global::UltralightNet.ULString* ulCreateStringUTF16(string str, nuint len)
        {
            unsafe
            {
                ushort* __str_gen_native = default;
                global::UltralightNet.ULString* __retVal = default;
                //
                // Invoke
                //
                fixed (char* __str_gen_native__pinned = str)
                    __retVal = ulCreateStringUTF16__PInvoke__((ushort*)__str_gen_native__pinned, len);
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateStringUTF16")]
        extern private static unsafe global::UltralightNet.ULString* ulCreateStringUTF16__PInvoke__(ushort* str, nuint len);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static unsafe partial bool ulStringIsEmpty(global::UltralightNet.ULString* str)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulStringIsEmpty__PInvoke__(str);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulStringIsEmpty")]
        extern private static unsafe byte ulStringIsEmpty__PInvoke__(global::UltralightNet.ULString* str);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetIsAccelerated(global::System.IntPtr config, bool is_accelerated)
        {
            unsafe
            {
                byte __is_accelerated_gen_native = default;
                //
                // Marshal
                //
                __is_accelerated_gen_native = (byte)(is_accelerated ? 1 : 0);
                //
                // Invoke
                //
                ulViewConfigSetIsAccelerated__PInvoke__(config, __is_accelerated_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetIsAccelerated")]
        extern private static unsafe void ulViewConfigSetIsAccelerated__PInvoke__(global::System.IntPtr config, byte is_accelerated);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetIsTransparent(global::System.IntPtr config, bool is_transparent)
        {
            unsafe
            {
                byte __is_transparent_gen_native = default;
                //
                // Marshal
                //
                __is_transparent_gen_native = (byte)(is_transparent ? 1 : 0);
                //
                // Invoke
                //
                ulViewConfigSetIsTransparent__PInvoke__(config, __is_transparent_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetIsTransparent")]
        extern private static unsafe void ulViewConfigSetIsTransparent__PInvoke__(global::System.IntPtr config, byte is_transparent);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetInitialFocus(global::System.IntPtr config, bool is_focused)
        {
            unsafe
            {
                byte __is_focused_gen_native = default;
                //
                // Marshal
                //
                __is_focused_gen_native = (byte)(is_focused ? 1 : 0);
                //
                // Invoke
                //
                ulViewConfigSetInitialFocus__PInvoke__(config, __is_focused_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetInitialFocus")]
        extern private static unsafe void ulViewConfigSetInitialFocus__PInvoke__(global::System.IntPtr config, byte is_focused);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetEnableImages(global::System.IntPtr config, bool enabled)
        {
            unsafe
            {
                byte __enabled_gen_native = default;
                //
                // Marshal
                //
                __enabled_gen_native = (byte)(enabled ? 1 : 0);
                //
                // Invoke
                //
                ulViewConfigSetEnableImages__PInvoke__(config, __enabled_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetEnableImages")]
        extern private static unsafe void ulViewConfigSetEnableImages__PInvoke__(global::System.IntPtr config, byte enabled);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetEnableJavaScript(global::System.IntPtr config, bool enabled)
        {
            unsafe
            {
                byte __enabled_gen_native = default;
                //
                // Marshal
                //
                __enabled_gen_native = (byte)(enabled ? 1 : 0);
                //
                // Invoke
                //
                ulViewConfigSetEnableJavaScript__PInvoke__(config, __enabled_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetEnableJavaScript")]
        extern private static unsafe void ulViewConfigSetEnableJavaScript__PInvoke__(global::System.IntPtr config, byte enabled);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetFontFamilyStandard(global::System.IntPtr config, string font_name)
        {
            unsafe
            {
                global::UltralightNet.ULString* __font_name_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
                    __font_name_gen_native = __font_name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewConfigSetFontFamilyStandard__PInvoke__(config, __font_name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __font_name_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetFontFamilyStandard")]
        extern private static unsafe void ulViewConfigSetFontFamilyStandard__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* font_name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetFontFamilyFixed(global::System.IntPtr config, string font_name)
        {
            unsafe
            {
                global::UltralightNet.ULString* __font_name_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
                    __font_name_gen_native = __font_name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewConfigSetFontFamilyFixed__PInvoke__(config, __font_name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __font_name_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetFontFamilyFixed")]
        extern private static unsafe void ulViewConfigSetFontFamilyFixed__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* font_name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetFontFamilySerif(global::System.IntPtr config, string font_name)
        {
            unsafe
            {
                global::UltralightNet.ULString* __font_name_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
                    __font_name_gen_native = __font_name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewConfigSetFontFamilySerif__PInvoke__(config, __font_name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __font_name_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetFontFamilySerif")]
        extern private static unsafe void ulViewConfigSetFontFamilySerif__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* font_name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetFontFamilySansSerif(global::System.IntPtr config, string font_name)
        {
            unsafe
            {
                global::UltralightNet.ULString* __font_name_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
                    __font_name_gen_native = __font_name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewConfigSetFontFamilySansSerif__PInvoke__(config, __font_name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __font_name_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetFontFamilySansSerif")]
        extern private static unsafe void ulViewConfigSetFontFamilySansSerif__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* font_name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewConfigSetUserAgent(global::System.IntPtr config, string agent_string)
        {
            unsafe
            {
                global::UltralightNet.ULString* __agent_string_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __agent_string_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __agent_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(agent_string);
                    __agent_string_gen_native = __agent_string_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewConfigSetUserAgent__PInvoke__(config, __agent_string_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __agent_string_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewConfigSetUserAgent")]
        extern private static unsafe void ulViewConfigSetUserAgent__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* agent_string);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial global::System.IntPtr ulCreateView(global::System.IntPtr renderer, uint width, uint height, in global::UltralightNet.ULViewConfig viewConfig, global::System.IntPtr session)
        {
            unsafe
            {
                global::UltralightNet._ULViewConfig __viewConfig_gen_native = default;
                global::System.IntPtr __retVal = default;
                try
                {
                    //
                    // Marshal
                    //
                    __viewConfig_gen_native = new global::UltralightNet._ULViewConfig(viewConfig);
                    //
                    // Invoke
                    //
                    __retVal = ulCreateView__PInvoke__(renderer, width, height, &__viewConfig_gen_native, session);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __viewConfig_gen_native.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateView")]
        extern private static unsafe global::System.IntPtr ulCreateView__PInvoke__(global::System.IntPtr renderer, uint width, uint height, global::UltralightNet._ULViewConfig* viewConfig, global::System.IntPtr session);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial string ulViewGetURL(global::System.IntPtr view)
        {
            unsafe
            {
                string __retVal = default;
                global::UltralightNet.ULString* __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewGetURL__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Unicode, EntryPoint = "ulViewGetURL")]
        extern private static unsafe global::UltralightNet.ULString* ulViewGetURL__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial string ulViewGetTitle(global::System.IntPtr view)
        {
            unsafe
            {
                string __retVal = default;
                global::UltralightNet.ULString* __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewGetTitle__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewGetTitle")]
        extern private static unsafe global::UltralightNet.ULString* ulViewGetTitle__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulViewIsLoading(global::System.IntPtr view)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewIsLoading__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewIsLoading")]
        extern private static unsafe byte ulViewIsLoading__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial global::UltralightNet.RenderTarget ulViewGetRenderTarget(global::System.IntPtr view)
        {
            unsafe
            {
                global::UltralightNet.RenderTarget __retVal = default;
                global::UltralightNet.RenderTargetNative __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewGetRenderTarget__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewGetRenderTarget")]
        extern private static unsafe global::UltralightNet.RenderTargetNative ulViewGetRenderTarget__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewLoadHTML(global::System.IntPtr view, string html_string)
        {
            unsafe
            {
                global::UltralightNet.ULString* __html_string_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __html_string_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __html_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(html_string);
                    __html_string_gen_native = __html_string_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewLoadHTML__PInvoke__(view, __html_string_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __html_string_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewLoadHTML")]
        extern private static unsafe void ulViewLoadHTML__PInvoke__(global::System.IntPtr view, global::UltralightNet.ULString* html_string);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewLoadURL(global::System.IntPtr view, string url_string)
        {
            unsafe
            {
                global::UltralightNet.ULString* __url_string_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __url_string_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __url_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(url_string);
                    __url_string_gen_native = __url_string_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    ulViewLoadURL__PInvoke__(view, __url_string_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __url_string_gen_native__marshaler.FreeNative();
                }
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewLoadURL")]
        extern private static unsafe void ulViewLoadURL__PInvoke__(global::System.IntPtr view, global::UltralightNet.ULString* url_string);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial string ulViewEvaluateScript(global::System.IntPtr view, string js_string, out string exception)
        {
            unsafe
            {
                global::UltralightNet.ULString* __js_string_gen_native = default;
                exception = default;
                global::UltralightNet.ULString* __exception_gen_native = default;
                string __retVal = default;
                global::UltralightNet.ULString* __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __js_string_gen_native__marshaler = default;
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __exception_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __js_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(js_string);
                    __js_string_gen_native = __js_string_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    __retVal_gen_native = ulViewEvaluateScript__PInvoke__(view, __js_string_gen_native, &__exception_gen_native);
                    //
                    // Unmarshal
                    //
                    __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                    __retVal = __retVal_gen_native__marshaler.ToManaged();
                    __exception_gen_native__marshaler.Value = __exception_gen_native;
                    exception = __exception_gen_native__marshaler.ToManaged();
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __js_string_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewEvaluateScript")]
        extern private static unsafe global::UltralightNet.ULString* ulViewEvaluateScript__PInvoke__(global::System.IntPtr view, global::UltralightNet.ULString* js_string, global::UltralightNet.ULString** exception);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulViewCanGoBack(global::System.IntPtr view)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewCanGoBack__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewCanGoBack")]
        extern private static unsafe byte ulViewCanGoBack__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulViewCanGoForward(global::System.IntPtr view)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewCanGoForward__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewCanGoForward")]
        extern private static unsafe byte ulViewCanGoForward__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulViewHasFocus(global::System.IntPtr view)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewHasFocus__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewHasFocus")]
        extern private static unsafe byte ulViewHasFocus__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulViewHasInputFocus(global::System.IntPtr view)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewHasInputFocus__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewHasInputFocus")]
        extern private static unsafe byte ulViewHasInputFocus__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial void ulViewSetNeedsPaint(global::System.IntPtr view, bool needs_paint)
        {
            unsafe
            {
                byte __needs_paint_gen_native = default;
                //
                // Marshal
                //
                __needs_paint_gen_native = (byte)(needs_paint ? 1 : 0);
                //
                // Invoke
                //
                ulViewSetNeedsPaint__PInvoke__(view, __needs_paint_gen_native);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewSetNeedsPaint")]
        extern private static unsafe void ulViewSetNeedsPaint__PInvoke__(global::System.IntPtr view, byte needs_paint);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        [System.Runtime.CompilerServices.SkipLocalsInitAttribute]
        public static partial bool ulViewGetNeedsPaint(global::System.IntPtr view)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewGetNeedsPaint__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewGetNeedsPaint")]
        extern private static unsafe byte ulViewGetNeedsPaint__PInvoke__(global::System.IntPtr view);
    }
}

#pragma warning restore IDE0059

#endif
