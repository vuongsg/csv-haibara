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

        private char GetDelimiter()
        {
            if (string.IsNullOrEmpty(tbxDelimiter.Text))
                return ',';

            return tbxDelimiter.Text[0];
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
            ResetUI(-1, false, -1, "Serializing...");
            int count = 0;
			Stopwatch sw = new Stopwatch();

			try
            {
                using (ICsvHaibara csvHaibara = CsvHaibaraConfiguration.GetCsvHaibara())
                {
                    csvHaibara.Delimiter = GetDelimiter();
                    string path = tbxFilePath.Text;
                    bool header = cbxHasHeader.IsChecked == true;
                    cts = new();
                    sw.Start();

                    using (StreamIn streamIn = new(csvHaibara, path))
                    {
                        await Task.Run(async () =>
                        {
                            await foreach (var item in csvHaibara.DeserializeAsync(streamIn, hasHeader: header, cts.Token))
                                count++;
                        });
                    }

                    sw.Stop();
                }

                double timeTaken = Math.Round(sw.Elapsed.TotalMilliseconds, 2);
				ResetUI(count, false, timeTaken);

				MessageBox.Show(this, $"Completed. Time taken: {timeTaken} miliseconds.");
			}
            catch (OperationCanceledException)
            {
                sw.Stop();
				double timeTaken = Math.Round(sw.Elapsed.TotalMilliseconds, 2);
				ResetUI(count, true, timeTaken);
			}
            catch (Exception ex)
            {
                sw.Stop();
				double timeTaken = Math.Round(sw.Elapsed.TotalMilliseconds, 2);
				ResetUI(count, true, timeTaken);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}

        private void ResetUI(int count, bool cancelled, double timeTaken, string title = TITLE)
        {
			Dispatcher.Invoke(() =>
			{
				Title = title;

				if (count == -1)
                {
                    btnRead.IsEnabled = btnSelectFile.IsEnabled = false;
                    btnCancel.IsEnabled = true;
                    cbxHasHeader.IsEnabled = false;
                    tblCount.Text = string.Empty;
                }
                else
                {
                    btnRead.IsEnabled = btnSelectFile.IsEnabled = true;
                    btnCancel.IsEnabled = false;
                    cbxHasHeader.IsEnabled = true;

                    if (cancelled)
                        tblCount.Text = $"There are total {count} objects (Cancelled). Time taken: {timeTaken} miliseconds.";
                    else
                        tblCount.Text = $"There are total {count} objects. Time taken: {timeTaken} miliseconds.";
                }
			});
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
            cts?.Cancel();
		}
	}
}