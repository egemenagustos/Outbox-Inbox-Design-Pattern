using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Models.Contexts;
using Order.Api.Models.Entities;
using Order.Api.ViewModels;
using Shared.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["Mssql"]);
});

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, configure) =>
    {
        configure.Host(builder.Configuration["RabbitMq"]);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrder model, OrderDbContext orderDbContext, ISendEndpointProvider sendEndpointProvider) =>
{
    Guid idempotentToken = Guid.NewGuid();

    Order.Api.Models.Entities.Order order = new()
    {
        BuyerId = model.BuyerId,
        CreatedDate = DateTime.UtcNow,
        TotalPrice = model.OrderItems.Sum(oi => oi.Count * oi.Price),
        OrderItems = model.OrderItems.Select(oi => new Order.Api.Models.Entities.OrderItem
        {
            Price = oi.Price,
            Count = oi.Count,
            ProductId = oi.ProductId,
        }).ToList(),
    };

    await orderDbContext.Orders.AddAsync(order);

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = model.OrderItems.Sum(oi => oi.Count * oi.Price),
        OrderItems = model.OrderItems.Select(oi => new Shared.Datas.OrderItem
        {
            Price = oi.Price,
            Count = oi.Count,
            ProductId = oi.ProductId
        }).ToList(),
        IdempotentToken = idempotentToken,
    };

    OrderOutbox orderOutbox = new()
    {
        OccuredOn = DateTime.UtcNow,
        ProcessedDate = null,
        Payload = JsonSerializer.Serialize(orderCreatedEvent),
        Type = nameof(OrderCreatedEvent),
        IdempotentToken = idempotentToken
    };

    await orderDbContext.OrderOutboxes.AddAsync(orderOutbox);
    await orderDbContext.SaveChangesAsync();

    //var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEvent}"));
    //await sendEndpoint.Send<OrderCreatedEvent>(orderCreatedEvent);
});

app.Run();
