using System;
using TDSM.API.Command;
using TDSM.API.Data;
using Microsoft.Xna.Framework;
using TDSM.API.Logging;

namespace TDSM.Core
{
    public partial class Entry
    {
        void GroupPermission(ISender sender, ArgumentList args)
        {
            if (!Storage.IsAvailable)
                throw new CommandError("No permissions plugin or data plugin is attached");

            int a = 0;
            string groupName;
            var cmd = args.GetString(a++);
            switch (cmd)
            {
                case "add":
                    //group add "Guest" <ApplyToGuests> <Parent> <R> <G> <B> <Prefix> <Suffix>
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after [" + cmd + "]");

                    bool applyToGuests;
                    args.TryGetBool(a++, out applyToGuests);

                    string parent;
                    args.TryGetString(a++, out parent);

                    byte r;
                    if (!args.TryGetByte(a++, out r))
                        r = 255;
                    byte g;
                    if (!args.TryGetByte(a++, out g))
                        g = 255;
                    byte b;
                    if (!args.TryGetByte(a++, out b))
                        b = 255;

                    string prefix;
                    args.TryGetString(a++, out prefix);

                    string suffix;
                    args.TryGetString(a++, out suffix);

                    if (Storage.FindGroup(groupName) != null)
                        throw new CommandError("There is already a group defined as " + groupName);

                    if (Storage.AddOrUpdateGroup(groupName, applyToGuests, parent, r, g, b, prefix, suffix))
                    {
                        sender.Message("Successfully created group " + groupName, Color.Green);
                    }
                    else
                    {
                        sender.Message("Failed to create group " + groupName, Color.Red);
                    }
                    break;
                case "remove":
                    //group remove "Guest
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after [" + cmd + "]");

                    if (Storage.FindGroup(groupName) == null)
                        throw new CommandError("Group does not exist: " + groupName);

                    if (Storage.RemoveGroup(groupName))
                    {
                        sender.Message("Successfully removed group " + groupName, Color.Green);
                    }
                    else
                    {
                        sender.Message("Failed to remove group " + groupName, Color.Red);
                    }
                    break;
                case "update":
                    //group remove "Guest"
                    break;

                case "addnode":
                    //group addnode "Guest" "tdsm.help" <deny>
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after [" + cmd + "]");

                    string addNode;
                    if (!args.TryGetString(a++, out addNode))
                        throw new CommandError("Expected node name after group name");

                    bool deny;
                    if (!args.TryGetBool(a++, out deny))
                        deny = false;
                    
                    if (Storage.FindGroup(groupName) == null)
                        throw new CommandError("Group does not exist: " + groupName);
                    
                    if (Storage.AddGroupNode(groupName, addNode, deny))
                    {
                        sender.Message(String.Format("Successfully added node {0} to group {1} ", addNode, groupName), Color.Green);
                    }
                    else
                    {
                        sender.Message(String.Format("Failed to add node {0} to group {1} ", addNode, groupName), Color.Red);
                    }
                    break;
                case "removenode":
                    //group removenode "Guest" "tdsm.help"
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after [" + cmd + "]");

                    string remNode;
                    if (!args.TryGetString(a++, out remNode))
                        throw new CommandError("Expected node name after group name");

                    if (Storage.FindGroup(groupName) == null)
                        throw new CommandError("Group does not exist: " + groupName);

                    bool denied;
                    if (!args.TryGetBool(a++, out denied))
                        denied = false;

                    if (Storage.RemoveGroupNode(groupName, remNode, denied))
                    {
                        sender.Message(String.Format("Successfully removed node {0} from group {1} ", remNode, groupName), Color.Green);
                    }
                    else
                    {
                        sender.Message(String.Format("Failed to remove node {0} from group {1} ", remNode, groupName), Color.Red);
                    }
                    break;

                case "list":
                    //group list
                    var groups = Storage.GroupList();
                    if (groups != null && groups.Length > 0)
                    {
                        sender.Message("Current groups:");
                        foreach (var grp in groups)
                        {
                            sender.Message("\t" + grp);
                        }
                    }
                    else
                    {
                        sender.Message("There are no registered groups.");
                    }
                    break;

                case "listnodes":
                    //group listnodes "Guest"
                    //Could be used in 'list' but this will make things a bit more simple for the end user.
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after [" + cmd + "]");

                    var grpList = Storage.FindGroup(groupName);
                    if (grpList == null)
                        throw new CommandError("Group does not exist: " + groupName);

                    var nodes = Storage.GroupNodes(groupName);
                    if (nodes != null && nodes.Length > 0)
                    {
                        sender.Message("Current permissions for group {0}:", grpList.Name);
                        foreach (var nd in nodes)
                        {
                            sender.Message("\t{0}\t- {1}", nd.Deny ? "Denied" : "Allowed", nd.Node);
                        }
                    }
                    else
                    {
                        sender.Message("There are no permissions assigned to group: " + grpList.Name);
                    }
                    break;
                default:
                    throw new CommandError("Invalid command " + cmd);
            }
        }

        void UserPermission(ISender sender, ArgumentList args)
        {
            if (!Storage.IsAvailable)
                throw new CommandError("No permissions plugin or data plugin is attached");
            
            int a = 0;
            string username, groupName, node, password;
            UserDetails? user;
            Group grp;
            bool deny, op;

            var cmd = args.GetString(a++);
            switch (cmd)
            {
                case "addgroup":
                    //user addgroup "username" "group"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after username");

                    user = AuthenticatedUsers.GetUser(username);
                    if (null == user)
                        throw new CommandError("No user found by: " + username);

                    grp = Storage.FindGroup(groupName);
                    if (grp == null)
                        throw new CommandError("Group does not exist: " + groupName);

                    if (Storage.AddUserToGroup(user.Value.Username, grp.Name))
                    {
                        sender.Message(String.Format("Successfully added {0} to group {1} ", user.Value.Username, grp.Name), Color.Green);
                    }
                    else
                    {
                        sender.Message(String.Format("Failed to add {0} from group {1} ", user.Value.Username, grp.Name), Color.Red);
                    }
                    break;
                case "removegroup":
                    //user removegroup "username" "group"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");
                    if (!args.TryGetString(a++, out groupName))
                        throw new CommandError("Expected group name after username");

                    user = AuthenticatedUsers.GetUser(username);
                    if (null == user)
                        throw new CommandError("No user found by: " + username);

                    grp = Storage.FindGroup(groupName);
                    if (grp == null)
                        throw new CommandError("Group does not exist: " + groupName);

                    if (Storage.RemoveUserFromGroup(user.Value.Username, grp.Name))
                    {
                        sender.Message(String.Format("Successfully removed {0} to group {1} ", user.Value.Username, grp.Name), Color.Green);
                    }
                    else
                    {
                        sender.Message(String.Format("Failed to remove {0} from group {1} ", user.Value.Username, grp.Name), Color.Red);
                    }
                    break;

                case "addnode":
                    //user addnode "username" "node" "deny"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");
                    if (!args.TryGetString(a++, out node))
                        throw new CommandError("Expected node name after username");

                    if (!args.TryGetBool(a++, out deny))
                        deny = false;

                    user = AuthenticatedUsers.GetUser(username);
                    if (null == user)
                        throw new CommandError("No user found by: " + username);

                    if (Storage.AddNodeToUser(user.Value.Username, node, deny))
                    {
                        sender.Message(String.Format("Successfully added {0} to user {1} ", node, user.Value.Username), Color.Green);
                    }
                    else
                    {
                        sender.Message(String.Format("Failed to add {0} from user {1} ", node, user.Value.Username), Color.Red);
                    }
                    break;
                case "removenode":
                    //user removenode "username" "node" "deny"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");
                    if (!args.TryGetString(a++, out node))
                        throw new CommandError("Expected node name after username");

                    if (!args.TryGetBool(a++, out deny))
                        deny = false;

                    user = AuthenticatedUsers.GetUser(username);
                    if (null == user)
                        throw new CommandError("No user found by: " + username);

                    if (Storage.RemoveNodeFromUser(user.Value.Username, node, deny))
                    {
                        sender.Message(String.Format("Successfully removed {0} to user {1} ", node, user.Value.Username), Color.Green);
                    }
                    else
                    {
                        sender.Message(String.Format("Failed to remove {0} from user {1} ", node, user.Value.Username), Color.Red);
                    }
                    break;

                case "groups":
                case "listgroups":
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");

                    user = AuthenticatedUsers.GetUser(username);
                    if (null == user)
                        throw new CommandError("No user found by: " + username);

                    var groups = Storage.UserGroupList(username);
                    if (groups != null && groups.Length > 0)
                    {
                        ProgramLog.Admin.Log("Current groups:");
                        foreach (var gps in groups)
                        {
                            sender.Message("\t" + gps);
                        }
                    }
                    else
                    {
                        sender.Message("There are no registered groups for user " + user.Value.Username);
                    }
                    break;

                case "nodes":
                case "listnodes":
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");

                    user = AuthenticatedUsers.GetUser(username);
                    if (null == user)
                        throw new CommandError("No user found by: " + username);

                    var nodes = Storage.UserNodes(username);
                    if (nodes != null && nodes.Length > 0)
                    {
                        sender.Message("Current permissions for user {0}:", user.Value.Username);
                        foreach (var nd in nodes)
                        {
                            sender.Message("\t{0}\t- {1}", nd.Deny ? "Denied" : "Allowed", nd.Node);
                        }
                    }
                    else
                    {
                        sender.Message("There are no permissions assigned to user: " + user.Value.Username);
                    }
                    break;

                case "search":
                    //user search "part"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected part of a users name after [" + cmd + "]");

                    var matches = AuthenticatedUsers.FindUsersByPrefix(username);
                    if (matches != null && matches.Length > 0)
                    {
                        sender.Message("Matches:");
                        foreach (var mth in matches)
                        {
                            sender.Message("\t" + mth);
                        }
                    }
                    else
                    {
                        sender.Message("There are no registered users matching " + username);
                    }
                    break;

                case "add":
                    //user add "username" "password" "op"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");
                    
                    if (!args.TryGetString(a++, out password))
                        throw new CommandError("Expected password name after username");

                    args.TryGetBool(a++, out op);

                    var existing = AuthenticatedUsers.GetUser(username);
                    if (existing == null)
                    {
                        if (AuthenticatedUsers.CreateUser(username, password, op))
                        {
                            if (op)
                            {
                                sender.Message("Successfully created user as operator: " + username);
                            }
                            else
                            {
                                sender.Message("Successfully created user " + username);
                            }
                        }
                        else
                            throw new CommandError("User failed to be created");
                    }
                    else
                        throw new CommandError("A user already exists by the name " + username);
                    break;

                case "update":
                    //user add "username" "password" "op"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");
                    
                    if (!args.TryGetString(a++, out password))
                        throw new CommandError("Expected password name after username");

                    args.TryGetBool(a++, out op);

                    var updatee = AuthenticatedUsers.GetUser(username);
                    if (updatee != null)
                    {
                        if (AuthenticatedUsers.UpdateUser(username, password, op))
                        {
                            if (op)
                            {
                                sender.Message("Successfully updated user as operator: " + username);
                            }
                            else
                            {
                                sender.Message("Successfully updated user " + username);
                            }
                        }
                        else
                            throw new CommandError("User failed to be updated");
                    }
                    else
                        throw new CommandError("No user exists by the name " + username);
                    break;

                case "remove":
                    //user remove "username"
                    if (!args.TryGetString(a++, out username))
                        throw new CommandError("Expected username name after [" + cmd + "]");

                    var delUser = AuthenticatedUsers.GetUser(username);
                    if (delUser != null)
                    {
                        if (AuthenticatedUsers.DeleteUser(username))
                        {
                            sender.Message("Successfully removed user " + username);
                        }
                        else
                            throw new CommandError("User failed to be removed");
                    }
                    else
                        throw new CommandError("Cannot find user " + username);
                    break;
                default:
                    throw new CommandError("Invalid command " + cmd);
            }
        }
    }
}

