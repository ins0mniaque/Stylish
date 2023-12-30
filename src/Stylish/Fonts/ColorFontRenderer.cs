namespace Stylish;

public class ColorFontRenderer
{
    // ThreadLocal usage inside emoji...
    // bool IsColorFontEnabled
    // https://stackoverflow.com/questions/46892844/gdi-can-i-use-the-new-windows-10-segoe-ui-emoji-colored-font-with-drawtext
    // Can all be generated with CsWin32: NativeMethods.txt => D2D1CreateFactory, ID2D1Factory
    //     public static class Direct2D1Util {
    //     public const string IID_ID2D1Factory =
    //         "06152247-6f50-465a-9245-118bfd3b6007";
    // 
    //     [DllImport("d2d1.dll", PreserveSig = false)]
    //     [return: MarshalAs(UnmanagedType.Interface)]
    //     private static extern object D2D1CreateFactory(D2D1_FACTORY_TYPE factoryType,
    //         [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
    //         D2D1_FACTORY_OPTIONS pFactoryOptions);
    // 
    //     public static ID2D1Factory CreateFactory() {
    //         D2D1_FACTORY_OPTIONS opts = new D2D1_FACTORY_OPTIONS();
    //         object factory = D2D1CreateFactory(
    //             D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED,
    //             new Guid(IID_ID2D1Factory), opts);
    //         return (ID2D1Factory)factory;
    //     }
    //
    // object factory;
    //         var hr = PInvoke.DWriteCreateFactory(
    //             DWRITE_FACTORY_TYPE.DWRITE_FACTORY_TYPE_SHARED,
    //             IID_IDWriteFactory5,
    //             out factory);
    //
    // var Result = PInvoke.D2D1CreateFactory ( D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof ( ID2D1Factory ).GUID, new D2D1_FACTORY_OPTIONS (), out var pFactory );
    // var Factory = Marshal.GetObjectForIUnknown ( new IntPtr ( pFactory ) ) as ID2D1Factory;
    // var RtProps = new D2D1_RENDER_TARGET_PROPERTIES () { };
    // var HwndRtProps = new D2D1_HWND_RENDER_TARGET_PROPERTIES () { hwnd = new HWND ( _NativeWindowHandle.ToInt32 () ), pixelSize = new D2D_SIZE_U () { width = 123, height = 123 } };
    // Factory.CreateHwndRenderTarget ( RtProps, HwndRtProps, null /*!?*/ );
    // }
}