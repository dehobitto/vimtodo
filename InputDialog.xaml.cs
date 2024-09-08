using System.Windows;
using System.Windows.Input;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog
    {
        private const Key InputDialogSave = Key.Enter;
        public string? InputText
        {
            get {return InputTextBox.Text;}
            set {InputTextBox.Text = value;}
        }
        
        public InputDialog(string text = "")
        {
            InitializeComponent();
            InputTextBox.Text = text; 
            Loaded += InputDialog_Loaded; 
            PreviewKeyDown += InputDialog_Save; 
        }

        private void InputDialog_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus(); 
            InputTextBox.Select(InputTextBox.Text.Length, 0); 
        }

        private void InputDialog_Save(object sender, KeyEventArgs e)
        {
            if (e.Key == InputDialogSave)
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