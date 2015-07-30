CREATE PROCEDURE `SqlPermissions_RemoveGroupNode`(in prmGroupName varchar(255), in prmNode varchar(255), in prmDeny bit)
BEGIN
	declare vGroupId int default 0;
	declare vNodeId int default 0;

	select Id
	from SqlPermissions_Groups g
	where g.Name = prmGroupName
	limit 1
	into vGroupId;
	
	select Id
	from SqlPermissions_Permissions
	where Node = prmNode
		and Deny = prmDeny
	into vNodeId;

	if vGroupId is not null and vNodeId is not null and vGroupId > 0 and vNodeId > 0 then
		delete from SqlPermissions_GroupPermissions
		where GroupId = vGroupId
			and PermissionId = vNodeId;

		select 1 Result; /* No fail required */
	else
		select 0 Result;
	end if;
END