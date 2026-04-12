using DiffMatchPatch;
using System.Text.Json;
using Newtonsoft.Json;
namespace ConfidocLib
{
    public static class Difference
    {
        /// <summary>
        /// Generates a JSON string of changes made to the original text 
        /// that make it equal to the new text. This can then be used
        /// to recreate the new text essentially creating a chain of 
        /// changes.
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <returns></returns>
        public static string GetPatches(string oldText, string newText) {
            diff_match_patch dmp = new();
            var patches = dmp.patch_make(oldText, newText);
            var json = JsonConvert.SerializeObject(patches);
            return json;
        }

        /// <summary>
        /// Takes in patches made by <see cref="GetPatches(string, string)"/> and
        /// applies them to a text.
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static string Patch(string oldText, string changes)
        {
            diff_match_patch dmp = new();
            var patches = JsonConvert.DeserializeObject<List<Patch>>(changes);
            var newText = (string)dmp.patch_apply(patches, oldText)[0];
            return newText;
        }
    }
}
