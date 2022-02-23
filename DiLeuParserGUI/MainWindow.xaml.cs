using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using mdDiLeuRatioParser; 

namespace DiLeuParserGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string FilePath { get; set; }
		public string OutputFilePath { get; set; }

		public MainWindow()
		{
			InitializeComponent();
		}
		private void BrowseFiles_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.DefaultExt = ".psmtsv";
			ofd.Filter = "tsv documents (*.psmtsv; *.tsv) | *.psmtsv;*.tsv" ;
			bool? res = ofd.ShowDialog(); 

			if(res == true)
			{
				FilePath = ofd.FileName;
				fileNameTextBox.Text = FilePath; 
			}
		}

		private void OutputFilePath_Click(object sender, RoutedEventArgs e)
		{
			FolderPicker dlg = new();
			dlg.InputPath = @"C:\Users"; 
			if(dlg.ShowDialog() == true)
			{
				OutputFilePath = dlg.ResultPath;
				outputFilePathTextBox.Text = OutputFilePath; 
			}

		}

		private void ProcessFile_Click(object sender, RoutedEventArgs e)
		{
			string[] diLeuParams = { FilePath, OutputFilePath };
			progressBar.Visibility = Visibility.Visible; 
			Program.Main(diLeuParams);
			progressBar.IsIndeterminate = false;
			progressBar.Value = 100; 
		}
	}
}
