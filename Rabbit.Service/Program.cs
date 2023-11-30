using Rabbit.Service.Configuration;
using Rabbit.Service.Producer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<ProducerOptionsSetup>();
builder.Services.AddScoped(typeof(EventProducer<>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("produce")
    .MapProducerEndpoints()
    .WithTags("Produce");

app.Run();
