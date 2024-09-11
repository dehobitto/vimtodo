namespace TaskManager.Classes;

public class Vimtodo
{
    public string Title { get; set; }
    public string Date { get; }
    public bool IsCompleted { get; set; }
    public string? Description { get; set; }
    
    public Vimtodo(string title, bool isCompleted = false, string date = "", string? description = "")
    {
        if (date == "")
        {
            date += DateTime.Now.ToString("HH/mm ").Replace(".", ":");
            date += DateTime.Now.ToString("dd/MM/yy").Replace(".", "/");
        }
        Title = title;
        IsCompleted = isCompleted;
        Date = date;
        Description = description;
    }
    
    public override string ToString()
    {
        return IsCompleted ? Title + " - DONE - " + Date  : Title + " - TODO - " + Date;
    }
}