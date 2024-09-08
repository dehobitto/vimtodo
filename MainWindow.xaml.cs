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
    private const Key MainwindowAdd = Key.I; // Додати таск
    private const Key MainwindowRemove = Key.D; // Прибрати таск
    private const Key MainwindowControlUp = Key.J; // Вгору по таскам
    private const Key MainwindowControlDown = Key.K; // Вниз по таскам
    private const Key MainwindowChangetask = Key.C; // Змінити таск
    private const Key MainwindowMarktask = Key.M; // Помітити таск зробленим

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (sender, e) => 
        {
            LoadTasks(); //Підгружаємо таски якщо вони є збережені
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
        int delIndex = TaskList.SelectedIndex;
        TaskList.Items.Remove(TaskList.SelectedItem);
        SaveTasks();
            
        if (delIndex > 0)
        {
            TaskList.SelectedIndex = delIndex - 1;
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
        if (e.Key == MainwindowChangetask)
        {
            ChangeTask();
            e.Handled = true;
        }
    } 
    void ChangeTask() //Метод що оброблює зміну тасків
    {
        var inputDialog = new InputDialog();
        int i = TaskList.SelectedIndex;
        inputDialog.InputText = (string?)TaskList.Items[i];
        
        if (inputDialog.ShowDialog() == true)
        {
            string? userInput = inputDialog.InputText;
            if (TaskList.SelectedItem != null)
            {
                TaskList.Items[i] = (userInput);
                TaskList.SelectedIndex = i;
            }
            SaveTasks(); 
        }
    }
    
    void MainWindow_MarkTask(object sender, KeyEventArgs e) //
    {
        if (e.Key == MainwindowMarktask)
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
            if (TaskList.Items[i]!.ToString()![0] != '+')
            {
                TaskList.Items[i] = "+ " + TaskList.Items[i];
                TaskList.SelectedIndex = i;
            }
            else
            {
                // Тутт хз, воно працює так, а 
                // TaskList.Items[i] = TaskList.Items[i].ToString().Substring(2, TaskList.Items[i].ToString().Length - 1);
                // Не працює, сам метод видаляє два останніх символи в таску, щоб прибрати помітку (помічені таски відмічаються "+ "), тому потрібно видалити два останніх символи
                TaskList.Items[i] = TaskList.Items[i].ToString().Substring(1, TaskList.Items[i].ToString().Length - 1);
                TaskList.Items[i] = TaskList.Items[i].ToString().Substring(1, TaskList.Items[i].ToString().Length - 1);
                TaskList.SelectedIndex = i;
            }
        }
        SaveTasks();
    }
    
    void ShowInputDialog() //Відкриття вікна з вводом тексту 
    {
        var inputDialog = new InputDialog(); 
        if (inputDialog.ShowDialog() == true)
        {
            string? userInput = inputDialog.InputText;
            TaskList.Items.Add(userInput);
            SaveTasks(); 
        }
    }
    
    void SaveTasks() //Зберігання тасків у джсон
    {
        var tasks = new List<string>();
        foreach (var item in TaskList.Items)
        {
            tasks.Add(item.ToString()!);
        }

        var json = JsonSerializer.Serialize(tasks);
        File.WriteAllText("tasks.json", json);
    }
    void LoadTasks() //Знаходження джсону і підгрузка тесків
    {
        if (File.Exists("tasks.json"))
        {
            var json = File.ReadAllText("tasks.json");
            var tasks = JsonSerializer.Deserialize<List<string>>(json);

            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    TaskList.Items.Add(task);
                }
            }
        }
    }
    void LoadKeys()
    {
        this.PreviewKeyDown += MainWindow_AddTask; //Додаємо клавішу додати таск
        this.PreviewKeyDown += MainWindow_RemoveTask; //Додаємо клавішу прибрати таск
        this.PreviewKeyDown += MainWindow_Controls; //Додаємо клавіші керування виділенням
        this.PreviewKeyDown += MainWindow_ChangeTask; //Додаємо клавішу змінити таск
        this.PreviewKeyDown += MainWindow_MarkTask; //Додаємо клавішу відмітити таск
    }
    
    protected override void OnClosed(EventArgs e) //Зберігання коли закривається програма
    {
        base.OnClosed(e);
        SaveTasks(); 
    }
}