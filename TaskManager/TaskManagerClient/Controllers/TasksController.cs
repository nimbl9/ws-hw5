
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TaskManagerClient.Models;

namespace TaskManagerClient.Controllers;

public class TasksController : Controller
{
    private readonly IHttpClientFactory _factory;

    public TasksController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient Api()
    {
        return _factory.CreateClient("TaskApi");
    }

    private void ShowError(HttpResponseMessage response)
    {
        var message = response.StatusCode switch
        {
            HttpStatusCode.BadRequest => "Проверьте введённые данные.",
            HttpStatusCode.NotFound => "Задача не найдена.",
            HttpStatusCode.InternalServerError => "На сервере произошла ошибка.",
            _ => $"Ошибка сервера: {(int)response.StatusCode}."
        };

        TempData["Error"] = message;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await Api().GetAsync("api/tasks");

            if (!response.IsSuccessStatusCode)
            {
                ShowError(response);
                return View(new List<TaskItem>());
            }

            var items = await response.Content.ReadFromJsonAsync<List<TaskItem>>();

            return View(items ?? new List<TaskItem>());
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API. Проверьте, запущен ли сервер.";
            return View(new List<TaskItem>());
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return View(new List<TaskItem>());
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await Api().GetAsync($"api/tasks/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ShowError(response);
                return RedirectToAction(nameof(Index));
            }

            var item = await response.Content.ReadFromJsonAsync<TaskItem>();

            if (item == null)
            {
                TempData["Error"] = "Сервер не вернул данные задачи.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API.";
            return RedirectToAction(nameof(Index));
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TaskItem());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItem item)
    {
        if (!ModelState.IsValid)
            return View(item);

        try
        {
            var response = await Api().PostAsJsonAsync("api/tasks", item);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                TempData["Success"] = "Задача успешно добавлена.";
                return RedirectToAction(nameof(Index));
            }

            ShowError(response);
            return View(item);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API.";
            return View(item);
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return View(item);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await Api().GetAsync($"api/tasks/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ShowError(response);
                return RedirectToAction(nameof(Index));
            }

            var item = await response.Content.ReadFromJsonAsync<TaskItem>();

            if (item == null)
            {
                TempData["Error"] = "Не удалось получить данные задачи.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API.";
            return RedirectToAction(nameof(Index));
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItem item)
    {
        if (id != item.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(item);

        try
        {
            var response = await Api().PutAsJsonAsync($"api/tasks/{id}", item);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                TempData["Success"] = "Изменения сохранены.";
                return RedirectToAction(nameof(Index));
            }

            ShowError(response);
            return View(item);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API.";
            return View(item);
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return View(item);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await Api().GetAsync($"api/tasks/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ShowError(response);
                return RedirectToAction(nameof(Index));
            }

            var item = await response.Content.ReadFromJsonAsync<TaskItem>();

            if (item == null)
            {
                TempData["Error"] = "Не удалось получить данные задачи.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API.";
            return RedirectToAction(nameof(Index));
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await Api().DeleteAsync($"api/tasks/{id}");

            if (response.StatusCode == HttpStatusCode.NoContent ||
                response.StatusCode == HttpStatusCode.OK)
            {
                TempData["Success"] = "Задача удалена.";
                return RedirectToAction(nameof(Index));
            }

            ShowError(response);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["Error"] = "Не удалось подключиться к Web API.";
            return RedirectToAction(nameof(Index));
        }
        catch (TaskCanceledException)
        {
            TempData["Error"] = "Время ожидания ответа сервера истекло.";
            return RedirectToAction(nameof(Index));
        }
    }
}
