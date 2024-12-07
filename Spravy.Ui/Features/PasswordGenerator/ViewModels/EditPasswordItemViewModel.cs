using Spravy.PasswordGenerator.Domain.Enums;

namespace Spravy.Ui.Features.PasswordGenerator.ViewModels;

public partial class EditPasswordItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string customAvailableCharacters = string.Empty;

    [ObservableProperty]
    private bool isEditCustomAvailableCharacters;

    [ObservableProperty]
    private bool isAvailableLowerLatin = true;

    [ObservableProperty]
    private bool isEditIsAvailableLowerLatin;

    [ObservableProperty]
    private bool isAvailableNumber = true;

    [ObservableProperty]
    private bool isEditIsAvailableNumber;

    [ObservableProperty]
    private bool isAvailableSpecialSymbols = true;

    [ObservableProperty]
    private bool isEditIsAvailableSpecialSymbols;

    [ObservableProperty]
    private bool isAvailableUpperLatin = true;

    [ObservableProperty]
    private bool isEditIsAvailableUpperLatin;

    [ObservableProperty]
    private string key = string.Empty;

    [ObservableProperty]
    private bool isEditKey;

    [ObservableProperty]
    private ushort length = 512;

    [ObservableProperty]
    private bool isEditLength;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private bool isEditLogin;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isEditName;

    [ObservableProperty]
    private string regex = string.Empty;

    [ObservableProperty]
    private bool isEditRegex;

    [ObservableProperty]
    private PasswordItemType type;

    [ObservableProperty]
    private bool isEditType;

    public EditPasswordItemViewModel(bool isEditShow)
    {
        IsEditShow = isEditShow;
        PropertyChanged += OnPropertyChanged;
    }

    public bool IsEditShow { get; }

    public EditPasswordItems ToEditPasswordItems()
    {
        var result = new EditPasswordItems();

        if (IsEditName)
        {
            result = result.SetName(new(Name));
        }

        if (IsEditCustomAvailableCharacters)
        {
            result = result.SetCustomAvailableCharacters(new(CustomAvailableCharacters));
        }

        if (IsAvailableLowerLatin)
        {
            result = result.SetIsAvailableLowerLatin(new(IsAvailableLowerLatin));
        }

        if (IsEditIsAvailableNumber)
        {
            result = result.SetIsAvailableNumber(new(IsAvailableNumber));
        }

        if (IsEditIsAvailableSpecialSymbols)
        {
            result = result.SetIsAvailableSpecialSymbols(new(IsAvailableSpecialSymbols));
        }

        if (IsEditIsAvailableUpperLatin)
        {
            result = result.SetIsAvailableUpperLatin(new(IsAvailableUpperLatin));
        }

        if (IsEditKey)
        {
            result = result.SetKey(new(Key));
        }

        if (IsEditLength)
        {
            result = result.SetLength(new(Length));
        }

        if (IsEditLogin)
        {
            result = result.SetLogin(new(Login));
        }

        if (IsEditRegex)
        {
            result = result.SetRegex(new(Regex));
        }
        
        if (IsEditType)
        {
            result = result.SetType(new(Type));
        }

        return result;
    }
    
    public AddPasswordOptions ToAddPasswordOptions()
    {
        return new(
            Name,
            Key,
            Length,
            Regex,
            IsAvailableLowerLatin,
            IsAvailableUpperLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            CustomAvailableCharacters,
            Login,
            Type
        );
    }

    public Result UndoAllUi()
    {
        IsEditCustomAvailableCharacters = false;
        IsEditIsAvailableLowerLatin = false;
        IsEditIsAvailableNumber = false;
        IsEditIsAvailableSpecialSymbols = false;
        IsEditIsAvailableUpperLatin = false;
        IsEditKey = false;
        IsEditLength = false;
        IsEditLogin = false;
        IsEditName = false;
        IsEditRegex = false;
        IsEditType = false;

        return Result.Success;
    }

    public Result SetItemUi(PasswordItemEntityNotify notify)
    {
        CustomAvailableCharacters = notify.CustomAvailableCharacters;
        IsAvailableLowerLatin = notify.IsAvailableLowerLatin;
        IsAvailableNumber = notify.IsAvailableNumber;
        IsAvailableSpecialSymbols = notify.IsAvailableSpecialSymbols;
        IsAvailableUpperLatin = notify.IsAvailableUpperLatin;
        Key = notify.Key;
        Length = notify.Length;
        Login = notify.Login;
        Name = notify.Name;
        Regex = notify.Regex;
        Type = notify.Type;

        return Result.Success;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(CustomAvailableCharacters):
            {
                IsEditCustomAvailableCharacters = true;

                break;
            }
            case nameof(IsAvailableLowerLatin):
            {
                IsEditIsAvailableLowerLatin = true;

                break;
            }
            case nameof(IsAvailableNumber):
            {
                IsEditIsAvailableNumber = true;

                break;
            }
            case nameof(IsAvailableSpecialSymbols):
            {
                IsEditIsAvailableSpecialSymbols = true;

                break;
            }
            case nameof(IsAvailableUpperLatin):
            {
                IsEditIsAvailableUpperLatin = true;

                break;
            }
            case nameof(Key):
            {
                IsEditKey = true;

                break;
            }
            case nameof(Length):
            {
                IsEditLength = true;

                break;
            }
            case nameof(Login):
            {
                IsEditLogin = true;

                break;
            }
            case nameof(Name):
            {
                IsEditName = true;

                break;
            }
            case nameof(Regex):
            {
                IsEditRegex = true;

                break;
            }
            case nameof(Type):
            {
                IsEditType = true;

                break;
            }
        }
    }
}