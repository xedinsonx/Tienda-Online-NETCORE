using CrudDemoPro.Controllers;
using CrudDemoPro.Data;
using CrudDemoPro.Infrastructure;
using CrudDemoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CrudDemoPro.Tests;

public sealed class UsuariosControllerTests
{
    [Fact]
    public async Task Create_persists_user_and_publishes_created_event()
    {
        await using var context = CreateContext();
        var publisher = new RecordingEventPublisher();
        var controller = new UsuariosController(context, publisher);

        var result = await controller.Create(new Usuario
        {
            Nombre = "Ana",
            Email = "ana@example.com"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var user = Assert.IsType<Usuario>(created.Value);
        Assert.Equal(1, user.Id);
        Assert.Same(user, await context.Usuarios.SingleAsync());
        Assert.Equal(("usuario.created", user), Assert.Single(publisher.Events));
    }

    [Fact]
    public async Task Update_returns_not_found_when_user_does_not_exist()
    {
        await using var context = CreateContext();
        var controller = new UsuariosController(context, new RecordingEventPublisher());

        var result = await controller.Update(99, new Usuario
        {
            Nombre = "Ana",
            Email = "ana@example.com"
        });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_removes_user_and_publishes_deleted_event()
    {
        await using var context = CreateContext();
        var user = new Usuario { Nombre = "Luis", Email = "luis@example.com" };
        context.Usuarios.Add(user);
        await context.SaveChangesAsync();
        var publisher = new RecordingEventPublisher();
        var controller = new UsuariosController(context, publisher);

        var result = await controller.Delete(user.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Usuarios);
        Assert.Equal(("usuario.deleted", user), Assert.Single(publisher.Events));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private sealed class RecordingEventPublisher : IKafkaEventPublisher
    {
        public List<(string EventType, Usuario Usuario)> Events { get; } = [];

        public Task PublishAsync(
            string eventType,
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            Events.Add((eventType, usuario));
            return Task.CompletedTask;
        }
    }
}
