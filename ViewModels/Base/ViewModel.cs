using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asymptotics_definer.ViewModels.Base {
	internal abstract class ViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
		}

		protected virtual bool Set<T>(ref T source, T value, [CallerMemberName] string propertyName = null) {
			if (object.Equals(source, value))
				return true;

			source = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
