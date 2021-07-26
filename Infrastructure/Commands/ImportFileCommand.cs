using Asymptotics_definer.Infrastructure.Commands.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asymptotics_definer.Infrastructure.Commands {
	class ImportFileCommand : Command {

		public override bool CanExecute(object parameter) => true; //TODO: Return false when computing.

		public override void Execute(object parameter) {
			var currFileInfo = parameter as FileInfo;
			OpenFileDialog openFileDialog = new OpenFileDialog() {
				Filter = "Файлы CSV|*.csv",
				Title = "Выберите файл для импорта."
			};
			var res = openFileDialog.ShowDialog();
			if (res.HasValue && res.Value) {
				currFileInfo = new FileInfo(openFileDialog.FileName); //TODO: Solve rewrite linc ti FileInfo.
			}

		}
	}
}
