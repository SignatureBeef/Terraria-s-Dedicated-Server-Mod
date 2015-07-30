CREATE PROCEDURE `SqlPermissions_UserNodes`(in prmUserName nvarchar(255))
BEGIN
	select p.Node, p.Deny
	from tdsm_users u
		left join SqlPermissions_UserPermissions up on u.Id = up.UserId
		inner join SqlPermissions_Permissions p on up.PermissionId = p.Id
	where u.Username = prmUserName
	order by p.Node, p.Deny;
END