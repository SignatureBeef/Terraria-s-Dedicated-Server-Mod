CREATE PROCEDURE `SqlPermissions_GroupList`()
BEGIN
	select Name
	from SqlPermissions_Groups
	order by Name;
END