using System;

namespace tdsm.api
{
	public class ExitException : Exception
	{
		public ExitException() { }

		public ExitException(string Info) : base(Info) { }
	}
}