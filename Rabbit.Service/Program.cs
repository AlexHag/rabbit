using Rabbit.Service.Configuration;
using Rabbit.Domain.Producers;
using Rabbit.Domain.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<RabbitMQOptionsSetup>();
builder.Services.AddScoped(typeof(EventProducer<>));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGroup("produce")
    .MapProducerEndpoints()
    .WithTags("Produce");

app.Run();
