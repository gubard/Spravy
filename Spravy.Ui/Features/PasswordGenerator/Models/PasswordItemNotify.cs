using System;
using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.PasswordGenerator.Models;

public class PasswordItemNotify : NotifyBase, IIdProperty
{
    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public string Key { get; set; } = string.Empty;

    [Reactive]
    public string AvailableCharacters { get; set; } = string.Empty;

    [Reactive]
    public ushort Length { get; set; }

    [Reactive]
    public string Regex { get; set; } = string.Empty;
}