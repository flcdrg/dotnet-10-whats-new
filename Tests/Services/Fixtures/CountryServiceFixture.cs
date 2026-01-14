using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using webapp.Data;
using webapp.Models;

namespace Tests.Services.Fixtures;

public class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionData = new();

    public string Id { get; } = Guid.NewGuid().ToString();
    public bool IsAvailable => true;
    public IEnumerable<string> Keys => _sessionData.Keys;

    public void Clear() => _sessionData.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _sessionData.Remove(key);

    public void Set(string key, byte[] value) => _sessionData[key] = value;

    public bool TryGetValue(string key, out byte[]? value) => _sessionData.TryGetValue(key, out value);
}

public class CountryServiceFixture
{
    public PetstoreContext Context { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public MockSession Session { get; }
    public HttpContext HttpContext { get; }
    public List<Country> TestCountries { get; }

    public CountryServiceFixture()
    {
        var options = new DbContextOptionsBuilder<PetstoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new PetstoreContext(options);

        TestCountries = new List<Country>
        {
            new() { Id = 1, Code = "AU", Name = "Australia" },
            new() { Id = 2, Code = "US", Name = "United States" },
            new() { Id = 3, Code = "GB", Name = "United Kingdom" }
        };

        Context.Countries.AddRange(TestCountries);
        Context.SaveChanges();

        // Setup HTTP context with concrete MockSession
        Session = new MockSession();
        HttpContext = Substitute.For<HttpContext>();
        HttpContext.Session.Returns(Session);
        
        HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        HttpContextAccessor.HttpContext.Returns(HttpContext);
    }

    public void SetSessionValue(string key, string value)
    {
        Session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

