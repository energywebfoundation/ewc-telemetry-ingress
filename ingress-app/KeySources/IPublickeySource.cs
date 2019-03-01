namespace webapi
{
    public interface IPublickeySource
    {
        string GetKeyForNode(string nodeId);
        void AddKey(string nodeId, string pubkeyAsBase64);
        void RemoveKey(string nodeId);
    }
}