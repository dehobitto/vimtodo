using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace TaskManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public partial class MainWindow
{
    //Бінди
    private const Key MainwindowAdd = Key.I;
    private const Key MainwindowRemove = Key.D;
    private const Key MainwindowControlUp = Key.J;
    private const Key MainwindowControlDown = Key.K; 
    private const Key MainwindowChangetask = Key.C;
    private const Key MainwindowMarktask = Key.M;
    private const Key MainwindowAddDescription = Key.E; 
    
    List<Task>? tasks = new List<Task>();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (sender, e) => 
        {
            UpdateTasks(); 
            this.Activate(); 
            this.Focus(); 
            
            if (TaskList != null) 
            {
                TaskList.SelectedIndex = 0;
            }
        };
        LoadKeys();
    }

    void MainWindow_AddTask(object sender, KeyEventArgs e) 
    {
        if (e.Key == MainwindowAdd) 
        {
            AddTask();
            e.Handled = true;
        }
    }
    void AddTask()
    {
        if (!Keyboard.FocusedElement.Equals(TaskList))
        {
            ShowInputDialog();
        }
    }
    
    void MainWindow_RemoveTask(object sender, KeyEventArgs e) 
    {
        if (e.Key == MainwindowRemove && TaskList.SelectedItem != null)
        {
            RemoveTask();
            e.Handled = true;
        }
    }
    void RemoveTask()
    {
        int i = TaskList.SelectedIndex;
        tasks.RemoveAt(i);
        SaveTasks();
        UpdateTasks(i);
            
        if (i > 0)
        {
            TaskList.SelectedIndex = i - 1;
        }
        else
        {
            TaskList.SelectedIndex = 0;
        }
    }
    
    void MainWindow_Controls(object sender, KeyEventArgs e) 
    {
        if (e.Key == MainwindowControlDown)
        {
            if (TaskList.SelectedIndex < TaskList.Items.Count - 1)
            {
                TaskList.SelectedIndex += 1;
            }
        }
        
        if (e.Key == MainwindowControlUp)
        {
            if (TaskList.SelectedIndex > 0)
            {
                TaskList.SelectedIndex -= 1;
            }
        }
    }
    
    void MainWindow_ChangeTask(object sender, KeyEventArgs e)
    {
        if (e.Key == MainwindowChangetask && TaskList.SelectedIndex >= 0)
        {
            ChangeTask();
            e.Handled = true;
        }
    } 
    void ChangeTask()
    {
        var inputDialog = new InputDialog();
        int i = TaskList.SelectedIndex;
        inputDialog.InputText = tasks[i].Title;
        
        if (inputDialog.ShowDialog() == true)
        {
            string? userInput = inputDialog.InputText;
            if (TaskList.SelectedItem != null)
            {
                tasks[i].Title = (userInput);
            }
            SaveTasks();
            UpdateTasks(i);
        }
    }
    
    void MainWindow_MarkTask(object sender, KeyEventArgs e) //
    {
        if (e.Key == MainwindowMarktask && TaskList.SelectedIndex >= 0)
        {
            MarkTask();
            e.Handled = true;
        }
    }
    void MarkTask()
    {
        int i = TaskList.SelectedIndex;
        if (TaskList.SelectedItem != null)
        {
            Task task = (Task)TaskList.Items[i];
            task.IsCompleted = !task.IsCompleted;
            TaskList.Items[i] = task;
        }
        SaveTasks();
        UpdateTasks(i);
    }

    void MainWindow_AddDesription(object sender, KeyEventArgs e)
    {
        if (e.Key == MainwindowAddDescription && TaskList.SelectedIndex >= 0)
        {
            AddDescription();
            e.Handled = true;
        }
    }

    void AddDescription()
    {
        int i = TaskList.SelectedIndex;
        var descriptionWindow = new DescriptionWindow(tasks[i].Description);

        // Get the position of the main window
        var mainWindowLeft = this.Left;
        var mainWindowTop = this.Top;

        // Set the size and position of the secondary window
        descriptionWindow.Width = 480;
        descriptionWindow.Height = 480;
        descriptionWindow.Left = mainWindowLeft + this.ActualWidth + 20; // Position to the right of the main windows
        descriptionWindow.Top = mainWindowTop;
        
        if (descriptionWindow.ShowDialog() == true)
        {
            string? userInput = descriptionWindow.InputText;
            if (TaskList.SelectedItem != null)
            {
                tasks[i].Description = (userInput);
            }
            SaveTasks();
            UpdateTasks(i);
        }
    }
    
    void ShowInputDialog() 
    {
        var inputDialog = new InputDialog(); 
        if (inputDialog.ShowDialog() == true)
        {
            string? userInput = inputDialog.InputText;
            Task newTask = new Task(userInput);
            tasks.Add(newTask);
            TaskList.Items.Add(newTask);
            SaveTasks(); 
        }
    }
    
    void SaveTasks()
    {
        string json = JsonSerializer.Serialize(tasks);
        File.WriteAllText("tasks.json", json);
    }

    void UpdateTasks(int i = 0)
    {
        if (File.Exists("tasks.json"))
        {
            var json = File.ReadAllText("tasks.json");
            tasks = JsonSerializer.Deserialize<List<Task>>(json);
            TaskList.Items.Clear();
            
            foreach (Task task in tasks)
            {
                TaskList.Items.Add(task);
            }
            TaskList.SelectedIndex = i;
        }
    }
    void LoadKeys()
    {
        this.PreviewKeyDown += MainWindow_AddTask; //Додаємо клавішу додати таск
        this.PreviewKeyDown += MainWindow_RemoveTask; //Додаємо клавішу прибрати таск
        this.PreviewKeyDown += MainWindow_Controls; //Додаємо клавіші керування виділенням
        this.PreviewKeyDown += MainWindow_ChangeTask; //Додаємо клавішу змінити таск
        this.PreviewKeyDown += MainWindow_MarkTask; //Додаємо клавішу відмітити таск
        this.PreviewKeyDown += MainWindow_AddDesription; //Додаємо клавішу відмітити таск
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        SaveTasks(); 
    }
}

public class Task
{
    public string Title { get; set; }
    public string Date { get; set; }
    public bool IsCompleted { get; set; }
    public string Description { get; set; }

    public Task(string title, bool isCompleted = false, string date = "", string description = "")
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