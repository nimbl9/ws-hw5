
using System.ComponentModel.DataAnnotations;

namespace TaskManagerClient.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название задачи")]
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public bool IsCompleted { get; set; }
}
