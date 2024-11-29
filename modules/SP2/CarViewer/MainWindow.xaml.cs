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
		private readonly CarSys _carSys = new();
		private Person _currentlyDisplayedObject = null!;
		#endregion // Class members

		#region Constructor
		public MainWindow()
		{
			InitializeComponent();
		}
		#endregion // Constructor

		#region Button handlers
		private void FindById_OnClick(object sender, RoutedEventArgs e)
		{
			int searchId = int.Parse(IdCar.Text);
            var foundPerson = _carSys.Find(searchId);
			if (foundPerson != null) 
			{
                _currentlyDisplayedObject = foundPerson;
                RefreshData();
            }
            else MessageBox.Show("Osoba s ID " + searchId + " nebola nájdená!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

		private void FindByEcv_OnClick(object sender, RoutedEventArgs e)
		{
			string searchEcv = EcvCar.Text;
			var foundPerson = _carSys.Find(searchEcv);
            if (foundPerson != null)
			{
				_currentlyDisplayedObject = foundPerson;
                RefreshData();
            }
            else MessageBox.Show("Osoba s ECV " + searchEcv + " nebola nájdená!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

		private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
		{
            // TODO - vymazanie osoby z databazy
            throw new NotImplementedException();
		}

		private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{
            // TODO - editacia osoby v databaze
			// user moze upravovat v main okne data, ak stlaci tlacidlo - upravi sa s danymi hodnotami
            throw new NotImplementedException();
		}

		private void AddVisitButton_OnClick(object sender, RoutedEventArgs e)
		{
            // TODO - pridanie navstevy osoby
            // otvorit nove okno , kde sa vyplnia data o navsteve
            throw new NotImplementedException();
		}

		private void AddNoteButton_OnClick(object sender, RoutedEventArgs e)
		{
            // TODO - pridanie poznamky k navsteve
            // musi byt kliknute na navstevu v listboxe - otvori sa nove okno , kde sa vyplnia data o poznamke, tie sa pridaju do listboxu
            throw new NotImplementedException();
		}

        private void AddPerson_OnClick(object sender, RoutedEventArgs e)
		{
            // TODO - pridaj nove data o osobe/aute
            throw new NotImplementedException();
		}

		private void ClearDataDisplay_OnClick(object sender, RoutedEventArgs e)
		{
			_currentlyDisplayedObject = null!;
			RefreshData();
		}

		private void GenerateData_OnClick(object sender, RoutedEventArgs e)
		{
			InputWindow inputWindow = new();
			if (inputWindow.ShowDialog() == true)
			{
				int count = Int32.Parse(inputWindow.TextBoxCount.Text);
				_carSys.GenerujData(count);
				_currentlyDisplayedObject = null!;
				RefreshData();
				MessageBox.Show("Dáta boli úspešne vygenerované!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			// TODO skontrolovat
		}

		private void ShowHeapFileDetails_OnClick(object sender, RoutedEventArgs e)
		{
			var msg = _carSys.ZobrazHeapFileInfo();
			MessageBox.Show(msg, "Info o dátach", MessageBoxButton.OK);
		}

        private void ShowHashFileIdDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = _carSys.ZobrazHashFileIdInfo();
            MessageBox.Show(msg, "Info o hash file ID", MessageBoxButton.OK);
        }

        private void ShowHashFileEcvDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = _carSys.ZobrazHashFileEcvInfo();
            MessageBox.Show(msg, "Info o hash file ECV", MessageBoxButton.OK);
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Semestrálna práca 2 z predmetu\nALGORITMY A ÚDAJOVÉ ŠTRUKTÚRY 2\n\nAutor: Bc. David Kučera\nFRI UNIZA, 2024", "O aplikácii", MessageBoxButton.OK);
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			SaveWork_OnClick(sender, e);
            Environment.Exit(0);
		}

		private void SaveWork_OnClick(object sender, RoutedEventArgs e)
		{
			_carSys.Close();
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

		private void VisitsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            // TODO - naplnit notes textbox poznamkami k vybranej navsteve
        }
        #endregion // Event handlers

        #region Private functions
        private void RefreshData()
		{
			if (_currentlyDisplayedObject == null)
			{
                IdTextBox.Text = string.Empty;
				EcvTextBox.Text = string.Empty;
				NameTextBox.Text = string.Empty;
                SurnameTextBox.Text = string.Empty;
                VisitsListBox.Items.Clear();
                NotesTextBox.Text = string.Empty;
            }
			else
			{
                IdTextBox.Text = _currentlyDisplayedObject.Id.ToString();
				EcvTextBox.Text = _currentlyDisplayedObject.Ecv;
                NameTextBox.Text = _currentlyDisplayedObject.Name;
                SurnameTextBox.Text = _currentlyDisplayedObject.Surname;
                VisitsListBox.Items = _currentlyDisplayedObject.Visits;
            }
        }
        #endregion // Private functions
    }
}