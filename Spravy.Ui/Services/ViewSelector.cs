namespace Spravy.Ui.Services;

public class ViewSelector : IViewSelector
{
    private readonly IServiceFactory serviceFactory;

    public ViewSelector(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }

    public Result<Control> GetView(Type viewModelType)
    {
        if (typeof(MainViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<MainView>());
        }

        if (typeof(ToDoItemCreateTimerViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoItemCreateTimerView>());
        }

        if (typeof(PolicyViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<PolicyView>());
        }

        if (typeof(PaneViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<PaneView>());
        }

        if (typeof(MainSplitViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<MainSplitView>());
        }

        if (typeof(EditDescriptionViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<EditDescriptionView>());
        }

        if (typeof(ConfirmViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ConfirmView>());
        }

        if (typeof(DeleteAccountViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<DeleteAccountView>());
        }

        if (typeof(EmailOrLoginInputViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<EmailOrLoginInputView>());
        }

        if (typeof(InfoViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<InfoView>());
        }

        if (typeof(SettingViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<SettingView>());
        }

        if (typeof(TextViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<TextView>());
        }

        if (typeof(MainProgressBarViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<MainProgressBarView>());
        }

        if (typeof(ToDoItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoItemsView>());
        }

        if (typeof(AddToDoItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<AddToDoItemView>());
        }

        if (typeof(DeleteToDoItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<DeleteToDoItemView>());
        }

        if (typeof(MultiToDoItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<MultiToDoItemsView>());
        }

        if (typeof(ToDoItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoItemView>());
        }

        if (typeof(ResetToDoItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ResetToDoItemView>());
        }

        if (typeof(RootToDoItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<RootToDoItemsView>());
        }

        if (typeof(ToDoItemToStringSettingsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoItemToStringSettingsView>());
        }

        if (typeof(ToDoItemSelectorViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoItemSelectorView>());
        }

        if (typeof(ToDoSubItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoSubItemsView>());
        }

        if (typeof(ChangeToDoItemOrderIndexViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ChangeToDoItemOrderIndexView>());
        }

        if (typeof(EditToDoItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<EditToDoItemView>());
        }

        if (typeof(RandomizeChildrenOrderViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<RandomizeChildrenOrderView>());
        }

        if (typeof(ToDoItemSettingsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ToDoItemSettingsView>());
        }

        if (typeof(SearchToDoItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<SearchToDoItemsView>());
        }

        if (typeof(LeafToDoItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<LeafToDoItemsView>());
        }

        if (typeof(TodayToDoItemsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<TodayToDoItemsView>());
        }

        if (typeof(EditPasswordItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<EditPasswordItemView>());
        }

        if (typeof(AddPasswordItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<AddPasswordItemView>());
        }

        if (typeof(DeletePasswordItemViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<DeletePasswordItemView>());
        }

        if (typeof(PasswordGeneratorViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<PasswordGeneratorView>());
        }

        if (typeof(PasswordItemSettingsViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<PasswordItemSettingsView>());
        }

        if (typeof(ErrorViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ErrorView>());
        }

        if (typeof(ExceptionViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ExceptionView>());
        }

        if (typeof(VerificationCodeViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<VerificationCodeView>());
        }

        if (typeof(LoginViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<LoginView>());
        }

        if (typeof(ForgotPasswordViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ForgotPasswordView>());
        }

        if (typeof(CreateUserViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<CreateUserView>());
        }

        if (typeof(TimersViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<TimersView>());
        }

        if (typeof(AddTimerViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<AddTimerView>());
        }

        if (typeof(AddToDoItemToFavoriteEventViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<AddToDoItemToFavoriteEventView>());
        }

        if (typeof(DeleteTimerViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<DeleteTimerView>());
        }

        if (typeof(ChangeParentViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<ChangeParentView>());
        }

        if (typeof(CloneViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<CloneView>());
        }

        if (typeof(CreateReferenceViewModel) == viewModelType)
        {
            return new(serviceFactory.CreateService<CreateReferenceView>());
        }

        return new(new NotViewForViewModelError(viewModelType.Name));
    }
}