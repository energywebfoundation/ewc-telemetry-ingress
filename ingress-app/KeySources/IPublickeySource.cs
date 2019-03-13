using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// Interface for defining Public key Source
    /// </summary>
    public interface IPublickeySource
    {
        /// <summary>
        /// This funciton gets key for provided node Id from key source.
        /// </summary>
        /// <param name="nodeId">The node Id for which key is required.</param>
        /// <returns>returns Public key from key source.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when key does not exist in key source.</exception>
        string GetKeyForNode(string nodeId);

        /// <summary>
        /// This funciton adds key and node Id to key source.
        /// </summary>
        /// <param name="nodeId">The node Id to be registered.</param>
        /// <param name="pubkeyAsBase64">The Base64 encoded public key to be registered.</param>
        void AddKey(string nodeId, string pubkeyAsBase64);

        /// <summary>
        /// This funciton removes key for provided node Id from key source.
        /// </summary>
        /// <param name="nodeId">The node Id for which key removal is performed.</param>
        /// <exception cref="KeyNotFoundException">Thrown when key does not exist in key source.</exception>
        void RemoveKey(string nodeId);
    }
}