using System;
using System.Collections.Generic;

namespace ConsoleTestApp {
	class Program {
		static void Main(string[] args) {
			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");


			Console.WriteLine("Tests: ");
			//From seminar before 2nd test.
			var res = AsymptoticsDefiner.Evaluate(new Dictionary<int, int>() {
				{1, 0},
				{2, 0},
				{3, 1},
				{4, 5},
				{5, 10},
				{6, 22},
				{7, 25},
				{8, 50},
				{9, 99}
			});

			Console.WriteLine($"O1(N) = {res.a}{res.FuncItem.Str}");

			res = AsymptoticsDefiner.Evaluate(new Dictionary<int, int>() {
				{300,8 },
				{1000,9 },
				{3000,11 },
				{10000,13 },
				{30000,14 },
				{50000,15 },
			});
			Console.WriteLine($"O2(N) = {res.a}{res.FuncItem.Str}");

			res = AsymptoticsDefiner.Evaluate(new Dictionary<int, int>() {
				{1, 1},
				{2, 2},
				{3, 3},
				{4, 3},
				{5, 5},
				{6, 6},
				{7, 7},
				{8, 8},
				{9, 9}
			});
			Console.WriteLine($"O3(N) = {res.a}{res.FuncItem.Str}");

		}
	}
}
