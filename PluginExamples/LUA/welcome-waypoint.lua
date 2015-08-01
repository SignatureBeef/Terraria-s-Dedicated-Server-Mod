import ('System')
import ('System.IO')
import ('tdsm')
import ('TDSM.API')
import ('TDSM.API.Misc')
import ('TDSM.API.Plugin') --HookResult
import ('TDSM.API.Command') --AccessLevel
import ('TDSM.API.Logging') 

WelcomePlugin = {}
WelcomePlugin.__index = WelcomePlugin

function WelcomePlugin.create()
	local plg = {}
	setmetatable(plg, WelcomePlugin)
	
	--Set the details (TDSM requires this)
	plg.TDSMBuild = 3
	plg.Author = "TDSM"
	plg.Description = "TDSM LUA test plugin example"
	plg.Name = "TDSM LUA Example"
	plg.Version = "1"
	
	return plg
end

function WelcomePlugin:Initialized()
	--Register hooks
	export.Hooks = {}
	export.Hooks.PlayerEnteredGame = --This must be the event as per TDSMs API.
	{
		Call = export.OnPlayerEnteredGame,
		Priority = HookOrder.NORMAL --Normal is default and is here for demonstration purposes.
	}
	export.Hooks.PlayerEnteringGame =
	{
		Call = export.OnPlayerEnteringGame
	}
	--HookBase(HookPoints.PlayerEnteredGame, HookOrder.NORMAL, export.OnPlayerEnteredGame)

	--TODO seperate into seperate commands for adding and removing (permission reasons)
	AddCommand("wp")
		:WithAccessLevel(AccessLevel.PLAYER)
		:LuaCall(export.OnWPCommand)
		:WithDescription("Go to a way point")
		:WithHelpText("<name>")
		:WithHelpText("-add <name> <x> <y>")
		:WithHelpText("-add <name> <player name>")
		:WithHelpText("-remove <name>")
		--:WithHelpText("          wp next")
		--:WithHelpText("          wp previous")
		:WithPermissionNode("tdsm.waypoint")
		
	export.Waypoints = DataRegister(Path.Combine(Globals.DataPath, "way-point.txt"))
end

function WelcomePlugin:Enabled()
	export.Waypoints:Load();
	ProgramLog.Plugin:Log("Loaded " .. export.Waypoints.Count .. " waypoint(s)")
end

function WelcomePlugin:Disabled()
end

function WelcomePlugin:WorldLoaded()
end

function WelcomePlugin:OnWPCommand(sender, args)
	local adding = args:TryPop("-add")
	local removing = args:TryPop("-remove")
	
	if adding then
		local wp = args:GetString(0);
		
		local x = 0
		local y = 0
		if args.Count >= 3 then
			x = args:GetDouble(1);
			y = args:GetDouble(2);
		else
			local player = args:GetOnlinePlayer(1)
			x = player.position.X
			y = player.position.Y
		end
		Tools.WriteLine(wp .. " " .. tostring(x) .. "," .. tostring(y))
		if export.Waypoints:Add(wp, tostring(x) .. "," .. tostring(y)) then
			sender:SendMessage("Added way point " .. wp, 255, 0, 255, 0)
		else
			sender:SendMessage("Way point exists " .. wp, 255, 255, 0, 0)
		end
	elseif removing then
		local wp = args:GetString(0);
		if export.Waypoints:Remove(wp, true) then
			sender:SendMessage("Removed way point " .. wp, 255, 0, 255, 0)
		else
			sender:SendMessage("No such way point " .. wp, 255, 255, 0, 0)
		end
	else
--		--Teleport
		--if type(sender) == "Player" then
		if sender:IsPlayer() then
			local wp = args:GetString(0);
			
			local value = export.Waypoints:Find(wp)
			if value ~= nil then
				local ix = string.find(value, ",")
				
				local x = tonumber(string.sub(value, 0, ix - 1))
				local y = tonumber(string.sub(value, ix + 1))
				
				sender:Teleport(x, y);
				
				Tools.NotifyAllOps("Teleporting " .. sender.SenderName .. " to waypoint " .. wp)
			else
				sender:SendMessage("Cannot find way point " .. wp, 255, 255, 0, 0)
			end
		else
			sender:SendMessage("Only players can use this command", 255, 255, 0, 0)
		end
	end
end

function WelcomePlugin:OnPlayerEnteredGame(ctx, args)
	ctx.Player:SendMessage("It's good to see you again, " .. ctx.Player.name, 255, 0, 255, 0)
	Tools.WriteLine(ctx.Player.name .. " was welcomed")
end

function WelcomePlugin:OnPlayerEnteringGame(ctx, args)
	ctx:SetResult(HookResult.IGNORE) --Cancel event to prevent TDSM sending the MOTD & players.
	
	return ctx
end

export = WelcomePlugin.create() --Ensure this exists, as TDSM needs this to find your plugin