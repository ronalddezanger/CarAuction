using MassTransit;
using NotificationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit( x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        // username and pasword to login to rabbitmq
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username", "rabbitmq"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password", "rabbitmq"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();
