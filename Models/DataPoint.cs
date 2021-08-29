using System;

namespace Asymptotics_definer.Models {

	internal class DataPoint<TKey, TVal1, TVal2> : IEquatable<DataPoint<TKey, TVal1, TVal2>>
		where TKey : IEquatable<TKey>
		where TVal1 : IEquatable<TVal1>
		where TVal2 : IEquatable<TVal2>
		{

		public TKey Key { get; set; }
		public TVal1 Value1 { get; set; }
		public TVal2 Value2 { get; set; }


		public DataPoint(TKey key, TVal1 val1, TVal2 val2) {
			Key = key;
			Value1 = val1;
			Value2 = val2;
		}

		public DataPoint() {
			Key = default;
			Value1 = default;
			Value2 = default;
		}

		public bool Equals(DataPoint<TKey, TVal1, TVal2> other)
			=> other.Key.Equals(Key) && other.Value1.Equals(Value1) && other.Value2.Equals(Value2);

		public override bool Equals(object obj) {
			if(obj is DataPoint<TKey, TVal1, TVal2> other)
				return Equals(other);
			return false;
		}

		public override int GetHashCode() =>
			Key.GetHashCode() * (Value1.GetHashCode() + Value2.GetHashCode());
	}
}
