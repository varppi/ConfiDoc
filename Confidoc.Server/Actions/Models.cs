using Confidoc.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Confidoc.Server;

public class ParsedGroup
{
    public string? Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Owner { get; set; }
    public IEnumerable<string>? Members {get;set;}
}

public class ParsedChange
{
    public string? Id { get; set; }
    public string? Patch { get; set; }
    public string? Signature { get; set; }
    public string? Owner { get; set; }
    public long? Timestamp { get; set; }
    public bool? IsValidSignature { get; set; }
}

public class ParsedDocument
{
    public string? Id { get; set; }
    public int? Level { get; set; }
    public string? Name { get; set; }
    public IEnumerable<ParsedEvent>? Events { get; set; }
    public IEnumerable<ParsedChange>? Changes { get; set; }
    public string? Data { get; set; }
    public long? Created { get; set; }
    public long? LastModified { get; set; }
    public string? Owner { get; set; }
    public string? Encrypted { get; set; }
    public IEnumerable<ParsedGrant>? Grants { get; set; }
    public IEnumerable<string>? ReadAccessUsers { get; set; }
    public IEnumerable<string>? WriteAccessUsers { get; set; }
    public IEnumerable<string>? ReadAccessGroups { get; set; }
    public IEnumerable<string>? WriteAccessGroups { get; set; }
}

public class ParsedEvent
{
    public string? Id { get; set; }
    public string? User { get; set; }
    public string? UserAgent { get; set; }
    public string? Ip { get; set; }
    public long? Timestamp { get; set; }
    public string? Action { get; set; }
}

public class ParsedGrant
{
    public string? Id { get; set; }
    public bool Expired { get; set; }
    public long? Ends { get; set; }
    public long? Starts { get; set; }
    public int? MinutesLeft { get; set; }
    public string? Grantee { get; set; }
    public string? Receiver { get; set; }
    public int? Level { get; set; }
    public string? ReceiverType { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
}

public class ParsedUser
{
    public string? UserName { get; set; }
    public string? Id { get; set; }
    public string? PublicKey { get; set; }
}

public class ReconstructedDocument
{
    public string? Data { get; set; }
    public List<ReconstructedChange>? Changes { get; set; }
}

public class ReconstructedChange
{
    public Change? Change { get; set; }
    public bool? IsValidSignature { get; set; }
}