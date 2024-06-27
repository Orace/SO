using System;
using System.Windows.Forms;

namespace SO_8940675;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (DesignMode)
            return;

        var model = new MainModel();

        comboBox1.DataBindings.Add(nameof(comboBox1.DataSource), model, nameof(model.Values1), true, DataSourceUpdateMode.Never);
        comboBox1.DataBindings.Add(nameof(comboBox1.SelectedItem), model, nameof(model.Value1), true, DataSourceUpdateMode.OnPropertyChanged);
        comboBox1.DataBindings.Add(nameof(comboBox1.SelectedValue), model, nameof(model.Value1), true, DataSourceUpdateMode.OnPropertyChanged);

        comboBox2.DataBindings.Add(nameof(comboBox2.DataSource), model, nameof(model.Values2), true, DataSourceUpdateMode.Never);
        comboBox2.DataBindings.Add(nameof(comboBox2.SelectedItem), model, nameof(model.Value2), true, DataSourceUpdateMode.OnPropertyChanged);
        comboBox2.DataBindings.Add(nameof(comboBox2.SelectedValue), model, nameof(model.Value2), true, DataSourceUpdateMode.OnPropertyChanged);
    }
}