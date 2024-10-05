using System.Runtime.InteropServices;

namespace Spravy.Domain.Models;

public struct MarshalRef<T> : IDisposable
    where T : struct
{
    public nint Handle;

    public MarshalRef(T value)
    {
        Handle = Marshal.AllocHGlobal(Marshal.SizeOf(value));
        Marshal.StructureToPtr(value, Handle, true);
    }

    public T Value
    {
        get =>
            (T)(
                Marshal.PtrToStructure(Handle, typeof(T))
                ?? throw new NullReferenceException(nameof(Handle))
            );
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(Handle);
    }
}
