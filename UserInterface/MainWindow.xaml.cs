using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CsvToJson;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseSource(object sender, RoutedEventArgs e)
        {
            sourcePathText.Text = GetPath();
        }

        private void BrowseDestination(object sender, RoutedEventArgs e)
        {
            destinationPathText.Text = GetPath();
        }

        private static string GetPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                return dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK ? string.Empty : dialog.SelectedPath;
            }
        }

        private void ConvertFiles(object sender, RoutedEventArgs e)
        {
            if (sourcePathText.Text == string.Empty)
            {
                MessageBox.Show("A forrásútvonal nem lehet üres!");
                return;
            }

            if (sourcePathText.Text == string.Empty)
            {
                MessageBox.Show("A célútvonal nem lehet üres!");
                return;
            }

            var files = Directory.GetFiles(sourcePathText.Text);
            var current = 0;

            var converter = new Converter();

            try
            {
                foreach (var file in files)
                {
                    current++;

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, (Action) (() => { progressBarText.Text = $"Fájlok átalakítása: {current} / {files.Length}"; }));

                    converter.ConvertFile(file, destinationPathText.Text);
                }

                MessageBox.Show("A fájlok átalakítása sikeres volt!", "Kész!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Hiba lépett fel a fájlok átalakítása közben. {exception.Message}", "Hiba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}