CREATE PROCEDURE `SqlPermissions_AddOrUpdateGroup`(in prmName varchar(255), in prmApplyToGuests bit, in prmParent varchar(255), in prmR tinyint unsigned, in prmG tinyint unsigned, in prmB tinyint unsigned, in prmPrefix varchar(10), in prmSuffix varchar(10))
BEGIN
	if exists
	(
		select 1
		from SqlPermissions_Groups
		where Name = prmName
	) then
		update SqlPermissions_Groups
		set
			ApplyToGuests = prmApplyToGuests,
			Parent = prmParent,
			Chat_Red = prmR,
			Chat_Green = prmG,
			Chat_Blue = prmB,
			Chat_Prefix = prmPrefix,
			Chat_Suffix = prmSuffix
		where Name = prmName;

		select Id
		from SqlPermissions_Groups
		where Name = prmName;
	else
		insert SqlPermissions_Groups
		( Name, ApplyToGuests, Parent, Chat_Red, Chat_Green, Chat_Blue, Chat_Prefix, Chat_Suffix )
		select prmName, prmApplyToGuests, prmParent, prmR, prmG, prmB, prmPrefix, prmSuffix;
		
		select CAST(LAST_INSERT_ID() AS SIGNED);
	end if;
END