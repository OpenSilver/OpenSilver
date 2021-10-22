namespace System.Xaml
{
    internal struct FlagValue
    {
		private int _set;
		private int _value;

		public bool? Get(int flag)
		{
			if ((_set & flag) != 0)
				return (_value & flag) != 0;
			return null;
		}

		public bool Set(int flag, bool value)
		{
			_set |= flag;
			if (value)
				_value |= flag;
			else
				_value &= ~flag;
			return value;
		}
	}
}