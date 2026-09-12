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
        DataContext = this;
    }

    #region Fields
    /// <summary>
    /// The client used to communicate with the server API for registration and login.
    /// </summary>
    private readonly ServerApiClient _serverApiClient = new();

    /// <summary>
    /// The user ID obtained after a successful registration, used for login.
    /// </summary>
    private string _userID = string.Empty;

    #endregion Fields

    #region Dependency Properties

    /// <summary>
    /// Using a DependencyProperty as the backing store for Status.
    /// </summary>
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(ClientStatus), typeof(MainWindow),
            new PropertyMetadata(ClientStatus.Unregistered));

    /// <summary>
    /// Using a DependencyProperty as the backing store for RegisterButtonIsEnabled.
    /// </summary>
    public static readonly DependencyProperty RegisterButtonIsEnabledProperty =
        DependencyProperty.Register(nameof(RegisterButtonIsEnabled), typeof(bool), typeof(MainWindow),
            new PropertyMetadata(true));

    /// <summary>
    /// Using a DependencyProperty as the backing store for LoginControlsAreEnabled.
    /// </summary>
    public static readonly DependencyProperty LoginButtonIsEnabledProperty =
        DependencyProperty.Register(nameof(LoginButtonIsEnabled), typeof(bool), typeof(MainWindow),
            new PropertyMetadata(false));

    /// <summary>
    /// Using a DependencyProperty as the backing store for RegisterErrorVisibility.
    /// </summary>
    public static readonly DependencyProperty RegisterErrorVisibilityProperty =
        DependencyProperty.Register(nameof(RegisterErrorVisibility), typeof(Visibility), typeof(MainWindow),
            new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for RegisterError.
    /// </summary>
    public static readonly DependencyProperty RegisterErrorProperty =
        DependencyProperty.Register(nameof(RegisterError), typeof(string), typeof(MainWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Using a DependencyProperty as the backing store for RegisterTextVisibility.
    /// </summary>
    public static readonly DependencyProperty RegisterTextVisibilityProperty =
        DependencyProperty.Register(nameof(RegisterTextVisibility), typeof(Visibility), typeof(MainWindow),
            new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for RegisterText.
    /// </summary>
    public static readonly DependencyProperty RegisterTextProperty =
        DependencyProperty.Register(nameof(RegisterText), typeof(string), typeof(MainWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Using a DependencyProperty as the backing store for LoginErrorVisibility.
    /// </summary>
    public static readonly DependencyProperty LoginErrorVisibilityProperty =
        DependencyProperty.Register(nameof(LoginErrorVisibility), typeof(Visibility), typeof(MainWindow),
            new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for LoginError.
    /// </summary>
    public static readonly DependencyProperty LoginErrorProperty =
        DependencyProperty.Register(nameof(LoginError), typeof(string), typeof(MainWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Using a DependencyProperty as the backing store for LoginTextVisibility.
    /// </summary>
    public static readonly DependencyProperty LoginTextVisibilityProperty =
        DependencyProperty.Register(nameof(LoginTextVisibility), typeof(Visibility), typeof(MainWindow),
            new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for LoginText.
    /// </summary>
    public static readonly DependencyProperty LoginTextProperty =
        DependencyProperty.Register(nameof(LoginText), typeof(string), typeof(MainWindow),
            new PropertyMetadata(string.Empty));

    #endregion Dependency Properties

    #region Properties

    /// <summary>
    /// Indicates the current status of the registration / login process.
    /// </summary>
    public ClientStatus Status
    {
        get => (ClientStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// True if the Register controls are enabled; false otherwise.
    /// </summary>
    public bool RegisterButtonIsEnabled
    {
        get => (bool)GetValue(RegisterButtonIsEnabledProperty);
        set => SetValue(RegisterButtonIsEnabledProperty, value);
    }

    /// <summary>
    /// True if the Login controls are enabled; false otherwise.
    /// </summary>
    public bool LoginButtonIsEnabled
    {
        get => (bool)GetValue(LoginButtonIsEnabledProperty);
        set => SetValue(LoginButtonIsEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the visibility of the registration error message.
    /// </summary>
    public Visibility RegisterErrorVisibility
    {
        get => (Visibility)GetValue(RegisterErrorVisibilityProperty);
        set => SetValue(RegisterErrorVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the error message to display when registration fails.
    /// </summary>
    public string RegisterError
    {
        get => (string)GetValue(RegisterErrorProperty);
        set => SetValue(RegisterErrorProperty, value);
    }

    /// <summary>
    /// Gets or sets the visibility of the registration success message.
    /// </summary>
    public Visibility RegisterTextVisibility
    {
        get => (Visibility)GetValue(RegisterTextVisibilityProperty);
        set => SetValue(RegisterTextVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the success message to display when registration succeeds.
    /// </summary>
    public string RegisterText
    {
        get => (string)GetValue(RegisterTextProperty);
        set => SetValue(RegisterTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the visibility of the login error message.
    /// </summary>
    public Visibility LoginErrorVisibility
    {
        get => (Visibility)GetValue(LoginErrorVisibilityProperty);
        set => SetValue(LoginErrorVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the error message to display when login fails.
    /// </summary>
    public string LoginError
    {
        get => (string)GetValue(LoginErrorProperty);
        set => SetValue(LoginErrorProperty, value);
    }

    /// <summary>
    /// Gets or sets the visibility of the login success message.
    /// </summary>
    public Visibility LoginTextVisibility
    {
        get => (Visibility)GetValue(LoginTextVisibilityProperty);
        set => SetValue(LoginTextVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the success message to display when login succeeds.
    /// </summary>
    public string LoginText
    {
        get => (string)GetValue(LoginTextProperty);
        set => SetValue(LoginTextProperty, value);
    }

    #endregion Properties


    #region Methods

    /// <summary>
    /// Clears the registration success and error messages, and hides their visibility.
    /// </summary>
    private void ClearRegisterTexts()
    {
        RegisterText = string.Empty;
        RegisterError = string.Empty;
        RegisterTextVisibility = Visibility.Collapsed;
        RegisterErrorVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Clears the login success and error messages, and hides their visibility.
    /// </summary>
    private void ClearLoginTexts()
    {
        LoginText = string.Empty;
        LoginError = string.Empty;
        LoginTextVisibility = Visibility.Collapsed;
        LoginErrorVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Updates the UI to show a registration error message and re-enables the Register button.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    private void UpdateRegisterError(string message)
    {
        RegisterButtonIsEnabled = true;
        RegisterError = message;
        RegisterErrorVisibility = Visibility.Visible;
        RegisterTextVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Updates the UI to show a login error message and re-enables the login controls.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    private void UpdateLoginError(string message)
    {
        LoginButtonIsEnabled = true;
        LoginError = message;
        LoginErrorVisibility = Visibility.Visible;
        LoginTextVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Updates the UI to show a registration success message and hides the error message.
    /// </summary>
    /// <param name="successText">The success message to display.</param>
    private void UpdateRegisterText(string successText)
    {
        RegisterText = successText;
        RegisterTextVisibility = Visibility.Visible;
        RegisterErrorVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Updates the UI to show a login success message and hides the error message.
    /// </summary>
    /// <param name="message">The success message to display.</param>
    private void UpdateLoginText(string message)
    {
        LoginText = message;
        LoginTextVisibility = Visibility.Visible;
        LoginErrorVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Handles the change of the ClientStatus property and updates the UI accordingly.
    /// </summary>
    /// <param name="newStatus">The new status value.</param>
    private void OnStatusChanged(ClientStatus newStatus)
    {
        Status = newStatus;

        switch (newStatus)
        {
            case ClientStatus.Unregistered:
                RegisterButtonIsEnabled = true;
                LoginButtonIsEnabled = false;
                break;

            case ClientStatus.Registered:
                RegisterButtonIsEnabled = false;
                LoginButtonIsEnabled = true;
                break;

            case ClientStatus.LoggedIn:
                RegisterButtonIsEnabled = true;
                LoginButtonIsEnabled = false;
                break;

            case ClientStatus.Registering:
                RegisterButtonIsEnabled = false;
                LoginButtonIsEnabled = false;
                break;

            case ClientStatus.LoggingIn:
                RegisterButtonIsEnabled = false;
                LoginButtonIsEnabled = false;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
        }
    }

    /// <summary>
    /// Parses the result of the registration request and extracts the user ID if successful.
    /// </summary>
    /// <param name="result">The result from the registration request.</param>
    /// <param name="successText">The success message to display.</param>
    /// <param name="errorText">The error message to display.</param>
    /// <returns>True to indicate the registration was successful, false otherwise.</returns>
    private bool ParseRegisterResult(string result, out string successText, out string errorText)
    {
        if (string.IsNullOrWhiteSpace(result))
        {
            successText = string.Empty;
            errorText = "Registration failed: empty response from server.";
            return false;
        }
        else if (result.IndexOf("user_id") > 0)
        {
            int idStartIndex = result.IndexOf("user_id") + "user_id".Length;
            _userID = result[idStartIndex..].Trim('"', '}', ' ', ':');
            successText = $"Registration successful. User ID: {_userID}";
            errorText = string.Empty;
            return true;
        }
        else
        {
            successText = string.Empty;
            errorText = $"Registration failed: unexpected response from server: {result}.";
            return false;
        }
    }

    #endregion Methods

    #region Event Handlers

    private async void RegisterButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            OnStatusChanged(ClientStatus.Registering);
            RegisterButtonIsEnabled = false;
            ClearRegisterTexts();
            string result = await _serverApiClient.RegisterAsync("");
            if (ParseRegisterResult(result, out string successText, out string errorText))
            {
                UpdateRegisterText(successText);
                OnStatusChanged(ClientStatus.Registered);
            }
            else
            {
                UpdateRegisterError(errorText);
                OnStatusChanged(ClientStatus.Unregistered);
            }
        }
        catch (OperationCanceledException)
        {
            UpdateRegisterError("Registration was canceled.");
            OnStatusChanged(ClientStatus.Unregistered);
        }
        catch (ServerApiException saException)
        {
            UpdateRegisterError(saException.Message);
            OnStatusChanged(ClientStatus.Unregistered);
        }
        catch (Exception ex)
        {
            UpdateRegisterError($"An unexpected error occurred: {ex.Message}");
            OnStatusChanged(ClientStatus.Unregistered);
        }
    }

    private async void LoginButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        try
        {
            OnStatusChanged(ClientStatus.LoggingIn);
            LoginButtonIsEnabled = false;
            ClearLoginTexts();
            var result = await _serverApiClient.LoginAsync(_userID);
            if (result)
            {
                UpdateLoginText("Login successful.");
                OnStatusChanged(ClientStatus.LoggedIn);
            }
            else
            {
                UpdateLoginError("Login failed.");
                OnStatusChanged(ClientStatus.Registered);
            }
        }
        catch (OperationCanceledException)
        {
            UpdateLoginError("Login was canceled.");
            OnStatusChanged(ClientStatus.Registered);
        }
        catch (ServerApiException saException)
        {
            if (saException.Message.Contains("user not found"))
            {
                // Invalid user ID, reset to unregistered state.
                UpdateLoginError("Login failed: user not found. Please register again.");
                OnStatusChanged(ClientStatus.Unregistered);
            }
            else
            {
                UpdateLoginError(saException.Message);
                OnStatusChanged(ClientStatus.Registered);
            }
        }
        catch (Exception ex)
        {
            UpdateLoginError($"An unexpected error occurred: {ex.Message}");
            OnStatusChanged(ClientStatus.Registered);
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    #endregion Event Handlers
}