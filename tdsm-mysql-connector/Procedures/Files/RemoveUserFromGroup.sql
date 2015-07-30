CREATE PROCEDURE `SqlPermissions_RemoveUserFromGroup`(in prmUserName varchar(255), in prmGroupName varchar(255))
BEGIN
	declare vGroupId int default 0;
	declare vUserId int default 0;

	select Id
	from SqlPermissions_Groups g
	where g.Name = prmGroupName
	limit 1
	into vGroupId;

	select Id
	from tdsm_users g
	where g.Username = prmUserName
	limit 1
	into vUserId;

	if vUserId is not null and vGroupId is not null and vUserId > 0 and vGroupId > 0 then
		delete from SqlPermissions_UserGroups
		where UserId = vUserId
			and GroupId = vGroupId;

		select 1 Result;
	else
		select 0 Result;
	end if;
END