CREATE PROCEDURE `SqlPermissions_GroupNodes`(in prmGroupName varchar(255))
BEGIN
	select nd.Node, nd.Deny
	from SqlPermissions_Groups g
		inner join SqlPermissions_GroupPermissions gp on g.Id = gp.GroupId
		inner join SqlPermissions_Permissions nd on gp.PermissionId = nd.Id
	where g.Name = prmGroupName
	order by nd.Node;
END