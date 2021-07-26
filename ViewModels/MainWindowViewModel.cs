using Asymptotics_definer.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asymptotics_definer.ViewModels {

	internal sealed class MainWindowViewModel : ViewModel {

		#region ImportedFile : FileInfo

		private FileInfo _ImportedFile;

		public FileInfo ImportedFile {
			get => _ImportedFile;
			set => Set(ref _ImportedFile, value);
		}

		#endregion
	}
}
