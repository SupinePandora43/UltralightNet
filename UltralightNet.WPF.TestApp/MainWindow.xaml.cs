using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UltralightNet.AppCore;

namespace UltralightNet.WPF.TestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Renderer renderer;

		public MainWindow()
		{
			InitializeComponent();

			AppCoreMethods.ulEnableDefaultLogger("./ullog.txt");
			AppCoreMethods.ulEnablePlatformFontLoader();

			renderer = new(new ULConfig()
			{
				ResourcePath = "./resources/"
			});

			ulBrowser.Init(renderer);

			ulBrowser.view.URL = "https://github.com/SupinePandora43";

			bool loaded = false;

			ulBrowser.view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) =>
			{
				loaded = true;
			});

			while (!loaded)
			{
				renderer.Update();
				Thread.Sleep(10);
			}
		}
	}
}
