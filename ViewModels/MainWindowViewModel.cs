using Asymptotics_definer.Infrastructure.Commands;
using Asymptotics_definer.Models;
using Asymptotics_definer.ViewModels.Base;
using Microsoft.Win32;
using System;
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

		#region GraphPoints : ObservableCollection<DataPoint<uint, double, double>>

		private ObservableCollection<DataPoint<uint, double, double>> _GraphPoints =
			new ObservableCollection<DataPoint<uint, double, double>>() {
				new DataPoint<uint, double, double>(1000,2,3),
				new DataPoint<uint, double, double>(315,4,542),
				new DataPoint<uint, double, double>(2,30,4000)
			};

		public ObservableCollection<DataPoint<uint, double, double>> GraphPoints {
			get => _GraphPoints;
			set => Set(ref _GraphPoints, value);
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
					GraphPoints = new ObservableCollection<DataPoint<uint, double, double>>(
						line.Select((x, i) => new DataPoint<uint, double, double>(uint.Parse(x), line2[i], default(double)))
						);
				}
				else {
					var pointsList = new List<DataPoint<uint, double, double>>();
					while (!sr.EndOfStream) {
						pointsList.Add(new DataPoint<uint, double, double>(uint.Parse(line[0]), double.Parse(line[1]), default(double)));
						line = sr.ReadLine().Split(';', ',', ' ');
					}
					pointsList.Add(new DataPoint<uint, double, double>(uint.Parse(line[0]), double.Parse(line[1]), default(double)));
					GraphPoints = new ObservableCollection<DataPoint<uint, double, double>>(
						pointsList);
				}
			}
		}

		#endregion

		#region ComputeCommand

		public ICommand ComputeCommand { get; }

		private bool CanComputeCommandExecute(object param) => GraphPoints != null && GraphPoints.Count != 0;

		private void OnComputeCommandExecuted(object param) {
			var res = AsymptoticsDefiner.Evaluate(
				new Dictionary<int, int>(
					GraphPoints.Select(x => new KeyValuePair<int, int>((int)x.Key, (int)x.Value1)))
				);
			foreach (var p in GraphPoints)
				p.Value2 = Math.Round(res.a * res.FuncItem.Exec(p.Key));
			// ERROR: It does't work. Why?? - + +. Bad practice.
			// - OnPropertyChanged(nameof(GraphPoints)); 
			// + GraphPoints = new ObservableCollection<DataPoint<uint, double, double>>( GraphPoints);
			// + //
			var tmp = GraphPoints;
			GraphPoints = null;
			GraphPoints = tmp;

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
