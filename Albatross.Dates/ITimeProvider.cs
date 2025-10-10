using System;

namespace Albatross.Dates {
	public interface ITimeProvider {
		DateTime UtcNow { get; }
	}
	public class SystemClock : ITimeProvider {
		public DateTime UtcNow => DateTime.UtcNow;
	}
}
