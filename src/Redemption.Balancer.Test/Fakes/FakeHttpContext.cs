using Microsoft.AspNetCore.Http;
using Redemption.Balancer.Api.Constants;

namespace Redemption.Balancer.Test.Fakes;

public static class FakeHttpContext
{
    public static DefaultHttpContext CreateFakeHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[Application.Headers.XUserId] = "1";

        return httpContext;
    }
}