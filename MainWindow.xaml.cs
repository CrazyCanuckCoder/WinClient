using System.Windows;
using WinClient.Logic;

namespace WinClient;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    /// <summary>
    /// Using a DependencyProperty as the backing store for Status.
    /// </summary>
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(ClientStatus), typeof(MainWindow),
            new PropertyMetadata(ClientStatus.Unregistered));

    #endregion Dependency Properties


    /// <summary>
    /// Indicates the current status of the registration / login process.
    /// </summary>
    public ClientStatus Status
    {
        get => (ClientStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }



    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}