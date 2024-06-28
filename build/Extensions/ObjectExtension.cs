using System;

namespace _build.Extensions;

public static class ObjectExtension
{
    public static TObject ThrowIfNull<TObject>(this TObject obj)
    {
        if (obj is null)
        {
            throw new NullReferenceException();
        }

        return obj;
    }
}
