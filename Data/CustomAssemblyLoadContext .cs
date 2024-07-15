using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private IntPtr _nativeLibraryHandle;

    public CustomAssemblyLoadContext()
    {
    }

    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        _nativeLibraryHandle = LoadUnmanagedDll(absolutePath);
        return _nativeLibraryHandle;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadLibraryW(unmanagedDllName);
    }
    public void UnloadLibrary()
    {
        if (_nativeLibraryHandle != IntPtr.Zero)
        {
            FreeLibrary(_nativeLibraryHandle);
            _nativeLibraryHandle = IntPtr.Zero;
        }
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);
}



/*using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private IntPtr _nativeLibraryHandle;

    public CustomAssemblyLoadContext()
    {
    }

    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        _nativeLibraryHandle = LoadUnmanagedDll(absolutePath);
        return _nativeLibraryHandle;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadLibraryW(unmanagedDllName);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null; // or implement loading logic if needed
    }

    protected override void Dispose(bool disposing)
    {
        if (_nativeLibraryHandle != IntPtr.Zero)
        {
            FreeUnmanagedLibrary(_nativeLibraryHandle);
            _nativeLibraryHandle = IntPtr.Zero;
        }
        base.Dispose(disposing);
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    private void FreeUnmanagedLibrary(IntPtr handle)
    {
        FreeLibrary(handle);
    }
}
*/



/*using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Runtime.Loader;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    private IntPtr _nativeLibraryHandle;

    public CustomAssemblyLoadContext()
    {
    }

    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        _nativeLibraryHandle = LoadUnmanagedDll(absolutePath);
        return _nativeLibraryHandle;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (_nativeLibraryHandle != IntPtr.Zero)
        {
            FreeUnmanagedLibrary(_nativeLibraryHandle);
            _nativeLibraryHandle = IntPtr.Zero;
        }
        base.Dispose(disposing);
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    private override IntPtr LoadUnmanagedDllFromPath(string unmanagedDllPath)
    {
        IntPtr handle = LoadLibraryW(unmanagedDllPath);
        if (handle == IntPtr.Zero)
        {
            throw new DllNotFoundException($"Unable to load library {unmanagedDllPath}");
        }
        return handle;
    }

    private void FreeUnmanagedLibrary(IntPtr handle)
    {
        FreeLibrary(handle);
    }
}
*/




/*using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

public class CustomAssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
{
    private IntPtr _nativeLibraryHandle;

    public CustomAssemblyLoadContext()
    {
    }

    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        _nativeLibraryHandle = LoadUnmanagedDll(absolutePath);
        return _nativeLibraryHandle;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllName);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (_nativeLibraryHandle != IntPtr.Zero)
        {
            FreeUnmanagedLibrary(_nativeLibraryHandle);
            _nativeLibraryHandle = IntPtr.Zero;
        }
        base.Dispose(disposing);
    }

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    private IntPtr LoadUnmanagedDllFromPath(string unmanagedDllPath)
    {
        IntPtr handle = LoadLibraryW(unmanagedDllPath);
        if (handle == IntPtr.Zero)
        {
            throw new DllNotFoundException($"Unable to load library {unmanagedDllPath}");
        }
        return handle;
    }

    private void FreeUnmanagedLibrary(IntPtr handle)
    {
        FreeLibrary(handle);
    }
}
*/