using System.Windows;


namespace CarViewer
{
	public partial class InputWindow : Window
	{
		public InputWindow(InputWindowType type)
		{
			InitializeComponent();

			switch (type)
			{
				case InputWindowType.GENERATE:
					Title = "Generovanie dát";
					LabelInput.Content = "Zadaj počet dát na vygenerovanie: ";
					TextBoxInput.Text = "100";
					break;
				case InputWindowType.INPUT_ID:
					Title = "Pridávanie novej osoby";
					LabelInput.Content = "Zadaj ID osoby: ";
					TextBoxInput.Text = "";
					break;
				case InputWindowType.INPUT_ECV:
					Title = "Pridávanie novej osoby";
					LabelInput.Content = "Zadaj EČV vozidla: ";
					TextBoxInput.Text = "";
					break;
			}
		}

		private void Ok(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TextBoxInput.Text))
			{
				MessageBox.Show("Vyplň dáta!", "Nevyplnené dáta!", MessageBoxButton.OK, MessageBoxImage.Error);
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
