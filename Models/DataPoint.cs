using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asymptotics_definer.Models {

	internal class DataPoint<TKey, TVal> {

		public TKey Key { get; set; }
		public TVal Value { get; set; }

		public DataPoint(TKey key, TVal val) {
			Key = key;
			Value = val;
		}

	}
}
