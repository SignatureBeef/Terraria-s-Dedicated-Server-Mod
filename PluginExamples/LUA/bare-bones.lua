import ('System')
import ('System.IO')
import ('tdsm')
import ('tdsm.api')
import ('tdsm.api.Misc')
import ('tdsm.api.Plugin') --HookResult
import ('tdsm.api.Command') --AccessLevel

YourPlugin = {}
YourPlugin.__index = YourPlugin

function YourPlugin.create()
	local plg = {}
	setmetatable(plg, YourPlugin)

	--Set the details (TDSM requires this)
	plg.TDSMBuild = 1
	plg.Version = "1"
	plg.Author = "TDSM"
	plg.Name = "Simple name"
	plg.Description = "This plugin does these awesome things!"
	
	return plg
end

function YourPlugin:Initialized()
end

function YourPlugin:Enabled()
end

function YourPlugin:Disabled()
end

function YourPlugin:WorldLoaded()
end

export = YourPlugin.create() --Ensure this exists, as TDSM needs this to find your plugin