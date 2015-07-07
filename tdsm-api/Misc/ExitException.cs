using System;

namespace tdsm.api.Misc
{
	public class ExitException : Exception
	{
		public ExitException() { }

		public ExitException(string Info) : base(Info) { }
	}
}