using System.Windows;

namespace GeoViewer
{
	public partial class InputWindowCount : Window
	{
		public InputWindowCount(char typ)
		{
			InitializeComponent();
			if (typ == 'N')
			{
				Title = "Generuj dáta nehnuteľností";
				LabelZadaj.Content = "Zadaj počet nehnuteľností na generovanie: ";
			}
			else
			{
				Title = "Generuj dáta parciel";
				LabelZadaj.Content = "Zadaj počet parciel na generovanie: ";
			}
		}

		private void Ok(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TextBoxCountToGenerate.Text))
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
