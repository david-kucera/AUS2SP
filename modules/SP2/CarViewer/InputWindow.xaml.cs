using System.Windows;


namespace CarViewer
{
	public partial class InputWindow : Window
	{
		public InputWindow()
		{
			InitializeComponent();
		}

		private void Ok(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TextBoxCount.Text))
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
