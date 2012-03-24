using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSM_PermissionsX
{
	public class Languages
	{
		public String DisabledMessaged;
		public String Initializing;
		public String Enabled;
		public String Protected;
		public String GetAuth;
		public String NoAuth;
		public String RestrictDlFailed;
		public String RestrictLoadFail;
		public String RestrictLoadResult;

		public String Success;
		public String Failure;
		public String Failed;
		public String Saving;
		public String Loading;
		public String Added;
		public String Removed;

		public String NoPlayersAndForce;

		public String DefinitionsExist;
		public String User;
		public String Group;
		public String Definitions;
		public String NoUser;
		public String NoGroup;
		public String NodesWhere;

		public String HasBeenCreated;
		public String ArgumentsExpected;
		public String UserAndNodeExpected;
		public String UserAndGrpExpected;
		public String GrpAndNodeExpected;
		public String UpdatingAttribute;

		public String InvalidColorMsg;
		public String InvalidBooleanValue;
		public String AttributeExpected;

		public void LoadLanguages(PermissionsX permissions)
		{
			DisabledMessaged	= permissions.SetLanguageVariable("Disabled", "has been disabled.");
			Initializing		= permissions.SetLanguageVariable("Initializing", "Initializing...");
			Enabled				= permissions.SetLanguageVariable("Enabled", "Enabled");
			Protected			= permissions.SetLanguageVariable("Protected", "Your Server is now protected!");
			GetAuth				= permissions.SetLanguageVariable("GetAuth", "Your Server is vulnerable, Get an Authentication system!");
			NoAuth				= permissions.SetLanguageVariable("NoAuth", "No login system found! Restrict will be downloaded.");
			RestrictLoadFail	= permissions.SetLanguageVariable("RestrictLoadFail", "Restrict failed to install!");
			RestrictLoadResult	= permissions.SetLanguageVariable("RestrictLoadResult", "Load result:");
			RestrictDlFailed	= permissions.SetLanguageVariable("RestrictDlFailed", "Restrict failed to download!");
			
			Success				= permissions.SetLanguageVariable("Success", "success");
			Failure				= permissions.SetLanguageVariable("Failure", "failure");
			Failed				= permissions.SetLanguageVariable("Failed", "failed");
			Saving				= permissions.SetLanguageVariable("Saving", "saving");
			Loading				= permissions.SetLanguageVariable("Loading", "loading");

			Added				= permissions.SetLanguageVariable("Added", "Added");
			Removed				= permissions.SetLanguageVariable("Removed", "Removed");

			NoPlayersAndForce	= permissions.SetLanguageVariable("NoPlayersAndForce", "No online player found, Use -f if you know for certain that the name is correct.");
			DefinitionsExist	= permissions.SetLanguageVariable("DefinitionsExist", "Definitions already exist for that ");

			User				= permissions.SetLanguageVariable("User", "user");
			Group				= permissions.SetLanguageVariable("Group", "group");
			Definitions			= permissions.SetLanguageVariable("Definitions", "Definitions");
			HasBeenCreated		= permissions.SetLanguageVariable("HasBeenCreated", "has been created");
			ArgumentsExpected	= permissions.SetLanguageVariable("ArgumentsExpected", "Arguments expected");
			UserAndNodeExpected = permissions.SetLanguageVariable("UserAndNodeExpected", "User & permission node(s) expected");
			GrpAndNodeExpected	= permissions.SetLanguageVariable("GrpAndNodeExpected", "Group & permission node(s) expected");
			UserAndGrpExpected	= permissions.SetLanguageVariable("UserAndGrpExpected", "User & group(s) expected");
			NoUser				= permissions.SetLanguageVariable("NoUser", "No user");
			NoGroup				= permissions.SetLanguageVariable("NoGroup", "No group");
			NodesWhere			= permissions.SetLanguageVariable("NodesWhere", "node(s) where");

			UpdatingAttribute	= permissions.SetLanguageVariable("UpdatingAttribute", "updating attribute");
			InvalidColorMsg		= permissions.SetLanguageVariable("InvalidColorMsg", "Invalid color value, try");
			InvalidBooleanValue = permissions.SetLanguageVariable("InvalidBooleanValue", "Invalid boolean value");
			AttributeExpected	= permissions.SetLanguageVariable("AttributeExpected", "Attribute expected");
		}
	}
}
