using System;

namespace Albatross.Dates {
#if NET6_0_OR_GREATER
	public interface IDateProvider {
		DateOnly Today { get; }
	}
	public class SystemDate : IDateProvider {
		public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
	}
#endif
}
