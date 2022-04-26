namespace SO_39748090;

public class Kernel
{
    public Kernel()
    {
        var model = new Model();
        FlatViewModel = new FlatViewModel(model);
        ViewModel = new ViewModel(model);
    }

    public FlatViewModel FlatViewModel { get; }

    public ViewModel ViewModel { get; }
}