namespace webapi
{
    public interface IPublickeySource
    {
        string GetKeyForNode(string nodeId);
    }
}