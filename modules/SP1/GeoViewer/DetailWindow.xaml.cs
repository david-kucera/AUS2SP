using DataStructures.Data;
using System.Windows;

namespace GeoViewer
{
	public partial class DetailWindow : Window
	{
		public DetailWindow(GeoObjekt objekt)
		{
			InitializeComponent();
			if (objekt.Typ == 'N')
			{
				var objs = ((Nehnutelnost)objekt).Parcely;
				if (objs.Count == 0)
				{
					TextBox.Text = "Nehnutelnost neobsahuje ziadne parcely";
				}
				else
				{
					foreach (var obj in objs)
					{
						TextBox.Text += obj.ToString() + "\n";
					}
				}
			}
			else 
			{
				var objs = ((Parcela)objekt).Nehnutelnosti;
				if (objs.Count == 0)
				{
					TextBox.Text = "Parcela neobsahuje ziadne nehnutelnosti";
				}
				else
				{
					foreach (var obj in objs)
					{
						TextBox.Text += obj.ToString() + "\n";
					}
				}
			}
		}
	}
}
