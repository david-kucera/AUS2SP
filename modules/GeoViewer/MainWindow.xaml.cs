using System.IO;
using GeoLib;
using System.Windows;
using DataStructures.Data;
using Microsoft.Win32;

namespace GeoViewer
{
	public partial class MainWindow : Window
	{
		#region Class members
		private KatSys _katSys = new();
		private List<GeoObjekt> _currentlyDisplayedObjects = [];
		#endregion //Class members

		#region Constructor
		public MainWindow()
		{
			InitializeComponent();
		}
		#endregion //Constructor

		#region Button handlers
		private void OpenFile_OnClick(object sender, RoutedEventArgs e)
		{
			string baseDirectory = Path.Combine(Environment.CurrentDirectory, "..", "..", "userdata/");
			baseDirectory = Path.GetFullPath(baseDirectory);

			if (!Directory.Exists(baseDirectory))
			{
				Directory.CreateDirectory(baseDirectory);
			}

			var fileDialog = new OpenFileDialog
			{
				InitialDirectory = baseDirectory,
				Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*"
			};
			fileDialog.ShowDialog();

			if (fileDialog.FileName.Length == 0)
			{
				return;
			}
			var filePath = fileDialog.FileName;

			if (!_katSys.ReadFile(filePath))
			{
				MessageBox.Show("Data neboli načítané!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			MessageBox.Show("Dáta úspešne načítané!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			RefreshData();
			
		}

		private void SaveNewFile_OnClick(object sender, RoutedEventArgs e)
		{
			var baseDirectory = Path.Combine(Environment.CurrentDirectory, "..", "..", "userdata");
			baseDirectory = Path.GetFullPath(baseDirectory);
			if (!Directory.Exists(baseDirectory))
			{
				Directory.CreateDirectory(baseDirectory);
			}

			var fileDialog = new SaveFileDialog()
			{
				InitialDirectory = baseDirectory,
				Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*"
			};
			fileDialog.ShowDialog();

			if (fileDialog.FileName.Length == 0)
			{
				return;
			}
			var filePath = fileDialog.FileName;
			FileInfo fileInfo = new(filePath);

			if (!_katSys.SaveFile(filePath))
			{
				MessageBox.Show("Data neboli uložené!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			MessageBox.Show($"Dáta boli úspešne uložené do súboru {fileInfo.Name}!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}

		private void FindAllButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GpsFirstLongitude.Text)
			    || string.IsNullOrEmpty(GpsFirstLatitude.Text)
			    || string.IsNullOrEmpty(GpsSecondLongitude.Text)
			    || string.IsNullOrEmpty(GpsSecondLatitude.Text))
			{
				MessageBox.Show("Je potrbné zadať oboje GPS súradnice!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
				
			var lat1 = GpsFirstLatitude.Text.Split(" ");
			var lon1 = GpsFirstLongitude.Text.Split(" ");
			GpsPos pozicia1 = new(lat1[0].First(), double.Parse(lat1[1]), lon1[0].First(), double.Parse(lon1[1]));

			var lat2 = GpsSecondLatitude.Text.Split(" ");
			var lon2 = GpsSecondLongitude.Text.Split(" ");
			GpsPos pozicia2 = new(lat2[0].First(), double.Parse(lat2[1]), lon2[0].First(), double.Parse(lon2[1]));

			var ret = _katSys.Vyhladaj(pozicia1, pozicia2);
			if (!ret.Any())
			{
				MessageBox.Show("Neboli nájdené žiadne objekty na zadaných súradniciach!", "Informácia", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			_currentlyDisplayedObjects.Clear();
			foreach (var objekt in ret)
			{
				_currentlyDisplayedObjects.Add(objekt);
			}
			RefreshData();
		}

		private void FindParcelyButton_OnClick(object sender, RoutedEventArgs e)
		{
			var lat = GpsFirstLatitude.Text.Split(" ");
			var lon = GpsFirstLongitude.Text.Split(" ");
			GpsPos pozicia = new(lat[0].First(), double.Parse(lat[1]), lon[0].First(), double.Parse(lon[1]));

			var ret = _katSys.VyhladajParcely(pozicia);
			if (!ret.Any())
			{
				MessageBox.Show("Neboli nájdené žiadne parcely na zadanej súradnici!", "Informácia", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			_currentlyDisplayedObjects.Clear();
			foreach (var parcela in ret)
			{
				_currentlyDisplayedObjects.Add(parcela);
			}
			RefreshData();
		}

		private void FindNehnutelnostiButton_OnClick(object sender, RoutedEventArgs e)
		{
			var lat = GpsFirstLatitude.Text.Split(" ");
			var lon = GpsFirstLongitude.Text.Split(" ");
			GpsPos pozicia = new(lat[0].First(), double.Parse(lat[1]), lon[0].First(), double.Parse(lon[1]));

			var ret = _katSys.VyhladajNehnutelnosti(pozicia);
			if (!ret.Any())
			{
				MessageBox.Show("Neboli nájdené žiadne nehnuteľnosti na zadanej súradnici!", "Informácia", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}
			_currentlyDisplayedObjects.Clear();
			foreach (var nehnutelnost in ret)
			{
				_currentlyDisplayedObjects.Add(nehnutelnost);
			}
			RefreshData();
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
				var firstLatitudeValues = addObjektWindow.GpsFirstLatitude.Text.Split(" ");
				var firstLongitudeValues = addObjektWindow.GpsFirstLongitude.Text.Split(" ");
				GpsPos gpsPrva = new(firstLatitudeValues[0].First(), double.Parse(firstLatitudeValues[1]), firstLongitudeValues[0].First(), double.Parse(firstLongitudeValues[1]));

				// druha gps
				var secondLatitudeValues = addObjektWindow.GpsSecondLatitude.Text.Split(" ");
				var secondLongitudeValues = addObjektWindow.GpsSecondLongitude.Text.Split(" ");
				GpsPos gpsDruha = new(secondLatitudeValues[0].First(), double.Parse(secondLatitudeValues[1]), secondLongitudeValues[0].First(), double.Parse(secondLongitudeValues[1]));

				Nehnutelnost result = new(supisneCislo, popis, gpsPrva, gpsDruha);
				_katSys.PridajNehnutelnost(result);
				_currentlyDisplayedObjects.Add(result);
				RefreshData();
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
				var firstLatitudeValues = addObjektWindow.GpsFirstLatitude.Text.Split(" ");
				var firstLongitudeValues = addObjektWindow.GpsFirstLongitude.Text.Split(" ");
				GpsPos gpsPrva = new(firstLatitudeValues[0].First(), double.Parse(firstLatitudeValues[1]), firstLongitudeValues[0].First(), double.Parse(firstLongitudeValues[1]));

				// druha gps
				var secondLatitudeValues = addObjektWindow.GpsSecondLatitude.Text.Split(" ");
				var secondLongitudeValues = addObjektWindow.GpsSecondLongitude.Text.Split(" ");
				GpsPos gpsDruha = new(secondLatitudeValues[0].First(), double.Parse(secondLatitudeValues[1]), secondLongitudeValues[0].First(), double.Parse(secondLongitudeValues[1]));

				Parcela result = new(cisloParcely, popis, gpsPrva, gpsDruha);
				_katSys.PridajParcelu(result);
				_currentlyDisplayedObjects.Add(result);
				RefreshData();
				MessageBox.Show("Dáta parcely boli pridané do databázy!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (ObjectListBox.SelectedItem == null)
			{
				MessageBox.Show("Nie je vybraný žiadny objekt na editáciu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var selectedObject = (GeoObjekt)ObjectListBox.SelectedItem;
			if (selectedObject.Typ == 'N')
			{
				_katSys.VymazNehnutelnost((Nehnutelnost)selectedObject);
			}
			else
			{
				_katSys.VymazParcelu((Parcela)selectedObject);
			}
			_currentlyDisplayedObjects.Remove(selectedObject);
			RefreshData();
			MessageBox.Show("Objekt bol úspešne vymazaný!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (ObjectListBox.SelectedItem == null)
			{
				MessageBox.Show("Nie je vybraný žiadny objekt na editáciu!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			var selectedObject = (GeoObjekt)ObjectListBox.SelectedItem;
			EditObjektWindow editObjektWindow = new(selectedObject);
			if (editObjektWindow.ShowDialog() == true)
			{
				var newCislo = Int32.Parse(editObjektWindow.CisloInput.Text);
				var newPopis = editObjektWindow.PopisInput.Text;

				// prva gps
				var firstLatitudeValues = editObjektWindow.GpsFirstLatitude.Text.Split(" ");
				var firstLongitudeValues = editObjektWindow.GpsFirstLongitude.Text.Split(" ");
				GpsPos gpsPrvaNova = new(firstLatitudeValues[0].First(), double.Parse(firstLatitudeValues[1]), firstLongitudeValues[0].First(), double.Parse(firstLongitudeValues[1]));

				// druha gps
				var secondLatitudeValues = editObjektWindow.GpsSecondLatitude.Text.Split(" ");
				var secondLongitudeValues = editObjektWindow.GpsSecondLongitude.Text.Split(" ");
				GpsPos gpsDruhaNova = new(secondLatitudeValues[0].First(), double.Parse(secondLatitudeValues[1]), secondLongitudeValues[0].First(), double.Parse(secondLongitudeValues[1]));

				if (selectedObject is Nehnutelnost nehnutelnost)
				{
					_katSys.EditNehnutelnost(nehnutelnost, newCislo, newPopis, gpsPrvaNova, gpsDruhaNova);
				}
				else
				{
					var parcela = (Parcela)selectedObject;
					_katSys.EditParcela(parcela, newCislo, newPopis, gpsPrvaNova, gpsDruhaNova);
				}

				MessageBox.Show("Objekt bol úspešne editovaný!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show("Objekt nebol editovaný!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ClearDataDisplay_OnClick(object sender, RoutedEventArgs e)
		{
			_currentlyDisplayedObjects.Clear();
			RefreshData();
		}

		private void ShowAllData_OnClick(object sender, RoutedEventArgs e)
		{
			_currentlyDisplayedObjects.Clear();
			_currentlyDisplayedObjects.AddRange(_katSys.GetAllObjects());
			RefreshData();
		}

		private void ShowDataDetails_OnClick(object sender, RoutedEventArgs e)
		{
			var msg = _katSys.ZobrazTotalInfo();
			MessageBox.Show(msg, "Info o dátach", MessageBoxButton.OK);
		}

		private void GenerateData_OnClick(object sender, RoutedEventArgs e)
		{
			InputWindowCount inputWindow = new('P');
			if (inputWindow.ShowDialog() == true)
			{
				int countNehn = Int32.Parse(inputWindow.TextBoxNehn.Text);
				int countParc = Int32.Parse(inputWindow.TextBoxParc.Text);
				int perc = Int32.Parse(inputWindow.TextBoxPerc.Text);
				_katSys.GenerujData(countNehn, countParc, perc);
				MessageBox.Show("Dáta boli úspešne vygenerované!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
				RefreshData();
			}
		}
		#endregion //Button handlers

		#region Action handlers
		private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (ObjectListBox.SelectedItem != null)
			{
				DeleteButton.IsEnabled = true;
				EditButton.IsEnabled = true;
			}
			else
			{
				DeleteButton.IsEnabled = false;
				EditButton.IsEnabled = false;
			}
		}
		#endregion //Action handlers

		#region Private functions
		/// <summary>
		/// Method that refreshes current data in the application window that user sees
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		private void RefreshData()
		{
			ObjectListBox.Items.Clear();
			foreach (var obj in _currentlyDisplayedObjects)
			{
				ObjectListBox.Items.Add(obj);
			}
		}
		#endregion //Private functions
	}
}