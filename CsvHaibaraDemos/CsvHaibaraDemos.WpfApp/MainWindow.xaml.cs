using CsvHaibaraNt.Core;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CsvHaibaraDemos.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string TITLE = "CsvHaibara Demos";
        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
            Title = TITLE;
        }

		private void btnSelectFile_Click(object sender, RoutedEventArgs e)
		{
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                tbxFilePath.Text = ofd.FileName;
                btnRead.IsEnabled = true;
            }
        }

		private async void btnRead_Click(object sender, RoutedEventArgs e)
		{
            Dispatcher.Invoke(() =>
            {
                btnRead.IsEnabled = btnSelectFile.IsEnabled = false;
                btnCancel.IsEnabled = true;
                cbxHasHeader.IsEnabled = false;
                Title = "Serializing...";
                tblCount.Text = string.Empty;
            });

            int count = 0;
			Stopwatch sw = new Stopwatch();

			try
            {
                using (ICsvHaibara csvHaibara = CsvHaibaraConfiguration.GetCsvHaibara())
                {
                    string path = tbxFilePath.Text;
                    bool header = cbxHasHeader.IsChecked == true;
                    cts = new();
                    sw.Start();

                    await Task.Run(async () =>
                    {
                        await foreach (var item in csvHaibara.DeserializeAsync(path, hasHeader: header, cts.Token))
                            count++;
                    });

                    sw.Stop();
                }

                ResetUI(count, false);

				MessageBox.Show(this, $"Completed. Time taken: {Math.Round(sw.Elapsed.TotalMilliseconds, 2)} miliseconds.");
			}
            catch (OperationCanceledException)
            {
                sw.Stop();
                ResetUI(count, true);
			}
            catch (Exception ex)
            {
                sw.Stop();
                ResetUI(count, true);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}

        private void ResetUI(int count, bool cancelled)
        {
			Dispatcher.Invoke(() =>
			{
				btnRead.IsEnabled = btnSelectFile.IsEnabled = true;
                btnCancel.IsEnabled = false;
                cbxHasHeader.IsEnabled = true;
                Title = TITLE;

                if (cancelled)
					tblCount.Text = $"There are total {count} objects (Cancelled).";
                else
					tblCount.Text = $"There are total {count} objects.";
			});
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
            cts?.Cancel();
		}
	}
}