CREATE PROCEDURE `SqlPermissions_UserGroupList`(in prmUserName varchar(255))
BEGIN
	select g.Name
	from tdsm_users u
		left join SqlPermissions_UserGroups ug on u.Id = ug.UserId
		left join SqlPermissions_Groups g on ug.GroupId = g.Id
	where u.Username = prmUserName
	order by g.Name;
END