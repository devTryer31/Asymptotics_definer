namespace Asymptotics_definer.Models {

	internal class DataPoint<TKey, TVal1, TVal2> {

		public TKey Key { get; set; }
		public TVal1 Value1 { get; set; }
		public TVal2 Value2 { get; set; }


		public DataPoint(TKey key, TVal1 val1, TVal2 val2) {
			Key = key;
			Value1 = val1;
			Value2 = val2;
		}

		public DataPoint() {
			Key = default(TKey);
			Value1 = default(TVal1);
			Value2 = default(TVal2);
		}

	}
}
