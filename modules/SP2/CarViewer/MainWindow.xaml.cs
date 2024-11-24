using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CarLib;
using FilesLib.Data;

namespace CarViewer
{
	public partial class MainWindow : Window
	{
		#region Class members
		private CarSys _carSys;
		private List<Person> _currentlyDisplayedObjects = [];
		#endregion // Class members

		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
			_carSys = new CarSys();
		}

		#endregion // Constructor

		#region Button handlers
		private void FindById_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void FindByEcv_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void OpenData_OnClick(object sender, MouseButtonEventArgs e)
		{
			var selectedObject = (Person)ObjectListBox.SelectedItem;
			var person = selectedObject as Person;
			PersonWindow detailWindow = new(person);
			detailWindow.ShowDialog();
		}

		private void ShowAllData_OnClick(object sender, RoutedEventArgs e)
		{
			_currentlyDisplayedObjects.Clear();
			_currentlyDisplayedObjects.AddRange(_carSys.GetAllPeople());
			RefreshData();
		}

		private void AddPerson_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void ClearDataDisplay_OnClick(object sender, RoutedEventArgs e)
		{
			_currentlyDisplayedObjects.Clear();
			RefreshData();
		}

		private void GenerateData_OnClick(object sender, RoutedEventArgs e)
		{
			InputWindow inputWindow = new();
			if (inputWindow.ShowDialog() == true)
			{
				int count = Int32.Parse(inputWindow.TextBoxCount.Text);
				_carSys.GenerujData(count);
				_currentlyDisplayedObjects.Clear();
				foreach (var objekt in _carSys.GetAllPeople())
				{
					_currentlyDisplayedObjects.Add(objekt);
				}
				RefreshData();
				MessageBox.Show("Dáta boli úspešne vygenerované!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void ShowDataDetails_OnClick(object sender, RoutedEventArgs e)
		{
			var msg = _carSys.ZobrazTotalInfo();
			MessageBox.Show(msg, "Info o dátach", MessageBoxButton.OK);
		}

		private void About_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Semestrálna práca 2 z predmetu\nALGORITMY A ÚDAJOVÉ ŠTRUKTÚRY 2\n\nAutor: Bc. David Kučera\nFRI UNIZA, 2024", "O aplikácii", MessageBoxButton.OK);
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}

		private void OpenFile_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO - toto asi nebude treba ???
			throw new NotImplementedException();
		}

		private void SaveNewFile_OnClick(object sender, RoutedEventArgs e)
		{
			// TODO - toto asi nebude treba ???
			throw new NotImplementedException();
		}
		#endregion // Button handlers

		#region Event handlers
		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
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
		#endregion // Event handlers

		#region Private functions
		private void RefreshData()
		{
			ObjectListBox.Items.Clear();
			foreach (var obj in _currentlyDisplayedObjects)
			{
				ObjectListBox.Items.Add(obj);
			}
		}
		#endregion // Private functions
	}
}