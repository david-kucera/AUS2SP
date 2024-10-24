using System.Windows;
using DataStructures.Data;

namespace GeoViewer
{
	public partial class EditObjektWindow : Window
	{
		public EditObjektWindow(GeoObjekt objekt)
		{
			InitializeComponent();
			Title = $"Upravuješ objekt {objekt.ToString()}";
			CisloInput.Text = objekt.SupisneCislo.ToString();
			PopisInput.Text = objekt.Popis;

			GpsFirstLatitude.Text = objekt.Pozicie[0].Sirka + " " + objekt.Pozicie[0].PoziciaSirky;
			GpsFirstLongitude.Text = objekt.Pozicie[0].Dlzka + " " + objekt.Pozicie[0].PoziciaDlzky;

			GpsSecondLatitude.Text = objekt.Pozicie[1].Sirka + " " + objekt.Pozicie[1].PoziciaSirky;
			GpsSecondLongitude.Text = objekt.Pozicie[1].Dlzka + " " + objekt.Pozicie[1].PoziciaDlzky;
		}

		private void Ok_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(CisloInput.Text)
			    || string.IsNullOrWhiteSpace(PopisInput.Text)
			    || string.IsNullOrWhiteSpace(GpsFirstLatitude.Text)
			    || string.IsNullOrWhiteSpace(GpsSecondLatitude.Text)
			    || string.IsNullOrWhiteSpace(GpsFirstLongitude.Text)
			    || string.IsNullOrWhiteSpace(GpsFirstLongitude.Text)
			   )
			{
				MessageBox.Show("Prosím vyplň všetky polia!", "Prázdne polia!", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			DialogResult = true;
		}

		private void Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
