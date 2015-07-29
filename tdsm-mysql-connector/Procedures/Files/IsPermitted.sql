CREATE PROCEDURE `SqlPermissions_IsPermitted`(in prmNode varchar(50), in prmIsGuest bit, in prmAuthentication varchar(50))
BEGIN
	declare vPermissionValue int default 0;
	declare vUserId int default 0;
	declare vGroupId int default 0;
	declare vPrevGroupId int default 0;
	declare vNodeFound int default 0;
	/*
		PermissionEnum values:
			0	= Denied
			1	= Permitted
	*/

	if prmIsGuest = 0 and prmAuthentication is not null and CHAR_LENGTH(prmAuthentication) > 0 then
		select Id
		from tdsm_users u
		where u.Username = prmAuthentication
		limit 1
		into vUserId;

		if vUserId > 0 then
			/*
				If the user has specific nodes then use them
				If not then search for a group
				If still none then try the guest permissions
			*/

			/*Do we have any nodes?*/
			if exists
			(
				select 1
				from tdsm_users u
					left join SqlPermissions_UserPermissions up on u.Id = up.UserId
					left join SqlPermissions_Permissions nd on up.PermissionId = nd.Id
				where u.Id = vUserId
					and (nd.Node = prmNode or nd.Node = "*")
			) then
				if exists
				(
					select 1
					from tdsm_users u
						left join SqlPermissions_UserPermissions up on u.Id = up.UserId
						left join SqlPermissions_Permissions nd on up.PermissionId = nd.Id
					where u.Id = vUserId
						and (nd.Node = prmNode or nd.Node = "*")
						and nd.Deny = 0
				) then
					set vPermissionValue = 1;
					set vNodeFound = 1;
				else
					set vPermissionValue = 0;
					set vNodeFound = 1;
				end if;
			else
				/*
					For each group, see if it has a permission
					Else, if it has a parent recheck.
					Else guestMode
				*/

				/* Get the first group */
				select GroupId
				from SqlPermissions_UserGroups u
				where u.UserId = vUserId
				limit 1
				into vGroupId;

				set vPrevGroupId = vGroupId;
				set vNodeFound = 0;

				while (vGroupId is not null and vGroupId > 0 and vNodeFound = 0) do
					/* Check group permissions */
					select vGroupId;
					if exists
					(
						select 1
						from SqlPermissions_GroupPermissions gp
							left join SqlPermissions_Permissions pm on gp.PermissionId = pm.Id
						where gp.GroupId = vGroupId
							and (pm.Node = prmNode or pn.Node = "*")
							and pm.Deny = 1
					) then
						set vPermissionValue = 0;
						set vGroupId = 0;
						set vNodeFound = 1;
					elseif exists
					(
						select 1
						from SqlPermissions_GroupPermissions gp
							left join SqlPermissions_Permissions pm on gp.PermissionId = pm.Id
						where gp.GroupId = vGroupId
							and (pm.Node = prmNode or pn.Node = "*")
							and pm.Deny = 0
					) then
						set vPermissionValue = 1;
						set vGroupId = 0;
						set vNodeFound = 1;
					else
						select Id
						from SqlPermissions_Groups g
						where g.Name = (
							select c.Parent
							from SqlPermissions_Groups c
							where c.Id = vGroupId
							limit 1
						)
						limit 1
						into vGroupId;

						if vPrevGroupId = vGroupId then
							set vGroupId = 0;
						end if;

						set vPrevGroupId = vGroupId;
					end if;
				end while;

				if 1 <> vNodeFound then
					set prmIsGuest = 1;
				end if;
			end if;
		else
			/* Invalid user - try guest */
			set prmIsGuest = 1;
		end if;
	end if;

	if vNodeFound = 0 and prmIsGuest = 1 then
		if exists
		(
			select 1
			from SqlPermissions_Groups gr
				left join SqlPermissions_GroupPermissions gp on gr.Id = gp.GroupId
				left join SqlPermissions_Permissions nd on gp.PermissionId = nd.Id
			where gr.ApplyToGuests = 1
				and (nd.Node = prmNode or nd.Node = "*")
				and nd.Deny = 0
		) then
			set vPermissionValue = 1;
			set vNodeFound = 1;
		end if;
	end if;

	select vPermissionValue PermissionEnum;
END