using System.Windows;
using System.Windows.Input;

namespace TaskManager
{
    public partial class DescriptionWindow : Window
    {
        private const Key DescriptionWindowSave = Key.Enter;
        public string? InputText
        {
            get {return InputTextBox.Text;}
            set {InputTextBox.Text = value;}
        }
        
        public DescriptionWindow(string text)
        {
            InitializeComponent();
            // Set the initial text from the main window
            InputTextBox.Text = text;
            Loaded += DescriptionWindow_Loaded; 
            PreviewKeyDown += DescriptionWindow_Save; 
        }
        
        private void DescriptionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus(); 
            InputTextBox.Select(InputTextBox.Text.Length, 0); 
        }

        private void DescriptionWindow_Save(object sender, KeyEventArgs e)
        {
            if (e.Key == DescriptionWindowSave)
            {
                InputDialog_Close();
                e.Handled = true;
            }
        }

        private void InputDialog_Close()
        {
            if (!string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                InputText = InputTextBox.Text;
                DialogResult = true;
            }
        }
    }
}