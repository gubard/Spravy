using System.Text.Json.Serialization;
using Spravy.Db.Errors;
using Spravy.Domain.Errors;

namespace Spravy.EventBus.Service.Services;

[JsonSourceGenerationOptions]
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
[JsonSerializable(typeof(NotFoundEntityError))]
public partial class SpravyJsonSerializerContext : JsonSerializerContext;