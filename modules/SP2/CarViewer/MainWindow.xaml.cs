using System.Windows;
using System.Windows.Controls;
using CarLib;
using CarLib.Data;

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
			int searchId;
            try
			{
                searchId = int.Parse(IdCar.Text);
            }
			catch (Exception ex)
			{
                MessageBox.Show("Chyba pri hľadaní osoby: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


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
			_carSys.Remove(_currentlyDisplayedObject);
            _currentlyDisplayedObject = null!;
            RefreshData();
        }

		private void EditButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (_currentlyDisplayedObject == null!) return;
			bool keyChanged = false;
			try
			{
				keyChanged = _currentlyDisplayedObject.Id != int.Parse(IdTextBox.Text) || !_currentlyDisplayedObject.Ecv.Equals(EcvTextBox.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri editácii údajov: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			//if (keyChanged) // TODO zmenit po implementacii operacie delete v hash file
			//{
			//	MessageBox.Show("Nie je možné meniť ID alebo ECV osoby!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			//	return;
			//}

			int oldId = _currentlyDisplayedObject.Id;
			string oldEcv = _currentlyDisplayedObject.Ecv;

            if (keyChanged)
			{
				try
				{
                    _currentlyDisplayedObject.Id = int.Parse(IdTextBox.Text);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Chyba pri editácii údajov: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				try
				{
					_carSys.CheckId(_currentlyDisplayedObject.Id);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Chyba pri editácii údajov: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				_currentlyDisplayedObject.Ecv = EcvTextBox.Text;
				try
				{
					if (_currentlyDisplayedObject.Ecv.Length > 10)
                    {
                        MessageBox.Show("ECV nesmie byť dlhšie ako 10 znakov!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _carSys.CheckEcv(_currentlyDisplayedObject.Ecv);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Chyba pri editácii údajov: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}

			if (NameTextBox.Text.Length > 15)
            {
                MessageBox.Show("Meno nesmie byť dlhšie ako 15 znakov!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
			if (SurnameTextBox.Text.Length > 20)
            {
                MessageBox.Show("Priezvisko nesmie byť dlhšie ako 20 znakov!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
			if (VisitsListBox.Items.Count > 5)
            {
                MessageBox.Show("Osoba nesmie mať viac ako 5 návštev!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _currentlyDisplayedObject.Name = NameTextBox.Text;
			_currentlyDisplayedObject.Surname = SurnameTextBox.Text;
			_currentlyDisplayedObject.Visits = VisitsListBox.Items.Cast<Visit>().ToList();

			string notes = NotesTextBox.Text;
            notes = notes.Replace("\r", string.Empty);
			List<string> listNotes = new();
            if (_currentlySelectedVisit != null!)
			{
				listNotes = notes.Split('\n').ToList();
				if (listNotes.Count > 10)
                {
                    MessageBox.Show("Návšteva nesmie mať viac ako 10 poznámok!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var note in listNotes)
				{
					if (note.Length > 20)
					{
						MessageBox.Show("Poznámka nesmie byť dlhšia ako 20 znakov!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
				}
                _currentlySelectedVisit.Notes = listNotes;
            }

            try
			{
				if (keyChanged) _carSys.UpdateKeyChanged(_currentlyDisplayedObject, oldId, oldEcv);
				else _carSys.Update(_currentlyDisplayedObject);

				MessageBox.Show("Údaje boli úspešne upravené!", "Úspech", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri edite údajov: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddVisitButton_OnClick(object sender, RoutedEventArgs e)
		{
			NewVisitWindow newVisitWindow = new NewVisitWindow();
			if (newVisitWindow.ShowDialog() == true)
			{
				Visit newVisit;
                try
				{
                    newVisit = new Visit
                    {
                        Date = DateOnly.FromDateTime(newVisitWindow.DatePickerInput.SelectedDate!.Value.Date),
                        Price = double.Parse(newVisitWindow.PriceTextBox.Text),
                        Notes = newVisitWindow.NotesTextBox.Text.Split('\n').ToList()
                    };
                }
				catch (Exception ex)
                {
                    MessageBox.Show("Chyba pri pridávaní návštevy: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _currentlyDisplayedObject.Visits.Add(newVisit);
				VisitsListBox.Items.Add(newVisit);
			}
		}

		private void RemoveVisitButton_OnClick(object sender, RoutedEventArgs e)
		{
			_currentlySelectedVisit = (Visit)VisitsListBox.SelectedItem!;
			if (_currentlySelectedVisit == null!) return;
			VisitsListBox.Items.Remove(_currentlySelectedVisit);
			NotesTextBox.Text = string.Empty;
			_currentlyDisplayedObject.Visits.Remove(_currentlySelectedVisit);
		}

		private void AddPerson_OnClick(object sender, RoutedEventArgs e)
        {
	        int id = int.MinValue;
			string ecv = string.Empty;

			InputWindow inputWindowId = new InputWindow(InputWindowType.INPUT_ID);
	        if (inputWindowId.ShowDialog() == true)
	        {
				try
				{
					id = int.Parse(inputWindowId.TextBoxInput.Text);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Chyba pri pridávaní osoby: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
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
				int count;
				try
				{
					count = int.Parse(inputWindow.TextBoxInput.Text);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Chyba pri generovaní dát: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
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
			try
			{
				_carSys.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Chyba pri ukončovaní aplikácie: " + ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			Environment.Exit(0);
		}
		#endregion // Button handlers

		#region Event handlers
		private void VisitsListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_currentlySelectedVisit = (Visit)VisitsListBox.SelectedItem!;
			if (_currentlySelectedVisit == null!)
			{
				RemoveVisitButton.IsEnabled = false;
				return;
			}
			RemoveVisitButton.IsEnabled = true;
			var notes = _currentlySelectedVisit.Notes;
			if (notes.Count == 0) NotesTextBox.Text = string.Empty;
			else NotesTextBox.Text = string.Join("\n", notes);
		}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			Exit_OnClick(null!, null!);
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
				DeleteButton.IsEnabled = false;
            }
			else
			{
                IdTextBox.Text = _currentlyDisplayedObject.Id.ToString();
				EcvTextBox.Text = _currentlyDisplayedObject.Ecv;
                NameTextBox.Text = _currentlyDisplayedObject.Name;
                SurnameTextBox.Text = _currentlyDisplayedObject.Surname;
				VisitsListBox.Items.Clear();
				foreach (var visit in _currentlyDisplayedObject.Visits)
                {
	                VisitsListBox.Items.Add(visit);
                }	
				NotesTextBox.Text = string.Empty;
                EditButton.IsEnabled = true;
				AddVisitButton.IsEnabled = true;
				DeleteButton.IsEnabled = true;
            }
        }
        #endregion // Private functions
    }
}