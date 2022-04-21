namespace ResponseWrapper.Client.Services
{
    public interface ITokenDataProvider
    {
        string GetToken();
        string GetUserToken();
    }
}
