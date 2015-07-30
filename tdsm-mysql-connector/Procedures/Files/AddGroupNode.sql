CREATE PROCEDURE `SqlPermissions_AddGroupNode`(in prmGroupName varchar(50), in prmNode varchar(50), in prmDeny bit)
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

	if vNodeId is null or vNodeId = 0 then
		insert into SqlPermissions_Permissions
		( Node, Deny )
		select prmNode, prmDeny;
		set vNodeId = LAST_INSERT_ID();
	end if;

	if vGroupId > 0 and vNodeId > 0 then
		if not exists
		(
			select 1
			from SqlPermissions_GroupPermissions
			where GroupId = vGroupId
				and PermissionId = vNodeId
		) then
			insert into SqlPermissions_GroupPermissions
			( GroupId, PermissionId )
			select vGroupId, vNodeId;
		end if;

		select 1 Result; /* No fail required */
	else
		select 0 Result;
	end if;
END