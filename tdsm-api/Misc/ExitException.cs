using System;

namespace TDSM.API.Misc
{
	public class ExitException : Exception
	{
		public ExitException() { }

		public ExitException(string Info) : base(Info) { }
	}
}