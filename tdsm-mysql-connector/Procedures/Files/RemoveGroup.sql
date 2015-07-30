CREATE PROCEDURE `SqlPermissions_RemoveGroup`(in prmName varchar(255))
BEGIN
	delete from SqlPermissions_Groups
	where Name = prmName;
END