using System.Windows;

namespace GeoViewer
{
	#region Constants
	public enum GeoObjectType
	{
		NEHNUTELNOST,
		PARCELA
	}
	#endregion //Constants

	public partial class AddObjektWindow : Window
	{
		#region Constructor
		public AddObjektWindow(GeoObjectType typ)
		{
			InitializeComponent();
			if (typ == GeoObjectType.NEHNUTELNOST)
			{
				Title = "Pridanie nehnuteľnosti";
				CisloLabel.Content = "Súpisné číslo";
			}
			else
			{
				Title = "Pridanie parcely";
				CisloLabel.Content = "Číslo parcely";
			}
		}
		#endregion //Constructor

		#region Button handlers
		private void Cancel_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
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
		#endregion //Button handlers
	}
}
