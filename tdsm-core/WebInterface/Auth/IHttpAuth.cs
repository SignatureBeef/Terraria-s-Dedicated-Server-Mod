
namespace tdsm.core.WebInterface.Auth
{
    public interface IHttpAuth
    {
        bool CreateLogin(string username, string password, params string[] options);
        string Authenticate(WebRequest request);
    }
}
