using Confidoc.Server.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace Confidoc.Server
{
    public class ParsedGrant
    {
        public string? Id { get; set; }
        public bool Expired { get; set; }
        public DateTime? Ends { get; set; }
        public DateTime? Starts { get; set; }
        public string? Grantee { get; set; }
        public string? Receiver { get; set; }
        public int? Level { get; set; }
        public string? ReceiverType { get; set; }
        public string? ResourceType { get; set; }
        public string? ResourceId { get; set; }
    }

    public partial class Actions
    {
        /// <summary>
        /// Returns all grants
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParsedGrant> GetGrants()
        {
            return ParseGrants(
                _context.Grants
                    .Include(g => g.Grantee)
                    .ToList()
                    .OrderByDescending(g => g.Starts)
                );
        }

        /// <summary>
        /// Removes entities that no longer have access to the document from 
        /// the user facing access list.
        /// </summary>
        public void SyncDocuments()
        {
            foreach (var grant in ParseGrants(_context.Grants))
            {
                if (!grant.Expired) continue;
                if (grant.ResourceType != "document" || grant.ResourceId is null) continue;
                var document = GetDocumentByID(grant.ResourceId);
                if (document is null) continue;
                if (grant.Level == 1)
                {
                    if (document.ReadAccessGroups is null || document.ReadAccessUsers is null) continue;
                    var removableGroups = document.ReadAccessGroups.Where(e => e.Name == grant.Receiver);
                    var removableUsers = document.ReadAccessUsers.Where(e => e.UserName == grant.Receiver);
                    foreach (var user in removableUsers)
                        document.ReadAccessUsers.Remove(user);
                    foreach (var group in removableGroups)
                        document.ReadAccessGroups.Remove(group);
                }
                else if (grant.Level == 2)
                {
                    if (document.WriteAccessGroups is null || document.WriteAccessUsers is null) continue;
                    var removableGroups = document.WriteAccessGroups.Where(e => e.Name == grant.Receiver);
                    var removableUsers = document.WriteAccessUsers.Where(e => e.UserName == grant.Receiver);
                    foreach (var user in removableUsers)
                        document.WriteAccessUsers.Remove(user);
                    foreach (var group in removableGroups)
                        document.WriteAccessGroups.Remove(group);
                }
                _context.SaveChanges();
            }       
        }

        /// <summary>
        /// Follows the same standard, 1 = read, 2 = read&write.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public int? GrantLevelForDocument(ConfidocUser? user, string documentId)
            => ParseGrants(_context.Grants.Include(g => g.Grantee)).Where(grant =>
            {
                var final = true;
                if (grant.ResourceId != documentId) final = false;
                if (grant.ResourceType != "document") final = false;
                if (user is null || user.UserName is null || grant.Receiver != user.UserName) final = false;
                if (grant.Expired) final = false;
                return final;
            }).FirstOrDefault()?.Level;

        public int? GrantLevelForDocument(ClaimsPrincipal claim, string documentId)
            => GrantLevelForDocument(GetUser(claim), documentId);
        

        public string CreateDocumentGrant(
            ConfidocUser grantee,
            string receiver,
            string receiverType,
            int level,
            string document,
            double duration)
        {
            var grant = new Grant
            {
                Id = Guid.NewGuid().ToString(),
                Starts = DateTime.Now,
                Ends = DateTime.Now.AddHours(duration),
                Grantee = grantee,
                Level = level,
                Receiver = receiver,
                ReceiverType = receiverType,
                ResourceId = GetDocumentByID(document)!.Id,
                ResourceType = "document",
            };
            _context.Grants.Add(grant);
            _context.SaveChanges();
            return grant.Id;
        }

        public async Task<string?> CreateGroupDocumentGrant(
            ClaimsPrincipal claim,
            string receiver,
            string document,
            int level,
            double duration = .5)
        {
            var user = GetUser(claim);
            if (user is null) return null;
            return CreateDocumentGrant(user, receiver, "group", level, document, duration);
        }

        public string? CreateUserDocumentGrant(
            ConfidocUser? user,
            string receiver,
            string document, 
            int level,
            double duration=.5)
        {
            if (user is null) return null;
            var receiverUser = GetUser(receiver);
            if (receiverUser is null || receiverUser.UserName is null) return null;
            return CreateDocumentGrant(user, receiverUser.UserName, "user", level, document, duration);
        }

        public string? CreateUserDocumentGrant(
            ClaimsPrincipal claim,
            string receiver,
            string document,
            int level,
            double duration = .5)
            => CreateUserDocumentGrant(GetUser(claim), receiver, document, level, duration);

        /// <summary>
        /// Parses out sensitive information from raw database results
        /// </summary>
        /// <param name="grants"></param>
        /// <returns></returns>
        public IEnumerable<ParsedGrant> ParseGrants(IEnumerable<Grant> grants)
        {
            return grants.Select(grant =>
                 new ParsedGrant
                 {
                     Id = grant.Id,
                     Grantee = grant.Grantee is null ? "" : grant.Grantee.UserName,
                     Receiver = grant.Receiver,
                     ReceiverType = grant.ReceiverType,
                     Level = grant.Level,
                     Starts = grant.Starts,
                     Ends = grant.Ends,
                     Expired = DateTime.Now > grant.Ends,
                     ResourceId = grant.ResourceId,
                     ResourceType = grant.ResourceType,
                 });
        }
    }
}
