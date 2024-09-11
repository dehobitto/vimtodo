using System.IO;
using System.Text.Json;
using System.Windows.Input;
using TaskManager.Classes;

namespace TaskManager.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public partial class MainWindow
{
    //Бінди
    private const Key MainwindowAdd = Key.I;
    private const Key MainwindowRemove = Key.F4;
    private const Key MainwindowControlUp = Key.J;
    private const Key MainwindowControlDown = Key.K; 
    private const Key MainwindowChangetask = Key.C;
    private const Key MainwindowMarktask = Key.M;
    private const Key MainwindowAddDescription = Key.D; 
    
    List<Vimtodo>? _tasks = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => 
        {
            UpdateTasks(); 
            this.Activate(); 
            this.Focus();
        };
        LoadKeys();
    }

    void MainWindow_AddTask(object sender, KeyEventArgs e) 
    {
        if (e.Key == MainwindowAdd && !Keyboard.FocusedElement.Equals(TaskList)) 
        {
            ShowInputDialog();
            e.Handled = true;
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
        _tasks?.RemoveAt(i);
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
        var inputDialog = new InputWindow();
        int i = TaskList.SelectedIndex;
        inputDialog.InputText = _tasks?[i].Title;
        
        if (inputDialog.ShowDialog() == true)
        {
            string? userInput = inputDialog.InputText;
            if (TaskList.SelectedItem != null)
            {
                _tasks![i].Title = (userInput!);
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
            Vimtodo? vimtodo = (Vimtodo?)TaskList.Items[i];
            vimtodo!.IsCompleted = !vimtodo.IsCompleted;
            TaskList.Items[i] = vimtodo;
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
        var descriptionWindow = new DescriptionWindow(_tasks?[i].Description!);

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
                _tasks![i].Description = (userInput);
            }
            SaveTasks();
            UpdateTasks(i);
        }
    }
    
    void ShowInputDialog() 
    {
        var inputDialog = new InputWindow(); 
        if (inputDialog.ShowDialog() == true)
        {
            string? userInput = inputDialog.InputText;
            Vimtodo newVimtodo = new Vimtodo(userInput!);
            _tasks!.Add(newVimtodo);
            TaskList.Items.Add(newVimtodo);
            SaveTasks(); 
        }
    }
    
    void SaveTasks()
    {
        string json = JsonSerializer.Serialize(_tasks);
        File.WriteAllText("tasks.json", json);
    }

    void UpdateTasks(int i = 0)
    {
        if (File.Exists("tasks.json"))
        {
            var json = File.ReadAllText("tasks.json");
            _tasks = JsonSerializer.Deserialize<List<Vimtodo>>(json);
            TaskList.Items.Clear();
            
            foreach (Vimtodo task in _tasks!)
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