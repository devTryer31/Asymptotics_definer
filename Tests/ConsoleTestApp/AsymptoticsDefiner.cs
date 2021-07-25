using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace ConsoleTestApp {



	static class AsymptoticsDefiner {
		public struct TFunc {
			public Func<double, double> Exec;
			public string Str;

			public TFunc(Func<double, double> exec, string str) {
				Exec = exec;
				Str = str;
			}
		}

		public static readonly TFunc[] Funcs = new TFunc[] {
			new TFunc( x => Log2(Log2(x)), "log2(log2(N))"),
			new TFunc(x => Sqrt(Log2(x)/2), "sqrt(log2(N)/2)"),
			new TFunc(x => Log2(x), "log2(N)"),
			new TFunc(x => Sqrt(x), "sqrt(x)"),
			new TFunc(x => Pow(Log2(x),2), "log2(N)^2"),
			new TFunc(x => x, "N"),
			new TFunc(x => x*Log2(x), "Nlog2(N)"),
			new TFunc(x => Pow(x, Log2(x)), "N^log2(N)"),
			new TFunc(x => Pow(x, Sqrt(x)), "N^sqrt(N)"),
			new TFunc(x => Pow(2,x), "(2^N)"),
			new TFunc(x => Exp(x), "exp(N)"),
			new TFunc(x => Factorial(x), "N!"),
		};

		public const int totalMaxN = 20_000_000;

		private static double Factorial(double numb) {
			double res = 1;
			for (double i = numb; i > 1; --i)
				res *= i;
			return res;
		}

		/// <summary>
		/// Calculation of the asymptotic complexity of the algorithm with the given N and O(N).
		/// </summary>
		/// <param name="NO">One ON pair of {3,5} mean N=3 and O(N)=5.</param>
		/// <param name="k_bound">The k upper bound where k=R(N)/O(N).</param>
		/// <returns>O(N)=a*R(N). Where R(N) - the founded asymptotic.</returns>
		public static (TFunc FuncItem, double a) Evaluate(Dictionary<int, int> NO, int k_bound = 2) {
			START:;

			(TFunc expr, double a) resHardCode = (new TFunc(), 0); //Future hardcode funcs (not pow funcs) result.
			double hardMink = double.MaxValue; //To find minimal k-coefficient. See the method description.
			for (int fi = 0; fi < Funcs.Length; ++fi) { //for fi-index hurdcode function.
				var R = Funcs[fi];
				double curMax_a = double.MinValue, currMax_k = double.MinValue;
				double a, k;
				//O(N)=a*R(N) => a=O(N)/R(N). k=R(N)/O(N) => The greater the a the k smaller.
				//The smaller k the result varies less from reality.
				//For searching bad results we should find max a-coefficient.
				foreach (var pr in NO) { //Searching max a-coefficient.
					double r = R.Exec(pr.Key); r = (r == 0 ? 1 : r); //I consider that there can be no case 
					a = (pr.Value == 0 ? 1 : pr.Value) / r;    //in which no operations are performed.
					curMax_a = Max(curMax_a, a);              //R(N) >= 1
				}
				foreach (var pr in NO) { //Calculating max k-coefficient.
					double r = R.Exec(pr.Key); r = (r == 0 ? 1 : r);
					k = curMax_a * r / (pr.Value == 0 ? 1 : pr.Value);
					if (k > k_bound || k < 1) //If k-coefficient is out of bounds goto next hardcode func.
						goto END;
					else
						currMax_k = Max(currMax_k, k);
				}

				if (hardMink > currMax_k) {
					hardMink = currMax_k;
					resHardCode = (expr: Funcs[fi], curMax_a); //Set hardcode funcs (not pow funcs) tmp result.
				}
				END:;
			}

			//There we found (or not (if resHardCode.a == 0)) hardcode funcs result.

			//There we do similar actions for finde pow-func result
			//where a numerator=[1..10] and denumerator=[1..10].
			double powMink = double.MaxValue; //Future  pow-funcs result.
			(TFunc expr, double a) resPows = (new TFunc(), 0);
			for (double numer = 1; numer <= 10; ++numer) {
				for (int denumer = 1; denumer <= 10; ++denumer) {
					if (numer != denumer) { //Avoid pow is equal 1.
						double curMax_a = double.MinValue, currMax_k = 0;
						double a, k;
						Func<double, double> PowFunc = x => Pow(x, numer / denumer);
						foreach (var pr in NO) {
							double p = PowFunc(pr.Key);
							a = (pr.Value == 0 ? 1 : pr.Value) / (p == 0 ? 1 : p);
							curMax_a = Max(curMax_a, a);
						}
						foreach (var pr in NO) {
							k = curMax_a * PowFunc(pr.Key) / (pr.Value == 0 ? 1 : pr.Value);
							if (k > k_bound || k < 1)
								goto END2;
							else
								currMax_k = Max(currMax_k, k);
						}
						if (currMax_k < powMink) {
							powMink = currMax_k;
							//For beautiful output.
							if (numer % denumer == 0) {
								resPows = (new TFunc(PowFunc, $"N^{numer / denumer}"), curMax_a);
							}
							else {
								resPows = (new TFunc(PowFunc, $"N^({numer}/{denumer})"), curMax_a);
							}
						}
						END2:;
					}
				}
			}

			//If we dont found any answer we axpand the bound of k=[1..k_bound_i + 1]
			//and go to the start of function using goto to avoid stacko verflow.
			if (resPows.a == 0 && resHardCode.a == 0) {
				++k_bound;
				goto START;
			}
			//We have at least one type of result.
			else if (resPows.a == 0) //If pow-func answer no found return hardcode func answer.
				return resHardCode;
			else if (resHardCode.a == 0) //If hardcode func answer no found return pow-func answer.
				return resPows;
			else
				return powMink < hardMink ? resPows : resHardCode; //Returning a less deviant function.
		}
	}
}
