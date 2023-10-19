using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleChatApi.Data;
using SimpleChatApi.Models;

namespace SimpleChatApi.InregrationTests
{
  public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
      where TProgram : class
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureTestServices(services =>
      {
        var dbContextDescriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                typeof(DbContextOptions<ChatContext>));

        services.Remove(dbContextDescriptor!);
        services.AddDbContext<ChatContext>(options =>
          options.UseInMemoryDatabase("Test.db"));        
      });
    }
  }
}