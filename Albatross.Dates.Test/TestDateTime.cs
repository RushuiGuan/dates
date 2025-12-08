using System;
using Xunit;

namespace Albatross.Dates.Test {
	public class TestDateTime {
		[Fact]
		public void TestDateFormat() {
			var date = new DateTime(2023, 6, 1, 8, 10, 20, DateTimeKind.Local);
			string text = date.ToString("yyyy-MM-ddTHH:mm:ss.fffz");
			Assert.Equal("2023-06-01T08:10:20.000-4", text);


			string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
			text = date.ToString(format);
			Assert.Equal("2023-06-01T08:10:20.000Z", text);

			text = "2023-06-25T21:47:44.060Z";
			date = DateTime.ParseExact(text, format, null);
			Assert.True(date.Kind == DateTimeKind.Local);
		}
	}
}