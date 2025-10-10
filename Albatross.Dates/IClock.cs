using System;

namespace Albatross.Dates {
	public interface IClock {
		DateTime UtcNow { get; }
	}
	public class SystemClock : IClock {
		public DateTime UtcNow => DateTime.UtcNow;
	}
}
