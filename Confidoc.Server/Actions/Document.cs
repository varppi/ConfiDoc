using Confidoc.Server.Models;
using ConfidocLib;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Confidoc.Server;


public partial class Actions
{
    /// <summary>
    /// Retrieves all documents OWNED by the user
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public List<ParsedDocument> GetDocuments(ClaimsPrincipal claim)
    {
        var documents = GetDocumentsByOwner(claim)
            .ToList();
        return ToParsedDocuments(documents);
    }


    /// <summary>
    /// Gets a document by ID and verifies that the 
    /// user has access to it.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public ParsedDocument? GetDocument(
        string id, 
        ConfidocUser? user, 
        string? password = null, 
        int? downLevel = null,
        bool download = true)
    {
        if (user is null) return null;
        var document = GetDocumentByID(id);
        if (document is null) return null;
        var accessLevel = DocumentAccessLevel(user, document);
        if (downLevel is not null) accessLevel = accessLevel < downLevel ? accessLevel : downLevel ?? 0;
        if (accessLevel < 1) return null;
        if (document.Encrypted is not null)
            try
            {
                Security.Decrypt(document.Encrypted, password ?? "");
            }
            catch
            {
                throw new CryptographicException("password is invalid");
            }

        var parsedDocument = ToParsedDocuments([document], password)[0];
        parsedDocument.Level = accessLevel;
        if (accessLevel == 1) return ToPdfDocument(parsedDocument, user.UserName??"", download);
        
        return parsedDocument;
    }

    /// <summary>
    /// Gets a document by ID and verifies that the 
    /// user has access to it.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public ParsedDocument? GetDocument(string id, 
        ClaimsPrincipal claim, 
        string? password=null, 
        int? downLevel=null,
        bool download=true)
        => GetDocument(id, GetUser(claim), password, downLevel, download);

    /// <summary>
    /// Creates a new document 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public string CreateDocument(Document document)
    {
        if (document.Owner is null)
            throw new ArgumentNullException("owner must not be null");
        document.Id = Guid.NewGuid().ToString();
        document.Created = DateTime.Now;
        document.LastModified = DateTime.Now;
        document.ReadAccessUsers = [];
        document.WriteAccessUsers = [];
        document.ReadAccessGroups = [];
        document.WriteAccessGroups = [];
        document.Changes = [];
        _context.Documents.Add(document);
        _context.SaveChanges();
        return document.Id;
    }

    /// <summary>
    /// Saves the document
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newData"></param>
    /// <param name="claim"></param>
    /// <returns>true=success, false=failed</returns>
    public bool SaveDocument(string id, string newData, ClaimsPrincipal claim, string? password=null)
    {
        var document = GetDocumentByID(id);
        if (document is null) return false;
        if (DocumentAccessLevel(claim, document) < 2) return false;

        var user = GetUser(claim);
        if (user is null) return false;

        var original = ReconstructDocument(id, password);
        newData = Convert.ToBase64String(Encoding.Latin1.GetBytes(newData));
        var patch = Encrypt(Difference.GetPatches(original.Data!, newData), 
                            document.Encrypted, 
                            password);
        var change = new Change
        {
            Id        = Guid.NewGuid().ToString(),
            Patch     = patch,
            Signature = SignData(patch??"", user),
            Owner     = user,
            Timestamp = DateTime.Now,
        };
        document.LastModified = DateTime.Now;
        document.Changes!.Add(change);
        _context.SaveChanges();
        return true;
    }
    
    /// <summary>
    /// Deletes a document given the user trying to execute
    /// the action is the owner.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    public bool DeleteDocument(string id, ClaimsPrincipal claim)
    {
        var document = GetDocumentByID(id);
        if (document is null) return false;

        if (DocumentAccessLevel(claim, document) < 3)
            return false;

        _context.Documents.Remove(document);
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Walks back through all the changes done to the document
    /// and reconstructs the newest version.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="claim"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public ReconstructedDocument ReconstructDocument(string id, string? password=null)
    {
        var document = GetDocumentByID(id);
        if (document is null || document.Changes is null) throw new Exception("document does not exist or is invalid");
        var user = document.Owner;

        List<Change> changes = document.Changes
            .OrderBy(x => x.Timestamp)
            .ToList();

        ReconstructedDocument reconDoc = new()
        {
            Changes = new List<ReconstructedChange>(),
            Data = "",
        };

        string final = "";
        foreach(var change in changes)
        {
            var changeUser = GetUserByID(change.OwnerId!);
            bool isValidSignature = Security.ValidSignature(
                change.Patch??"", 
                change.Signature??"", 
                changeUser!.PublicKey??""
            );
            try
            {
                reconDoc.Changes.Add(new ReconstructedChange
                {
                    Change = change,
                    IsValidSignature = isValidSignature
                });
                final = Difference.Patch(final,  Decrypt(change.Patch!, password!)!);
            }
            catch
            {
                return reconDoc;
            }
        }
        reconDoc.Data = final;
        return reconDoc;
    }

    /// <summary>
    /// Retrieves a document that has a certain ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Document? GetDocumentByID(string id)
    {
        return _context.Documents
            .Include(document => document.Owner)
            .Include(document => document.Changes)
            .Include(document => document.ReadAccessUsers)
            .Include(document => document.WriteAccessUsers)
            .Include(document => document.ReadAccessGroups)
            .Include(document => document.WriteAccessGroups)
            .FirstOrDefault(doc => doc.Id == id);
    }

    /// <summary>
    /// Retrieves a document that has a certain owner.
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public Document[] GetDocumentsByOwner(ClaimsPrincipal claim)
    {
        var user = GetUser(claim);
        return _context.Documents
            .Include(document => document.Owner)
            .Include(document => document.Changes)
            .Include(document => document.ReadAccessUsers)
            .Include(document => document.WriteAccessUsers)
            .Include(document => document.ReadAccessGroups)
            .Include(document => document.WriteAccessGroups)
            .ToList()
            .Where(document => DocumentAccessLevel(user, document) >= 1)
            .ToArray();
    }
}