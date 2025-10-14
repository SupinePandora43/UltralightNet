using Veldrid;

namespace UltralightNet.Veldrid.SDL2
{
	public static class KeyConverter
	{
		public static ULKeyEvent ToULKeyEvent(in this KeyEvent k)
		{
			ULKeyEventType type = k.Down ? ULKeyEventType.RawKeyDown : ULKeyEventType.KeyUp;
			ULKeyEventModifiers modifiers = 0;
			int keycode;
			string text = "";

			if ((k.Modifiers & ModifierKeys.Alt) is ModifierKeys.Alt) modifiers |= ULKeyEventModifiers.AltKey;
			if ((k.Modifiers & ModifierKeys.Control) is ModifierKeys.Control) modifiers |= ULKeyEventModifiers.CtrlKey;
			if ((k.Modifiers & ModifierKeys.Shift) is ModifierKeys.Shift) modifiers |= ULKeyEventModifiers.ShiftKey;
			if ((k.Modifiers & ModifierKeys.Gui) is ModifierKeys.Gui) modifiers |= ULKeyEventModifiers.MetaKey;

			Key key = k.Key;

			if (key is Key.KeypadEnter or Key.Enter)
			{
				/*type = ULKeyEventType.Char;
				if (type is ULKeyEventType.RawKeyDown)
				{
					text = '\r'.ToString();
				}*/
				keycode = 13;
			}
			else
				keycode = SDLtoUL(key);

			if (k.Down && false)
				if (key > (Key)82 && key < (Key)119)
				{
					//type = ULKeyEventType.Char;
					if (key < (Key)109)
					{
						text = ((modifiers & ULKeyEventModifiers.ShiftKey) is not 0) ? key.ToString() : key.ToString().ToLower();
					}
					else
					{
						text = ((int)key - 109).ToString();
					}
				}

			return ULKeyEvent.Create(type, modifiers, keycode, 0, text, text, false, k.Repeat, false);
		}
		public static int SDLtoUL(Key key)
		{
			#region 0 - 9
			if (key > (Key)108 && key < (Key)119)
			{
				return (int)key - 61;
			}
			#endregion

			#region A - Z
			if (key > (Key)82 && key < (Key)109)
			{
				return (int)key - 18;
			}
			#endregion

			#region CTRL - Shift
			if (key > 0 && key < (Key)5)
			{
				return (int)key + 159;
			}
			#endregion

			#region F1 - F24
			if (key > (Key)9)
			{
				if (key < (Key)34) return (int)key + 102;
				// F25 - F35 not supported
				else if (key < (Key)45) return 0;
			}
			#endregion

			#region Numpad 0 - Numpad 9
			if (key > (Key)66 && key < (Key)77)
			{
				return (int)key + 29;
			}
			#endregion

			int key_code = key switch
			{
				Key.NumLock => ULKeyCodes.GK_NUMLOCK,
				Key.KeypadDivide => ULKeyCodes.GK_DIVIDE,
				Key.KeypadMultiply => ULKeyCodes.GK_MULTIPLY,
				Key.KeypadSubtract => ULKeyCodes.GK_SUBTRACT,
				Key.KeypadAdd => ULKeyCodes.GK_ADD,
				Key.KeypadDecimal => ULKeyCodes.GK_DECIMAL,

				Key.Escape => ULKeyCodes.GK_ESCAPE,
				Key.Tilde => ULKeyCodes.GK_OEM_3,
				Key.Tab => ULKeyCodes.GK_TAB,
				Key.CapsLock => ULKeyCodes.GK_CAPITAL,
				Key.AltLeft or Key.AltRight => ULKeyCodes.GK_MENU,
				Key.Space => ULKeyCodes.GK_SPACE,

				Key.Slash => ULKeyCodes.GK_OEM_2,


				Key.PrintScreen => ULKeyCodes.GK_SNAPSHOT,
				Key.ScrollLock => ULKeyCodes.GK_SCROLL,

				Key.Insert => ULKeyCodes.GK_INSERT,
				Key.Delete => ULKeyCodes.GK_DELETE,
				Key.Home => ULKeyCodes.GK_HOME,
				Key.End => ULKeyCodes.GK_END,
				Key.PageUp => ULKeyCodes.GK_PRIOR,
				Key.PageDown => ULKeyCodes.GK_NEXT,

				Key.Left => ULKeyCodes.GK_LEFT,
				Key.Right=> ULKeyCodes.GK_RIGHT,
				Key.Up => ULKeyCodes.GK_UP,
				Key.Down => ULKeyCodes.GK_DOWN,


				_ => (int)key + 29
			};

			return key_code;
		}
	}
}
