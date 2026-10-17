
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Urls.Add("http://localhost:5100");

var tasks = new List<TaskItem>
{
    new TaskItem
    {
        Id = 1,
        Title = "Изучить HttpClient",
        Description = "Разобраться с отправкой HTTP-запросов",
        IsCompleted = true
    },
    new TaskItem
    {
        Id = 2,
        Title = "Реализовать Web API",
        Description = "Создать API для управления задачами",
        IsCompleted = false
    },
    new TaskItem
    {
        Id = 3,
        Title = "Изучить PATCH",
        Description = "Познакомиться с частичным обновлением данных",
        IsCompleted = false
    }
};

var nextId = 4;

app.MapGet("/api/tasks", () =>
{
    return Results.Ok(tasks);
});

app.MapGet("/api/tasks/{id:int}", (int id) =>
{
    var task = tasks.FirstOrDefault(x => x.Id == id);

    return task is null
        ? Results.NotFound(new { message = "задача не найдена" })
        : Results.Ok(task);
});

app.MapPost("/api/tasks", (TaskItem item) =>
{
    if (string.IsNullOrWhiteSpace(item.Title))
    {
        return Results.BadRequest(new { message = "название задачи обязательно" });
    }

    item.Id = nextId++;
    tasks.Add(item);

    return Results.Created($"/api/tasks/{item.Id}", item);
});

app.MapPut("/api/tasks/{id:int}", (int id, TaskItem item) =>
{
    var task = tasks.FirstOrDefault(x => x.Id == id);

    if (task is null)
    {
        return Results.NotFound(new { message = "задача не найдена" });
    }

    if (string.IsNullOrWhiteSpace(item.Title))
    {
        return Results.BadRequest(new { message = "название задачи обязательно" });
    }

    task.Title = item.Title;
    task.Description = item.Description;
    task.IsCompleted = item.IsCompleted;

    return Results.Ok(task);
});

app.MapDelete("/api/tasks/{id:int}", (int id) =>
{
    var task = tasks.FirstOrDefault(x => x.Id == id);

    if (task is null)
    {
        return Results.NotFound(new { message = "задача не найдена" });
    }

    tasks.Remove(task);

    return Results.NoContent();
});

app.Run();

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsCompleted { get; set; }
}
