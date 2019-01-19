namespace TvMaze.Scraper.Core
{
	public class ErrorCode
	{
		public static readonly ErrorCode NotFound = new ErrorCode(404);

		private readonly int _value;

		public ErrorCode(int value)
		{
			_value = value;
		}

		public static bool operator ==(ErrorCode left, ErrorCode right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ErrorCode left, ErrorCode right)
		{
			return !Equals(left, right);
		}

		private bool Equals(ErrorCode other)
		{
			return _value == other._value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ErrorCode) obj);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
	}
}
