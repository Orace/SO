namespace SO_75067133;

public partial class MyUserControl
{
    public MyUserControl()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            Label.Content = "Loaded";
        };
    }
}