CREATE PROCEDURE `SqlPermissions_FindGroup`(in prmName varchar(255))
BEGIN
	select Name
	from SqlPermissions_Groups
	where Name = prmName
	order by Name;
END