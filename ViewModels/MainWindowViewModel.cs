using Asymptotics_definer.Infrastructure.Commands;
using Asymptotics_definer.Models;
using Asymptotics_definer.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		#region MainGraphPoints : ObservableCollection<KeyValuePair<int, double>>

		private ObservableCollection<KeyValuePair<uint, double>> _MainGraphPoints;

		public ObservableCollection<KeyValuePair<uint, double>> MainGraphPoints {
			get => _MainGraphPoints;
			set => Set(ref _MainGraphPoints, value);
		}

		#endregion

		#region MainGraphComputedPoints : ObservableCollection<KeyValuePair<int, double>>

		private ObservableCollection<KeyValuePair<uint, double>> _MainGraphComputedPoints;

		public ObservableCollection<KeyValuePair<uint, double>> MainGraphComputedPoints {
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
					MainGraphPoints = new ObservableCollection<KeyValuePair<uint, double>>(
						line.Select((x, i) => new KeyValuePair<uint, double>(uint.Parse(x), line2[i]))
						);
					return;
				}
				else {
					List<KeyValuePair<uint, double>> pointsList = new List<KeyValuePair<uint, double>>();
					while (!sr.EndOfStream) {
						pointsList.Add(new KeyValuePair<uint, double>(uint.Parse(line[0]), double.Parse(line[1])));
						line = sr.ReadLine().Split(';', ',', ' ');
					}
					pointsList.Add(new KeyValuePair<uint, double>(uint.Parse(line[0]), double.Parse(line[1])));
					MainGraphPoints = new ObservableCollection<KeyValuePair<uint, double>>(
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
			var lst = new List<KeyValuePair<uint, double>>(MainGraphPoints.Count);
			foreach (var p in MainGraphPoints) {
				lst.Add(new KeyValuePair<uint, double>(p.Key, res.a * res.FuncItem.Exec(p.Key)));
			}
			MainGraphComputedPoints = new ObservableCollection<KeyValuePair<uint, double>>(lst);
			ResultPlotTitle = "Rn(N) = "+res.a.ToString() + res.FuncItem.Str;
		}

		#endregion

		#endregion


		public MainWindowViewModel() {

			#region Commands

			OpenFileCommand = new LambdaCommand(OnOpenFileCommandExecuted, CanOpenFileCommandExecute);

			ComputeCommand = new LambdaCommand(OnComputeCommandExecuted, CanComputeCommandExecute);

			#endregion

		}
	}
}
