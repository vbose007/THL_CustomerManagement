namespace CustomerManagement.API.Helpers
{
    public interface IHttpContextHelper
    {
        string Host { get; }
        string Path { get; }
        int Port { get; }
        string Scheme { get; }
        string AbsoluteUri { get; }
        string SchemeDelimiter { get; }
        string CurrentUserName { get; }
        bool IsCurrentUserRole(string role);
    }
}