using JWTAuth.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomMiddlewares();
app.MapControllers();
app.Run();
