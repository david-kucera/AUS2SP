using System.Windows;
using System.Windows.Controls;
using CarLib;
using FilesLib.Data;

namespace CarViewer
{
	public partial class MainWindow : Window
	{
		#region Class members
		private readonly CarSys _carSys = new();
		private Person _currentlyDisplayedObject = null!;
		private Visit _currentlySelectedVisit = null!;
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
			if (_currentlyDisplayedObject == null!) return;
			bool keyChanged = _currentlyDisplayedObject.Id != int.Parse(IdTextBox.Text) || !_currentlyDisplayedObject.Ecv.Equals(EcvTextBox.Text);

			if (keyChanged)
			{
				MessageBox.Show("Nie je možné meniť ID alebo ECV osoby!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			int oldId = _currentlyDisplayedObject.Id;
			string oldEcv = _currentlyDisplayedObject.Ecv;
			if (keyChanged)
			{
				_currentlyDisplayedObject.Id = int.Parse(IdTextBox.Text);
				_currentlyDisplayedObject.Ecv = EcvTextBox.Text;
			}

			_currentlyDisplayedObject.Name = NameTextBox.Text;
			_currentlyDisplayedObject.Surname = SurnameTextBox.Text;
			_currentlyDisplayedObject.Visits = VisitsListBox.Items.Cast<Visit>().ToList();

			try
			{
				if (keyChanged) _carSys.UpdateKeyChanged(_currentlyDisplayedObject, oldId, oldEcv);
				else _carSys.Update(_currentlyDisplayedObject);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri edite údajov: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddVisitButton_OnClick(object sender, RoutedEventArgs e)
		{
            // TODO - pridanie navstevy osoby
            // otvorit nove okno , kde sa vyplnia data o navsteve - datum, double cena a notes
            throw new NotImplementedException();
		}

        private void AddPerson_OnClick(object sender, RoutedEventArgs e)
        {
	        int id = int.MinValue;
			string ecv = string.Empty;

			InputWindow inputWindowId = new InputWindow(InputWindowType.INPUT_ID);
	        if (inputWindowId.ShowDialog() == true)
	        {
		        id = int.Parse(inputWindowId.TextBoxInput.Text);
	        }

			try
			{
				_carSys.CheckId(id);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri pridávaní osoby: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			InputWindow inputWindowEcv = new InputWindow(InputWindowType.INPUT_ECV);
			if (inputWindowEcv.ShowDialog() == true)
			{
				ecv = inputWindowEcv.TextBoxInput.Text;
			}

			try
			{
				_carSys.CheckEcv(ecv);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri pridávaní osoby: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			Person newPerson = new Person()
			{
				Id = id,
				Ecv = ecv
			};
			try
			{
				_carSys.Add(newPerson);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri pridávaní osoby: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			_currentlyDisplayedObject = newPerson;
			RefreshData();
		}

		private void ClearDataDisplay_OnClick(object sender, RoutedEventArgs e)
		{
			EcvCar.Text = string.Empty;
			IdCar.Text = string.Empty;
			_currentlyDisplayedObject = null!;
			RefreshData();
		}

		private void DeleteData_OnClick(object sender, RoutedEventArgs e)
		{
			_carSys.Clear();
		}

		private void GenerateData_OnClick(object sender, RoutedEventArgs e)
		{
			InputWindow inputWindow = new(InputWindowType.GENERATE);
			if (inputWindow.ShowDialog() == true)
			{
				int count = Int32.Parse(inputWindow.TextBoxInput.Text);
				try
				{
					_carSys.GenerujData(count);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Chyba pri generovaní dát: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				ClearDataDisplay_OnClick(null!,null!);
				MessageBox.Show("Dáta boli úspešne vygenerované!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void ShowHeapFileDetails_OnClick(object sender, RoutedEventArgs e)
		{
			var msg = _carSys.ZobrazHeapFileInfo();
			DetailWindow detailWindow = new DetailWindow("Info o heap file", msg);
			detailWindow.ShowDialog();
		}

        private void ShowHashFileIdDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = _carSys.ZobrazHashFileIdInfo();
			DetailWindow detailWindow = new DetailWindow("Info o hash file s ID", msg);
			detailWindow.ShowDialog();
		}

        private void ShowHashFileEcvDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = _carSys.ZobrazHashFileEcvInfo();
			DetailWindow detailWindow = new DetailWindow("Info o hash file s ECV", msg);
			detailWindow.ShowDialog();
		}

        private void About_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Semestrálna práca 2 z predmetu\nALGORITMY A ÚDAJOVÉ ŠTRUKTÚRY 2\n\nAutor: Bc. David Kučera\nFRI UNIZA, 2024", "O aplikácii", MessageBoxButton.OK);
		}

		private void Exit_OnClick(object sender, RoutedEventArgs e)
		{
			_carSys.Close();
			Environment.Exit(0);
		}

		private void ExitWithoutSaving_OnClick(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}
		#endregion // Button handlers

		#region Event handlers
		private void VisitsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_currentlySelectedVisit = (Visit)VisitsListBox.SelectedItem!;
			var notes = _currentlySelectedVisit.Notes;
			if (notes.Count == 0) NotesTextBox.Text = string.Empty;
			else NotesTextBox.Text = string.Join("\n", notes);
		}
        #endregion // Event handlers

        #region Private functions
        private void RefreshData()
		{
			if (_currentlyDisplayedObject == null!)
			{
                IdTextBox.Text = string.Empty;
				EcvTextBox.Text = string.Empty;
				NameTextBox.Text = string.Empty;
                SurnameTextBox.Text = string.Empty;
                VisitsListBox.Items.Clear();
                NotesTextBox.Text = string.Empty;
                EditButton.IsEnabled = false;
				AddVisitButton.IsEnabled = false;
			}
			else
			{
                IdTextBox.Text = _currentlyDisplayedObject.Id.ToString();
				EcvTextBox.Text = _currentlyDisplayedObject.Ecv;
                NameTextBox.Text = _currentlyDisplayedObject.Name;
                SurnameTextBox.Text = _currentlyDisplayedObject.Surname;
                foreach (var visit in _currentlyDisplayedObject.Visits)
                {
	                VisitsListBox.Items.Add(visit);
                }
                EditButton.IsEnabled = true;
				AddVisitButton.IsEnabled = true;
			}
        }
        #endregion // Private functions
	}
}