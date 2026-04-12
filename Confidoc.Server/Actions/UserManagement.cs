using Confidoc.Server.Helpers;
using Confidoc.Server.Models;
using ConfidocLib;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace Confidoc.Server;

public partial class Actions
{
    #region Access
    /// <summary>
    /// Adds read access to a document for the specified group.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="id"></param>
    public void AddDocumentWriteAccess(ConfidocRole group, string id)
    {
        var document = GetDocumentByID(id);
        AddDocumentReadAccess(group, id);
        document!.WriteAccessGroups!.Add(group);
        _context.SaveChanges();
    } 

    /// <summary>
    /// Adds read access to a document for the specified group.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="id"></param>
    public void AddDocumentWriteAccess(ConfidocUser user, string id)
    {
        var document = GetDocumentByID(id);
        AddDocumentReadAccess(user, id);
        document!.WriteAccessUsers!.Add(user);
        _context.SaveChanges();
    } 

    /// <summary>
    /// Adds read access to a document for the specified user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    public void AddDocumentReadAccess(ConfidocRole group, string id)
    {
        var document = GetDocumentByID(id);
        document!.ReadAccessGroups!.Add(group);
        _context.SaveChanges();
    } 

    /// <summary>
    /// Adds read access to a document for the specified user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    public void AddDocumentReadAccess(ConfidocUser user, string id)
    {
        var document = GetDocumentByID(id);
        document!.ReadAccessUsers!.Add(user);
        _context.SaveChanges();
    } 

    /// <summary>
    /// Removes a group's access to a document.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="id"></param>
    public void RemoveDocumentAccess(ConfidocRole group, string id)
    {
        var document = GetDocumentByID(id);
        document!.ReadAccessGroups = document.ReadAccessGroups!
            .Where(rGroup => rGroup.Name != group.Name)
            .ToList();
        document!.WriteAccessGroups = document.ReadAccessGroups!
            .Where(wGroup => wGroup.Name != group.Name)
            .ToList();
        _context.SaveChanges();
    } 

    /// <summary>
    /// Removes user's access to the document
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    public void RemoveDocumentAccess(ConfidocUser user, string id)
    {
        var document = GetDocumentByID(id);
        document!.ReadAccessUsers = document.ReadAccessUsers!
            .Where(rUser => rUser.UserName != user.UserName)
            .ToList();
        document!.WriteAccessUsers = document.WriteAccessUsers!
            .Where(rUser => rUser.UserName != user.UserName)
            .ToList();
        _context.SaveChanges();
    } 

    /// <summary>
    /// Checks which level of access a user 
    /// has to a specific document.
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    public int DocumentAccessLevel(ClaimsPrincipal claim, Document document)
    {
        var user = GetUser(claim);
        return DocumentAccessLevel(user, document);
    }

    /// <summary>
    /// Checks which level of access a user 
    /// has to a specific document.
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public int DocumentAccessLevel(ClaimsPrincipal claim, string id)
    {
        var document = GetDocumentByID(id);
        if (document is null) return 0;
        return DocumentAccessLevel(claim, document);
    }

    /// <summary>
    /// Checks which level of access a user 
    /// has to a specific document.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="document"></param>
    /// <returns></returns>
    public int DocumentAccessLevel(ConfidocUser? user, Document document)
    {
        // level 0 = no access
        // level 1 = read
        // level 2 = read/write
        // level 3 = owner
        int access = 0;

        // has read access?
        var parsedDocument = ToParsedDocuments([document])[0];
        if ((parsedDocument.ReadAccessUsers ?? []).Contains("anonymous"))
            access = 1;

        if (user is null) return 0;
        var roles = _userManager.GetRolesAsync(user)
                    .GetAwaiter()
                    .GetResult();
        if (user is null) return access;
        if (document.ReadAccessUsers!.Contains(user))
            access = 1;
        if (document.ReadAccessGroups!.Any(x => roles.Contains(x.Name??"")))
            access = 1;
        if (GetUserGroups(user).Any(group => document!.ReadAccessGroups!.Any(g => g.Name == group)))
            access = 1;

        if (document.WriteAccessUsers!.Contains(user))
            access = 2;
        if (document.WriteAccessGroups!.Any(x => roles.Contains(x.Name??"")))
            access = 2;
        if (GetUserGroups(user).Any(group => document!.WriteAccessGroups!.Any(g => g.Name == group)))
            access = 2;
        
        if (document.Owner == user)
            access = 3;
        return access;
    }

    public int GroupAccessLevel(ConfidocUser? user, string group)
    {
        // level 0 = no access
        // level 1 = read
        // level 2 = read/write
        // level 3 = owner
        int access = 0;

        // has read access?
        var role = GetGroup(group)
                    .GetAwaiter()
                    .GetResult();
        if (role is null) return 0;
        if ((role.Members ?? []).Contains("anonymous"))
            access = 1;

        if (user is null) return 0;
        var roles = _userManager.GetRolesAsync(user)
                    .GetAwaiter()
                    .GetResult();
        if (user is null) return access;
        if (role.Members!.Contains(user.UserName))
            access = 1;
        if (role.Owner == user.UserName)
            access = 3;
        return access;
    }
    #endregion Access

    #region Groups

    /// <summary>
    /// Get list of groups the user is in.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string[] GetUserGroups(ConfidocUser user)
    {
        return _userManager.GetRolesAsync(user)
                .GetAwaiter()
                .GetResult()
                .ToArray();
    }
    
    /// <summary>
    /// Removes a group.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> DeleteGroup(string group, ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        if (user is null) throw Exceptions.Null;
        var role = await _roleManager.FindByNameAsync(group);
        if (role is null) throw Exceptions.Null;
        if (GroupAccessLevel(user, group) < 2)
            return null;
        
        return await _roleManager.DeleteAsync(role);
    }

    /// <summary>
    /// Checks if the user has access to view
    /// the group and returns with a parsed
    /// "full" view of the group's properties 
    /// (excluding sensitive information of other users).
    /// </summary>
    /// <param name="group"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<ParsedGroup?> GetGroup(string group, ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        if (user is null) throw Exceptions.Null;
        var role = await _roleManager.FindByNameAsync(group);
        if (role is null) throw Exceptions.Null;
        var parsedGroup = await ToParsedGroup(role);
        if (GroupAccessLevel(user, group) < 1)
            return null;
        return parsedGroup;
    }

    /// <summary>
    /// Returns ConfidocRole with the name specified.
    /// <b>THIS FUNCTION DOES NOT HAVE ANY SAFETY RAILS</b>
    /// </summary>
    /// <param name="group"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public ConfidocRole GetGroupUnsafe(string group)
    {
        var role = _roleManager.FindByNameAsync(group)
            .GetAwaiter()
            .GetResult();
        if (role is null) throw Exceptions.Null;
        return role;
    }

    /// <summary>
    /// returns a parsed "full" view of the group's properties 
    /// (excluding sensitive information of other users).
    /// </summary>
    /// <param name="group"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<ParsedGroup?> GetGroup(string group)
    {
        var role = await _roleManager.FindByNameAsync(group);
        if (role is null) return null;
        var parsedGroup = await ToParsedGroup(role);
        return parsedGroup;
    }

    /// <summary>
    /// Get groups a user is part of.
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ParsedGroup>> GetGroups(ConfidocUser user)
    {
        var groups = await _userManager.GetRolesAsync(user);
        var parsedGroups = groups.Select(groupName => {
            var group =  _roleManager.FindByNameAsync(groupName)
                    .GetAwaiter()
                    .GetResult();
            if (group is null) return null;
            return ToParsedGroup(group)
                    .GetAwaiter()
                    .GetResult();
        }).ToList();
        return parsedGroups!;
    }

    /// <summary>
    /// Adds the user to the group specified. Note, the group parameter
    /// must be the <b>GROUP ID, NOT THE DISPLAY NAME</b>.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> AddToGroup(string group, ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        if (user is null) throw Exceptions.Null;
        var owner = (await GetGroupOwner(group)??new(){UserName=""}).UserName;
        if (owner != user.UserName) return null;
        return await _userManager.AddToRoleAsync(user, group);
    }

    /// <summary>
    /// Get owner Confidoc object from group ID.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public async Task<ConfidocUser?> GetGroupOwner(string group)
    {
        var role = await _roleManager.FindByNameAsync(group);
        if (role is null) return null;
        var userId = role.OwnerId;
        if (userId is null) return null;
        return await _userManager.FindByIdAsync(userId);
    }

    /// <summary>
    /// Gets Confidoc object from user ID.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public ConfidocUser? GetUserByID(string userId)
    {
        if (userId is null) return null;
        return _userManager.FindByIdAsync(userId)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Checks that the user executing the function has enough 
    /// permissions to do so and if the use does, removes the 
    /// target user from the specified group/role.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="target"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> RemoveFromGroup(string group, ConfidocUser target, ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        if (user is null) throw Exceptions.Null;
        var owner = (await GetGroupOwner(group)??new(){UserName=""}).UserName;
        if (owner == target.UserName) return null;
        if (target.UserName != user.UserName && user.UserName != owner) return null;
        return await _userManager.RemoveFromRoleAsync(target, group);
    }

    /// <summary>
    /// Adds the user to the group specified. Note, the group parameter
    /// must be the <b>GROUP ID, NOT THE DISPLAY NAME</b>.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<IdentityResult?> AddToGroup(string group, ConfidocUser target, ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        if (user is null) throw Exceptions.Null;
        var owner = (await GetGroupOwner(group)??new(){UserName=""}).UserName;
        if (owner != user.UserName) return null;
        return await _userManager.AddToRoleAsync(target, group);
    }

    /// <summary>
    /// Creates a role. Note, the "<paramref name="name"/>" will NOT be the 
    /// "name" of the role, the name is a randomly generated GUID that is 
    /// returned.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public async Task<GroupCreationResult> CreateGroup(string name, ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        if (user is null) throw Exceptions.Null;

        var roleID = Guid.NewGuid().ToString();

        var role = new ConfidocRole
        {
            Id = Guid.NewGuid().ToString(),
            Owner = user,
            Name = roleID,
            NormalizedName = roleID.Trim().ToUpperInvariant(),
            DisplayName = name,
        };
        return new()
        {
            Id = roleID,
            Result = await _roleManager.CreateAsync(role)
        };
    } 

    public class GroupCreationResult
    {
        public string? Id { get; set; }
        public IdentityResult? Result { get; set; }
    }

    #endregion Groups

    #region Users
    /// <summary>
    /// For getting the ConfiDoc object from a claim.
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public ConfidocUser? GetUser(ClaimsPrincipal claim)
    {
        if (claim.Identity is null) return null;
        var username = claim.Identity.Name;
        if (username is null) return null;
        var user = _userManager.FindByNameAsync(username)
            .GetAwaiter()
            .GetResult();
        return user;
    }

    /// <summary>
    /// For getting the ConfiDoc object from a UserName.
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public ConfidocUser? GetUser(string userName)
    {
        var user =  _userManager.FindByNameAsync(userName)
                        .GetAwaiter()
                        .GetResult();
        if (user is null) return null;
        return user;
    }

    /// <summary>
    /// Creates a new user account and creates the associated
    /// keypair.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateUser(string username, string password)
    {
        var keypair = Security.GenerateKeyPair();
        var user = new ConfidocUser
        {
            UserName = username,
            PrivateKey = keypair.PrivateKey,
            PublicKey = keypair.PublicKey,
        };
        var result = await _userManager.CreateAsync(user, password);
        return result;
    }

    /// <summary>
    /// Changes user password.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<bool> ChangePassword(ConfidocUser? user, string password, string newPassword)
    {
        if (user is null) return false;
        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword) return false;

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (!result.Succeeded)
            Log.Error(JsonConvert.SerializeObject(result.Errors));
        if (!result.Succeeded) return false;
        return true;
    }

    /// <summary>
    /// Changes user password.
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="password"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public async Task<bool> ChangePassword(ClaimsPrincipal claim, string password, string newPassword)
        => await ChangePassword(GetUser(claim), password, newPassword);


    /// <summary>
    /// Anonymizes user data and deletes documents and roles owned by the person.
    /// </summary>
    /// <param name="user"></param>
    public async Task<bool> DeleteUser(ConfidocUser? user, string password)
    {
        if (user is null) return false;
        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword) return false;

        _context.Documents.Where(document => document.OwnerId == user.Id).ExecuteDelete();
        _context.Roles.Where(role => role.OwnerId == user.Id).ExecuteDelete();
        user.UserName = $"delete_user_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10)}";
        var randomBytes = new byte[1024];
        new Random().NextBytes(randomBytes);
        var randomPassword = Security.HashString(Convert.ToBase64String(randomBytes));
        return await ChangePassword(user, password, randomPassword);
    }

    /// <summary>
    /// Anonymizes user data and deletes documents and roles owned by the person.
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="password"></param>
    public async Task<bool> DeleteUser(ClaimsPrincipal claim, string password)
        => await DeleteUser(GetUser(claim), password);
    #endregion Users
}