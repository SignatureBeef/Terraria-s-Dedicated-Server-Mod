using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSMExamplePlugin
{
	public class Languages
	{
		public String Enabled;

		public void LoadLanguages(TDSM_Plugin plugin)
		{
			/* SetLanguageVariable:
			 *		key		= The reference to the value in the definitions.
			 *		value	= The value to be translatable
			 */
			Enabled = plugin.SetLanguageVariable("Enabled", "enabled");
		}
	}
}
