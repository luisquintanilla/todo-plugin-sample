using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var id = 0;
var toDos = new Todo[] { };

app.MapGet("/todos", 
    async () =>
    {
        return await Task.FromResult(toDos);
    })
    .WithName("GetTodos")
    .WithOpenApi(o => { o.Description = "Get list of todos"; return o; });

app.MapGet("/todos/{id:int}",
    async (int id) => 
        { 
            return await Task.FromResult(toDos.FirstOrDefault(t => t.Id == id));
        })
    .WithName("GetTodoById")
    .WithOpenApi(o => 
        { 
            o.Description = "Gets a list of todos by Id";
            o.Parameters[0].In = ParameterLocation.Path;
            o.Parameters[0].Name = "id";
            o.Parameters[0].Description = "Id of todo item";
            o.Parameters[0].Schema = new OpenApiSchema { Type = "integer", Format = "int32" };
            return o; 
        });

app.MapPost("/todos",
    (Todo todo) =>
    {
        var todoWithId = todo with { Id = id };
        id = id + 1;
        toDos = toDos.Append(todoWithId).ToArray();
        return Task.CompletedTask;
    })
    .WithName("AddTodo")
    .Accepts<Todo>("application/json")
    .WithOpenApi(o => { o.Description = "Add todo item to list"; return o; });

app.Run();

internal record Todo(int Id, string Title, bool Completed);