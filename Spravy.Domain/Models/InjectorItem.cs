﻿using System.Linq.Expressions;

namespace Spravy.Domain.Models;

public readonly record struct InjectorItem(InjectorItemType Type, Expression Expression);