using System.Windows;

namespace CarViewer
{
	public partial class NewVisitWindow : Window
	{
		public NewVisitWindow()
		{
			InitializeComponent();
		}

		private void Ok(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(PriceTextBox.Text))
			{
				MessageBox.Show("Vyplň dáta!", "Nevyplnené dáta!", MessageBoxButton.OK, MessageBoxImage.Error);
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
