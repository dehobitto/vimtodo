using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Windows.Input;

namespace TaskManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public partial class MainWindow
{
    //Бінди
    private const Key MainwindowAdd = Key.I; // Додати таск
    private const Key MainwindowRemove = Key.D; // Прибрати таск
    private const Key MainwindowControlUp = Key.J; // Вгору по таскам
    private const Key MainwindowControlDown = Key.K; // Вниз по таскам
    private const Key MainwindowChangetask = Key.C; // Змінити таск
    private const Key MainwindowMarktask = Key.M; // Помітити таск зробленим
    private const Key MainwindowAddDescription = Key.E; // Помітити таск зробленим
    
    List<Task>? tasks = new List<Task>();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (sender, e) => 
        {
            UpdateTasks(); //Підгружаємо таски якщо вони є збережені
            this.Activate(); //Фокус на створене вікно
            this.Focus(); //Фокус на створене вікно
            
            if (TaskList != null) //Якщо є таски, то виділяємо перший
            {
                TaskList.SelectedIndex = 0;
            }
        };
        LoadKeys();
    }

    void MainWindow_AddTask(object sender, KeyEventArgs e) //Метод що оброблює натискання клавіші
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
    
    void MainWindow_RemoveTask(object sender, KeyEventArgs e) //Метод що оброблює натискання клавіші та видалення тасків
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
    
    void MainWindow_Controls(object sender, KeyEventArgs e) //Метод що оброблює натискання клавіші
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
    
    void MainWindow_ChangeTask(object sender, KeyEventArgs e) //Метод що оброблює натискання клавіші
    {
        if (e.Key == MainwindowChangetask && TaskList.SelectedIndex >= 0)
        {
            ChangeTask();
            e.Handled = true;
        }
    } 
    void ChangeTask() //Метод що оброблює зміну тасків
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
    
    void ShowInputDialog() //Відкриття вікна з вводом тексту 
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
    
    void SaveTasks() //Зберігання тасків у джсон
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
    
    private void ShowDescriptionWindow(string text)
    {
        // Create an instance of the secondary window and pass the text
        var descriptionWindow = new DescriptionWindow(text);

        // Get the position of the main window
        var mainWindowLeft = this.Left;
        var mainWindowTop = this.Top;

        // Set the size and position of the secondary window
        descriptionWindow.Width = 480;
        descriptionWindow.Height = 480;
        descriptionWindow.Left = mainWindowLeft + this.ActualWidth; // Position to the right of the main window
        descriptionWindow.Top = mainWindowTop;

        // Show the secondary window
        descriptionWindow.Show();
    }
    
    protected override void OnClosed(EventArgs e) //Зберігання коли закривається програма
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
            date += DateTime.Today.Date.ToString("dd/MM/yy");
        }
        Title = title;
        IsCompleted = isCompleted;
        Date = date;
        Description = description;
    }
    
    public override string ToString()
    {
        return Title + " - " + Date + " - " + IsCompleted;
    }
}