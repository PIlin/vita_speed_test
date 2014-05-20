using System;
using System.Diagnostics;
using System.Text;

namespace vita_speed_test {
	public enum eScope {
		TL0,
		TL1,
		TL2,
		TL3,
		TL4,
		TL5,
		TL6,
		TL7,
		TL8,
		TL9,
		TL10,
		TL11,
		//TL12,
		
		MAX
	}
	
	
	
	public struct sProfiler {
		private TimeSpan mStartTicks;
		private eScope mScope;
		
		public void Start(eScope scope) {
			mScope = scope;
			mStartTicks = cProfilerSystem.Stopwatch.Elapsed;
		}
		
		public void Stop() {
			TimeSpan time = cProfilerSystem.Stopwatch.Elapsed - mStartTicks;
			cProfilerSystem.AddTime(ref time, mScope);
		}
	}
	
	
	public class cProfilerSystem {
		private static Stopwatch mStopwatch;
		private static TimeSpan[]    mTime;
		private static readonly string[] g_ScopeNames = ScopeNamesInit();
		
		private static string[] ScopeNamesInit() {
			var arr = new string[(int)eScope.MAX];
			int i = 0;
			foreach (var e in Enum.GetValues(typeof(eScope))) {
				if (i == (int)eScope.MAX)
					break;
				arr[i] = e.ToString();
				++i;
			}
			return arr;
		}
		
		public static Stopwatch Stopwatch {
			get { return mStopwatch; }
		}
		
		static cProfilerSystem() {
			mTime = new TimeSpan[(int)eScope.MAX];
			mStopwatch = new Stopwatch();
			mStopwatch.Start();
		}
		
		public static string Dump() {
			StringBuilder sb = new StringBuilder();
			
			for (int i = 0; i < (int)eScope.MAX; ++i) {
				sb.AppendFormat("{0}: {1:00.0000} msec\n", g_ScopeNames[i], mTime[i].TotalMilliseconds);
			}
			
			return sb.ToString();
		}
		
		public static void ResetAll() {
			for (int i = 0; i < (int)eScope.MAX; ++i) {
				mTime[i] = new TimeSpan(0);
			}
		}
				
		public static void AddTime(ref TimeSpan time, eScope scope) {
			mTime[(int)scope] += time;
		}
	}
}

