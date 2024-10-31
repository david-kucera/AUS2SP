using System.Windows;

namespace GeoViewer
{
	public partial class InputWindowCount : Window
	{
		public InputWindowCount(char typ)
		{
			InitializeComponent();
		}

		private void Ok(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TextBoxNehn.Text) || string.IsNullOrEmpty(TextBoxParc.Text))
			{
				MessageBox.Show("Prosím vyplň počet!", "Prázdny počet!", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			DialogResult = true;
		}

		private void Cancel(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
