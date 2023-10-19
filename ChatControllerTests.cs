using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SimpleChatApi.Data;
using SimpleChatApi.DTOs;
using SimpleChatApi.Models;
using System.Net;
using Xunit;

namespace SimpleChatApi.InregrationTests
{


  [TestFixture]
  public class ChatControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
  {
    private readonly List<User> users = new() {
        new User { Id = 1, Name = "John" },
        new User { Id = 2, Name = "Elon" },
        new User { Id = 3, Name = "Kate" },
        new User { Id = 4, Name = "Dan" },
        new User { Id = 5, Name = "Chuck" },
        new User { Id = 6, Name = "Julia" },
        new User { Id = 7, Name = "Juliet" },
      };

    private readonly List<Chat> chats = new() {
        new Chat { Id = 1, Name = "NewChat_1", CreatorChatId = 1 },
        new Chat { Id = 2, Name = "NewChat_2", CreatorChatId = 2 },
        new Chat { Id = 3, Name = "NewChat_3", CreatorChatId = 3 }
      };

    [Test]
    public async Task CreateChat_ChatCreated_ReturnsOk()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Users.AddRangeAsync(users);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();
      var createChatDTO = new CreateChatDto { UserId = 1, ChatName = "MyNewChat" };
      var json = JsonConvert.SerializeObject(createChatDTO);

      StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

      // Act

      HttpResponseMessage message = await httpClient.PostAsync("/api/Chat/CreateChat", content);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateChat_UserNotFound_ReturnsNotFound()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Users.AddRangeAsync(users);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      var createChatDTO = new CreateChatDto { UserId = 23, ChatName = "ChatWithRandomName" };
      var json = JsonConvert.SerializeObject(createChatDTO);

      StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

      // Act

      HttpResponseMessage message = await httpClient.PostAsync("/api/Chat/CreateChat", content);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateChat_ChatAlreadyExist_ReturnsBadRequest()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Users.AddRangeAsync(users);
      await dbContext.Chats.AddRangeAsync(chats);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      var createChatDTO = new CreateChatDto { UserId = 1, ChatName = "NewChat_1" };
      var json = JsonConvert.SerializeObject(createChatDTO);

      StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

      // Act

      HttpResponseMessage message = await httpClient.PostAsync("/api/Chat/CreateChat", content);


      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DeleteChat_ChaDeleted_ReturnsNoContent()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Users.AddRangeAsync(users);
      await dbContext.Chats.AddRangeAsync(chats);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      var deleteChatDTO = new DeleteChatDto { UserId = 1, ChatId = 1 };
      var json = JsonConvert.SerializeObject(deleteChatDTO);
      StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

      // Act

      HttpResponseMessage message = await httpClient.PostAsync("/api/Chat/DeleteChat", content);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteChat_ChatNotFound_ReturnsNotFound()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Users.AddRangeAsync(users);
      await dbContext.Chats.AddRangeAsync(chats);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      var deleteChatDTO = new DeleteChatDto { UserId = 1, ChatId = 9999 };
      var json = JsonConvert.SerializeObject(deleteChatDTO);
      StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

      // Act

      HttpResponseMessage message = await httpClient.PostAsync("/api/Chat/DeleteChat", content);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteChat_UserNoHavePermission_ReturnsForbidden()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Users.AddRangeAsync(users);
      await dbContext.Chats.AddRangeAsync(chats);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      var deleteChatDto = new DeleteChatDto { UserId = 2, ChatId = 1 };
      var json = JsonConvert.SerializeObject(deleteChatDto);
      StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

      // Act

      HttpResponseMessage message = await httpClient.PostAsync("/api/Chat/DeleteChat", content);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetChat_ValidChatName_ReturnsChat()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Chats.AddRangeAsync(chats);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      string chatName = "NewChat_1";
      // Act

      HttpResponseMessage message = await httpClient.GetAsync(requestUri: "/api/Chat/GetChat/" + chatName);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.OK));
    }

    [Test]
    public async Task GetChat_ChatNotFound_ReturnsNotFound()
    {
      // Arrange

      CustomWebApplicationFactory<Program> webHost = new CustomWebApplicationFactory<Program>();

      ChatContext dbContext =
        webHost.Services.CreateScope().ServiceProvider.GetService<ChatContext>()!;

      await dbContext.Database.EnsureDeletedAsync();
      await dbContext.Chats.AddRangeAsync(chats);
      await dbContext.SaveChangesAsync();

      HttpClient httpClient = webHost.CreateClient();

      string chatName = "NotFoundChat";
      // Act

      HttpResponseMessage message = await httpClient.GetAsync(requestUri: "/api/Chat/GetChat/" + chatName);

      // Assert

      Assert.That(message.StatusCode, Is.EqualTo(expected: HttpStatusCode.NotFound));
    }
  }
}
