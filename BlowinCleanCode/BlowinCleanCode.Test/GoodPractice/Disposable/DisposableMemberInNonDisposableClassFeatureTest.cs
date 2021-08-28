using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.GoodPractice.Disposable
{
    public class DisposableMemberInNonDisposableClassFeatureTest
    {
        [Theory]
        [InlineData(@"
using System;
using System.Net.Http;
using System.Threading.Tasks;

public sealed class Store : IDisposable
{
    private readonly HttpClient _client;

    public Store(HttpClient client)
    {
        _client = client;
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}")]
        [InlineData(@"
using System;
using System.Net.Http;
using System.Threading.Tasks;

public sealed class Store : IAsyncDisposable
{
    private readonly HttpClient _client;

    public Store(HttpClient client)
    {
        _client = client;
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return new ValueTask();
    }
}")]
        [InlineData(@"
using System;
using System.Net.Http;
using System.Threading.Tasks;

public sealed class Store
{
    private readonly HttpClient _client;

    public Store(HttpClient client)
    {
        _client = client;
    }
}")]
        [InlineData(@"
using System;
using System.Net.Http;
using System.Threading.Tasks;

public sealed class Store
{
    private HttpClient Client { get; }

    public Store(HttpClient client)
    {
        Client = client;
    }
}")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
        
        [Theory]
        [InlineData(@"
using System;
using System.Net.Http;
using System.Threading.Tasks;

public sealed class {|#0:Store|}
{
    private readonly HttpClient _client;

    public Store()
    {
        _client = new HttpClient();
    }
}", "_client")]
        [InlineData(@"
using System;
using System.Net.Http;
using System.Threading.Tasks;

public sealed class {|#0:Store|}
{
    private HttpClient Client { get; }

    public Store()
    {
        Client = new HttpClient();
    }
}", "Client")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.DisposableMemberInNonDisposable).WithLocation(0).WithArguments(argument);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}