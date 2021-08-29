using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Asymptotics_definer.Infrastructure.Converters {

	[ValueConversion(typeof(uint),typeof(string))]
	class EmptyStringToIntConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value.ToString();

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> string.IsNullOrWhiteSpace((string)value) ? 0u : uint.Parse((string)value);
	}
}
