using Asymptotics_definer.Infrastructure.Commands;
using Asymptotics_definer.Models;
using Asymptotics_definer.ViewModels.Base;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Asymptotics_definer.ViewModels {

	internal sealed class MainWindowViewModel : ViewModel {

		#region ImportedFile : FileInfo

		private FileInfo _ImportedFile;

		public FileInfo ImportedFile {
			get => _ImportedFile;
			set => Set(ref _ImportedFile, value);
		}

		#endregion

		#region MainGraphPoints : ObservableCollection<DataPoint<int, double>>

		private ObservableCollection<DataPoint<uint, double>> _MainGraphPoints;

		public ObservableCollection<DataPoint<uint, double>> MainGraphPoints {
			get => _MainGraphPoints;
			set => Set(ref _MainGraphPoints, value);
		}

		#endregion

		#region MainGraphComputedPoints : ObservableCollection<DataPoint<int, double>>

		private ObservableCollection<DataPoint<uint, double>> _MainGraphComputedPoints;

		public ObservableCollection<DataPoint<uint, double>> MainGraphComputedPoints {
			get => _MainGraphComputedPoints;
			set => Set(ref _MainGraphComputedPoints, value);
		}

		#endregion

		#region ResultPlotTitle

		private string _ResultPlotTitle;

		public string ResultPlotTitle {
			get => _ResultPlotTitle;
			set => Set(ref _ResultPlotTitle, value);
		}


		#endregion

		#region Commands

		#region OpenFileCommand

		public ICommand OpenFileCommand { get; }

		private bool CanOpenFileCommandExecute(object param) => true; //TODO: When computing => false. Async method require.

		private void OnOpenFileCommandExecuted(object patam) {
			OpenFileDialog openFileDialog = new OpenFileDialog() {
				Filter = "Файлы CSV|*.csv",
				Title = "Выберите файл для импорта."
			};
			var res = openFileDialog.ShowDialog();
			if (res.HasValue && res.Value) {
				ImportedFile = new FileInfo(openFileDialog.FileName);

				using var sr = new StreamReader(ImportedFile.FullName);
				var line = sr.ReadLine().Split(';', ',', ' ');
				if (line.Length > 2) {
					var line2 = sr.ReadLine().Split(';', ',', ' ').Select(double.Parse).ToList();
					MainGraphPoints = new ObservableCollection<DataPoint<uint, double>>(
						line.Select((x, i) => new DataPoint<uint, double>(uint.Parse(x), line2[i]))
						);
					return;
				}
				else {
					List<DataPoint<uint, double>> pointsList = new List<DataPoint<uint, double>>();
					while (!sr.EndOfStream) {
						pointsList.Add(new DataPoint<uint, double>(uint.Parse(line[0]), double.Parse(line[1])));
						line = sr.ReadLine().Split(';', ',', ' ');
					}
					pointsList.Add(new DataPoint<uint, double>(uint.Parse(line[0]), double.Parse(line[1])));
					MainGraphPoints = new ObservableCollection<DataPoint<uint, double>>(
						pointsList);
				}
			}
		}

		#endregion

		#region ComputeCommand

		public ICommand ComputeCommand { get; }

		private bool CanComputeCommandExecute(object param) => MainGraphPoints != null && MainGraphPoints.Count != 0;

		private void OnComputeCommandExecuted(object param) {
			var res = AsymptoticsDefiner.Evaluate(
				new Dictionary<int, int>(
					MainGraphPoints.Select(x => new KeyValuePair<int, int>((int)x.Key, (int)x.Value )))
				);
			var lst = new List<DataPoint<uint, double>>(MainGraphPoints.Count);
			foreach (var p in MainGraphPoints) {
				lst.Add(new DataPoint<uint, double>(p.Key, res.a * res.FuncItem.Exec(p.Key)));
			}
			MainGraphComputedPoints = new ObservableCollection<DataPoint<uint, double>>(lst);
			ResultPlotTitle = "Rn(N)=" + res.a.ToString("F4") + res.FuncItem.Str;
		}

		#endregion

		#region OpenGoogleFileCommand

		public ICommand OpenGoogleFileCommand { get; }

		private bool CanOpenGoogleFileCommandExecute(object param) => true;

		private void OnOpenGoogleFileCommandExecuted(object param) {
			const string url = @"https://drive.google.com/file/d/1Y64sHZNHi26ovRIBC1eR7SWhY_bYHFnC/view?usp=sharing";
			Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
		}

		#endregion

		#endregion


		public MainWindowViewModel() {

			#region Commands

			OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommandExecute);

			ComputeCommand = new LambdaCommand(OnComputeCommandExecuted, CanComputeCommandExecute);

			OpenGoogleFileCommand = new LambdaCommand(OnOpenGoogleFileCommandExecuted, CanOpenGoogleFileCommandExecute);

			#endregion

		}
	}
}
