using Confidoc.Server.Models;
using ConfidocLib;

namespace Confidoc.Server;

public partial class Actions
{
    /// <summary>
    /// Decrypts string content with 
    /// the supplied password
    /// </summary>
    /// <param name="text"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public string? Decrypt(string? text, string? password)
    {
        if (text is null || password is null) return text;
        var decrypted = Security.Decrypt(text, password);
        return decrypted;
    }


    /// <summary>
    /// Encrypts the document change patch
    /// string with AES256 if a password is
    /// set.
    /// </summary>
    /// <param name="patch"></param>
    /// <param name="reference"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public string? Encrypt(string patch, string? reference, string? password)
    {
        if (password is null) return patch;
        if (reference is null) return patch;
        Security.Decrypt(reference, password);
        return Security.Encrypt(patch, password);
    }

    /// <summary>
    /// Digitally sign whatever data is provided
    /// with teh user's private key so his identity
    /// can be proven.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string SignData(string data, ConfidocUser user)
    {
        var pk = user.PrivateKey;
        if (pk is null) throw new Exception("user does not have a private key");
        return Security.Sign(data, pk);
    }
}