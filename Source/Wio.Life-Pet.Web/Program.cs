using Wio.Life_Pet.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddApplicationServices();
builder.Services.ModeInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", opt =>
    {
        opt.WithOrigins("*", "")
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(url =>
    {
        url.SwaggerEndpoint("/swagger/v1/swagger.json", "Wio.Life-Pet API V1");
    });
    app.MapOpenApi();
}

app.UseCors(
    policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("*"));

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
