using System.Windows;

namespace CarViewer
{
	public partial class DetailWindow : Window
	{
		public DetailWindow(string title, string data)
		{
			InitializeComponent();
			Title = title;
			Data.Text = data;
		}
	}
}
