using GeoLib;
using System.Windows;
using DataStructures.Data;

namespace GeoViewer
{
	public partial class MainWindow : Window
	{
		#region Class members
		private KatSys _katSys = new();
		#endregion //Class members

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

		private void AddNehnutelnost_OnClick(object sender, RoutedEventArgs e)
		{
			AddObjektWindow addObjektWindow = new(GeoObjectType.NEHNUTELNOST);
			if (addObjektWindow.ShowDialog() == true)
			{
				int supisneCislo = Int32.Parse(addObjektWindow.CisloInput.Text);
				string popis = addObjektWindow.PopisInput.Text;
				// prva gps
				var firstLatitudeValues = addObjektWindow.GpsFirstLatitude.Text.Split(",");
				var firstLongitudeValues = addObjektWindow.GpsFirstLongitude.Text.Split(",");
				GpsPos gpsPrva = new(firstLatitudeValues[0].First(), double.Parse(firstLatitudeValues[1]), firstLongitudeValues[0].First(), double.Parse(firstLongitudeValues[1]));

				// druha gps
				var secondLatitudeValues = addObjektWindow.GpsSecondLatitude.Text.Split(",");
				var secondLongitudeValues = addObjektWindow.GpsSecondLongitude.Text.Split(",");
				GpsPos gpsDruha = new(secondLatitudeValues[0].First(), double.Parse(secondLatitudeValues[1]), secondLongitudeValues[0].First(), double.Parse(secondLongitudeValues[1]));

				Nehnutelnost result = new(supisneCislo, popis, gpsPrva, gpsDruha);
				_katSys.PridajNehnutelnost(result);
				MessageBox.Show("Dáta nehnuteľnosti boli pridané do databázy!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void AddParcela_OnClick(object sender, RoutedEventArgs e)
		{
			AddObjektWindow addObjektWindow = new(GeoObjectType.PARCELA);
			if (addObjektWindow.ShowDialog() == true)
			{
				int cisloParcely = Int32.Parse(addObjektWindow.CisloInput.Text);
				string popis = addObjektWindow.PopisInput.Text;
				// prva gps
				var firstLatitudeValues = addObjektWindow.GpsFirstLatitude.Text.Split(",");
				var firstLongitudeValues = addObjektWindow.GpsFirstLongitude.Text.Split(",");
				GpsPos gpsPrva = new(firstLatitudeValues[0].First(), double.Parse(firstLatitudeValues[1]), firstLongitudeValues[0].First(), double.Parse(firstLongitudeValues[1]));

				// druha gps
				var secondLatitudeValues = addObjektWindow.GpsSecondLatitude.Text.Split(",");
				var secondLongitudeValues = addObjektWindow.GpsSecondLongitude.Text.Split(",");
				GpsPos gpsDruha = new(secondLatitudeValues[0].First(), double.Parse(secondLatitudeValues[1]), secondLongitudeValues[0].First(), double.Parse(secondLongitudeValues[1]));

				Parcela result = new(cisloParcely, popis, gpsPrva, gpsDruha);
				_katSys.PridajParcelu(result);
				MessageBox.Show("Dáta parcely boli pridané do databázy!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}