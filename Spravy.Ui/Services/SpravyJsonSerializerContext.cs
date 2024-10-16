using Spravy.Authentication.Domain.Client.Models;
using Spravy.PasswordGenerator.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.Ui.Features.Schedule.Settings;
using Spravy.Ui.Setting;

namespace Spravy.Ui.Services;

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(UserIdentifierTypeOutOfRangeError))]
[JsonSerializable(typeof(NonItemSelectedError))]
[JsonSerializable(typeof(NotViewForViewModelError))]
[JsonSerializable(typeof(ToDoItemIsCanOutOfRangeError))]
[JsonSerializable(typeof(ToDoItemAlreadyCompleteError))]
[JsonSerializable(typeof(ToDoItemStatusOutOfRangeError))]
[JsonSerializable(typeof(ToDoItemTypeOutOfRangeError))]
[JsonSerializable(typeof(UserWithEmailExistsError))]
[JsonSerializable(typeof(UnknownError))]
[JsonSerializable(typeof(UserWithLoginExistsError))]
[JsonSerializable(typeof(CastError))]
[JsonSerializable(typeof(ServiceUnavailableError))]
[JsonSerializable(typeof(DefaultCtorResultError))]
[JsonSerializable(typeof(CanceledByUserError))]
[JsonSerializable(typeof(UserWithLoginNotExistsError))]
[JsonSerializable(typeof(MultiUsersWithSameLoginError))]
[JsonSerializable(typeof(UserNotVerifiedError))]
[JsonSerializable(typeof(PropertyNullValueError))]
[JsonSerializable(typeof(PropertyEmptyStringError))]
[JsonSerializable(typeof(PropertyWhiteSpaceStringError))]
[JsonSerializable(typeof(VariableNullValueError))]
[JsonSerializable(typeof(VariableStringMaxLengthError))]
[JsonSerializable(typeof(VariableStringMinLengthError))]
[JsonSerializable(typeof(VariableInvalidCharsError))]
[JsonSerializable(typeof(NotFoundNamedError))]
[JsonSerializable(typeof(EmptyArrayError))]
[JsonSerializable(typeof(MultiUsersWithSameEmailError))]
[JsonSerializable(typeof(MultiValuesArrayError))]
[JsonSerializable(typeof(UserWithEmailNotExistsError))]
[JsonSerializable(typeof(RoleOutOfRangeError))]
[JsonSerializable(typeof(ContinueError))]
[JsonSerializable(typeof(MaxCycleCountReachedError))]
[JsonSerializable(typeof(NotFoundTypeError))]
[JsonSerializable(typeof(LoginViewModelSetting))]
[JsonSerializable(typeof(AddToDoItemViewModelSetting))]
[JsonSerializable(typeof(ToDoItemViewModelSetting))]
[JsonSerializable(typeof(LeafToDoItemsViewModelSetting))]
[JsonSerializable(typeof(ResetToDoItemViewModelSetting))]
[JsonSerializable(typeof(RootToDoItemsViewModelSetting))]
[JsonSerializable(typeof(SearchViewModelSetting))]
[JsonSerializable(typeof(LoginStorageItem))]
[JsonSerializable(typeof(SettingModel))]
[JsonSerializable(typeof(EmailOrLoginInputViewModelSetting))]
[JsonSerializable(typeof(VerificationCodePasswordError))]
[JsonSerializable(typeof(Setting.Setting))]
[JsonSerializable(typeof(GrpcScheduleServiceOptionsConfiguration))]
[JsonSerializable(typeof(GrpcToDoServiceOptionsConfiguration))]
[JsonSerializable(typeof(GrpcAuthenticationServiceOptionsConfiguration))]
[JsonSerializable(typeof(GrpcPasswordServiceOptionsConfiguration))]
[JsonSerializable(typeof(AppOptionsConfiguration))]
[JsonSerializable(typeof(AddToDoItemToFavoriteEventOptions))]
[JsonSerializable(typeof(AddTimerViewModelSettings))]
[JsonSerializable(typeof(AddToDoItemToFavoriteEventViewModelSettings))]
[JsonSerializable(typeof(ToDoItemCreateTimerViewModelSettings))]
[JsonSerializable(typeof(AppSetting))]
public partial class SpravyJsonSerializerContext : JsonSerializerContext;
