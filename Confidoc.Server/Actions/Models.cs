using Confidoc.Server.Models;

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
    public string? Name { get; set; }
    public IEnumerable<ParsedChange>? Changes { get; set; }
    public string? Data { get; set; }
    public long? Created { get; set; }
    public long? LastModified { get; set; }
    public string? Owner { get; set; }
    public string? Encrypted { get; set; }
    public IEnumerable<string>? ReadAccessUsers { get; set; }
    public IEnumerable<string>? WriteAccessUsers { get; set; }
    public IEnumerable<string>? ReadAccessGroups { get; set; }
    public IEnumerable<string>? WriteAccessGroups { get; set; }
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