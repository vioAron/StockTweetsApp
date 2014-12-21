using LinqToTwitter;

namespace StockTweetsApp.Repository
{
    public class SharedState
    {
        public static IAuthorizer Authorizer { get; set; }
    }
}