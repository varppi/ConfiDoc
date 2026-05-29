using Confidoc.Server.Models;
using ConfidocLib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace Confidoc.Server;

public partial class Actions
{


    /// <summary>
    /// Converts the normal markdown document into a PDF
    /// form.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public ParsedDocument ToPdfDocument(ParsedDocument document, string user, bool download=true, string? data=null)
    {
        var pdfDoc = new ParsedDocument()
        {
            Id = document.Id,
            Name = document.Name,
            Level = document.Level,
            Events = [],
            Grants = (document.Grants ?? []).Where(g => g.Receiver == user),
            Changes = [],
            Data = download ? Convert.ToBase64String(Pdf.PdfToImagePdf(Pdf.MdToPdf(document.Data ?? ""), data: data??"a")) : "",
            Created = document.Created,
            LastModified = document.LastModified,
            Owner = document.Owner,
            Encrypted = document.Encrypted,
            ReadAccessGroups = [],
            WriteAccessGroups = [],
            ReadAccessUsers = [user],
            WriteAccessUsers = [],
        };
        return pdfDoc;
    }

    /// <summary>
    /// Timestamp converted to ticks for easier parsing.
    /// </summary>
    /// <param name="eve"></param>
    /// <returns></returns>
    public ParsedEvent ToParsedEvent(Event eve)
        => new ParsedEvent
        {
            Id = eve.Id,
            Action = eve.Action,
            Timestamp = (eve.Timestamp ?? DateTime.MinValue).Ticks,
            Ip = eve.Ip,
            User = eve.User,
            UserAgent = eve.UserAgent,
        };

    /// <summary>
    /// Converts the original collection of documents into an
    /// easily ingestable form that doesn't leak needless
    /// information.
    /// </summary>
    /// <param name="documents"></param>
    /// <returns></returns>
    private List<ParsedDocument> ToParsedDocuments(IEnumerable<Document> documents, string? password=null)
    {
        return documents.Select(document => new ParsedDocument
        {
            Id = document.Id,
            Name = document.Name,
            Events = _context.Events
                .ToList()
                .Where(eve => Regex.IsMatch(eve.Action??"", @$"^.*:{document.Id}$"))
                .Select(eve => ToParsedEvent(eve))
                .OrderByDescending(eve => eve.Timestamp),
            Created = (document.Created ?? DateTime.MinValue).Ticks,
            LastModified = (document.LastModified ?? DateTime.MinValue).Ticks,
            Owner = document.Owner!.UserName,
            Encrypted = document.Encrypted,
            Grants = ParseGrants(_context.Grants.Include(g => g.Grantee).Where(g => g.ResourceId == document.Id)),
            ReadAccessUsers = (document.ReadAccessUsers ?? []).Select(user => user.UserName!),
            WriteAccessUsers = (document.WriteAccessUsers ?? []).Select(user => user.UserName!),
            ReadAccessGroups = (document.ReadAccessGroups ?? []).Select(group => group.Name!),
            WriteAccessGroups = (document.WriteAccessGroups ?? []).Select(group => group.Name!),
            Changes = ReconstructDocument(document.Id ?? "", password).Changes!.Select(
                change => ToParsedChange(change)
            ),
            Data = Encoding.Latin1.GetString(Convert.FromBase64String(ReconstructDocument(document.Id ?? "", password).Data!))
        }).ToList();
    }

    /// <summary>
    /// User account information with
    /// sensitive stuff like passwordhash
    /// and private key informatiomn removed.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private ParsedUser ToParsedUser(ConfidocUser user)
    {
        return new ParsedUser
        {
            Id = user.Id,
            UserName = user.UserName,
            PublicKey = user.PublicKey
        };
    }

    /// <summary>
    /// Parses out unneseccary and sensitive
    /// information from the ReconstructedChange object.
    /// </summary>
    /// <param name="change"></param>
    /// <returns></returns>
    private ParsedChange ToParsedChange(ReconstructedChange change)
    {
        return new ParsedChange
        {
            Id = change.Change!.Id,
            Patch = change.Change.Patch,
            Signature = change.Change.Signature,
            Timestamp = (change.Change.Timestamp ?? DateTime.MinValue).Ticks,
            IsValidSignature = change.IsValidSignature,
            Owner = change.Change.Owner!.UserName,
        };
    }

    /// <summary>
    /// Parses out unneseccary and sensitive information
    /// from the ConfidocRole object.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    private async Task<ParsedGroup?> ToParsedGroup(ConfidocRole group)
    {
        var members = await _userManager.GetUsersInRoleAsync(group.Name??"");
        return new ParsedGroup
        {
            Id = group.Name,
            Owner = group.Owner!.UserName,
            DisplayName = group.DisplayName,
            Members = members.Select(user => user.UserName??""),
        };
    }
}