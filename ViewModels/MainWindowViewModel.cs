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

		private DataPoint<uint, uint, uint> _toAddDataPoint = new();

		public DataPoint<uint, uint, uint> ToAddDataPoint {
			get => _toAddDataPoint;
			set => Set(ref _toAddDataPoint, value);
		}


		#region GraphPoints : ObservableCollection<DataPoint<uint, uint, uint>>

		private ObservableCollection<DataPoint<uint, uint, uint>> _GraphPoints =
			new() {
				//new DataPoint<uint, uint, uint>(2,30,4000),
				//new DataPoint<uint, uint, uint>(315,4,542),
				//new DataPoint<uint, uint, uint>(1000,2,3)
			};

		public ObservableCollection<DataPoint<uint, uint, uint>> GraphPoints {
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
			OpenFileDialog openFileDialog = new() {
				Filter = "Файлы CSV|*.csv",
				Title = "Выберите файл для импорта."
			};
			var res = openFileDialog.ShowDialog();
			if (res.HasValue && res.Value) {
				ImportedFile = new FileInfo(openFileDialog.FileName);

				using var sr = new StreamReader(ImportedFile.FullName);
				var line = sr.ReadLine().Split(';', ',', ' ');
				if (line.Length > 2) {
					var line2 = sr.ReadLine().Split(';', ',', ' ').Select(uint.Parse).ToList();
					GraphPoints = new ObservableCollection<DataPoint<uint, uint, uint>>(
						line.Select((x, i) => new DataPoint<uint, uint, uint>(uint.Parse(x), line2[i], default(uint)))
						);
				}
				else {
					var pointsList = new List<DataPoint<uint, uint, uint>>();
					while (!sr.EndOfStream) {
						pointsList.Add(new DataPoint<uint, uint, uint>(uint.Parse(line[0]), uint.Parse(line[1]), default(uint)));
						line = sr.ReadLine().Split(';', ',', ' ');
					}
					pointsList.Add(new DataPoint<uint, uint, uint>(uint.Parse(line[0]), uint.Parse(line[1]), default(uint)));
					GraphPoints = new ObservableCollection<DataPoint<uint, uint, uint>>(
						pointsList);
				}
			}
		}

		#endregion

		#region ComputeCommand

		public ICommand ComputeCommand { get; }

		private bool CanComputeCommandExecute(object param) => GraphPoints != null && GraphPoints.Count != 0;

		private void OnComputeCommandExecuted(object param = null) {
			var (FuncItem, a) = AsymptoticsDefiner.Evaluate(
				new Dictionary<int, int>(
					GraphPoints.Select(x => new KeyValuePair<int, int>((int)x.Key, (int)x.Value1)))
				);
			foreach (var p in GraphPoints)
				p.Value2 = (uint)Math.Round(a * FuncItem.Exec(p.Key));
			// ERROR: It does't work. Why? - + +. Bad practice.
			// - OnPropertyChanged(nameof(GraphPoints)); 
			// + GraphPoints = new ObservableCollection<DataPoint<uint, uint, uint>>( GraphPoints);
			// + //
			var tmp = GraphPoints;
			GraphPoints = null;
			GraphPoints = tmp;

			ResultPlotTitle = "Rn(N)=" + a.ToString("F4") + FuncItem.Str;
		}

		#endregion

		#region OpenGoogleFileCommand

		public ICommand OpenGoogleFileCommand { get; }

		private bool CanOpenGoogleFileCommandExecute(object param) => true;

		private void OnOpenGoogleFileCommandExecuted(object param) {
			const string url = @"https://docs.google.com/document/d/1Y64sHZNHi26ovRIBC1eR7SWhY_bYHFnC/edit?usp=sharing&ouid=101500186422908909807&rtpof=true&sd=true";
			Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
		}

		#endregion

		#region AddOneDataPointCommand
		//TODO: Fix this command.
		public ICommand AddOneDataPointCommand { get; }

		private bool CanAddOneDataPointCommandExecute(object param)
			=> ToAddDataPoint != null && !GraphPoints.Contains(ToAddDataPoint) && ToAddDataPoint.Key != 0 && ToAddDataPoint.Value1 != 0;

		private void OnAddOneDataPointCommandExecuted(object param) {
			GraphPoints.Add(ToAddDataPoint);
			GraphPoints = new ObservableCollection<DataPoint<uint, uint, uint>>(
				GraphPoints.OrderBy(p => p.Key)
				);
			ToAddDataPoint = new();
			OnComputeCommandExecuted();
		}

		#endregion

		#endregion


		public MainWindowViewModel() {

			#region Commands

			OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommandExecute);

			ComputeCommand = new LambdaCommand(OnComputeCommandExecuted, CanComputeCommandExecute);

			OpenGoogleFileCommand = new LambdaCommand(OnOpenGoogleFileCommandExecuted, CanOpenGoogleFileCommandExecute);

			AddOneDataPointCommand = new LambdaCommand(OnAddOneDataPointCommandExecuted, CanAddOneDataPointCommandExecute);

			#endregion

		}
	}
}
