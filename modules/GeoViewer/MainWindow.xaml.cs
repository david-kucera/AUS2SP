using System.Windows;

namespace GeoViewer
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenFile_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO load data from external file
			throw new NotImplementedException();
		}

		private void SaveFile_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO save data to external file - defined after open
			throw new NotImplementedException();
		}

		private void SaveNewFile_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO save data to NEW external file - defined by user from dialog window
			throw new NotImplementedException();
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void FindAllButton_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO find all geoobjects
			throw new NotImplementedException();
		}

		private void FindParcelyButton_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO find all parcely
			throw new NotImplementedException();
		}

		private void FindNehnutelnostiButton_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO find all nehnutelnosti
			throw new NotImplementedException();
		}

		private void About_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Semestrálna práca z predmetu\nALGORITMY A ÚDAJOVÉ ŠTRUKTÚRY 2\n\nAutor: Bc. David Kučera\nFRI UNIZA, 2024", "O aplikácii", MessageBoxButton.OK);
		}
	}
}