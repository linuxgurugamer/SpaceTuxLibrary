This is a simple example of how to initialize the logging class
and how to reference it


Define Log as a internal or public static in a single class
Make sure it is initialized before first use
Use the "using static" directive to remove the need to reference the entire class path:

For the following examples, the following are used:

    MyNameSpace     Namespace of mod
    MyClass         Class name


Example code to initialize logging class with different logging levels when
compiled with the DEBUG flag enabled
========================================
using static MyNameSpace.MyClass;

[KSPAddon(KSPAddon.Startup.Instantly, true)]
public class InitLog : MonoBehaviour
{
    protected void Awake()
    {
        Log = new KSP_Log.Log("DangIt"
#if DEBUG
                , KSP_Log.Log.LEVEL.INFO
#endif
                );
    }
}


Sample class where the variable/reference is located
====================================================

using KSP_Log;
namespace MyNameSpace
{
    public class MyClass
    {
        public static KSP_Log.Log Log;
    }
}


Sample class showing the use of the "using static" and
actual use of the log
======================================================

using static MyNameSpace.MyClass;
Class SecondClass
{
    void Start()
    {
        Log.Info("Start");
    }
}
        
