using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UltralightNet.WPF
{
	public partial class ULBrowser : UserControl
	{
		private Renderer renderer;
		public View view;

		public ULBrowser()
		{
			InitializeComponent();
		}

		public void Init(Renderer renderer)
		{
			this.renderer = renderer;

			view = new(renderer, (uint)Width, (uint)Height);
			view.Focus();
			MouseWheel += (sender, e) =>
			{
				view.FireScrollEvent(new ULScrollEvent() { deltaY = e.Delta });
			};
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			view.Resize((uint)sizeInfo.NewSize.Width, (uint)sizeInfo.NewSize.Height);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (renderer is null) return;

			renderer.Update();
			renderer.Render();

			ULBitmap bitmap = view.Surface.Bitmap;

			bitmap.WritePng("te.png");

			IntPtr pixels = bitmap.LockPixels();

			BitmapSource bitmapSource = BitmapSource.Create((int)bitmap.Width, (int)bitmap.Height, 1, 1, PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent, pixels, (int)bitmap.Size, (int)bitmap.RowBytes);

			imageWPF.Source = bitmapSource;

			bitmap.UnlockPixels();
		}

		public void Destroy()
		{
			renderer = null;
		}
	}
}
