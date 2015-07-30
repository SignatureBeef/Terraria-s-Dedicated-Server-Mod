CREATE PROCEDURE `SqlPermissions_RemoveNodeFromUser`(in prmUserName varchar(255), in prmNode varchar(255), in prmDeny bit)
BEGIN
	declare vUserId int default 0;
	declare vNodeId int default 0;

	select Id
	from tdsm_users u
	where u.Username = prmUserName
	limit 1
	into vUserId;
	
	select Id
	from SqlPermissions_Permissions
	where Node = prmNode
		and Deny = prmDeny
	into vNodeId;

	if vUserId is not null and vNodeId is not null and vUserId > 0 and vNodeId > 0 then
		delete from SqlPermissions_UserPermissions
		where UserId = vUserId
			and PermissionId = vNodeId;

		select 1 Result; /* No fail required */
	else
		select 0 Result;
	end if;
END