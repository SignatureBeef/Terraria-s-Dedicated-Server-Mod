CREATE PROCEDURE `SqlPermissions_AddNodeToUser`(in prmUserName varchar(50), in prmNode varchar(50), in prmDeny bit)
BEGIN
	declare vUserId int default 0;
	declare vNodeId int default 0;

	select Id
	from tdsm_users g
	where g.Username = prmUserName
	limit 1
	into vUserId;
	
	select Id
	from SqlPermissions_Permissions
	where Node = prmNode
		and Deny = prmDeny
	into vNodeId;

	if vNodeId is null or vNodeId = 0 then
		insert into SqlPermissions_Permissions
		( Node, Deny )
		select prmNode, prmDeny;
		set vNodeId = LAST_INSERT_ID();
	end if;

	if vNodeId > 0 and vUserId > 0 then
		if not exists
		(
			select 1
			from SqlPermissions_UserPermissions
			where UserId = vUserId
				and PermissionId = vNodeId
		) then
			insert into SqlPermissions_UserPermissions
			( UserId, PermissionId )
			select vUserId, vNodeId;
		end if;

		select 1 Result; /* No fail required */
	else
		select 0 Result;
	end if;
END