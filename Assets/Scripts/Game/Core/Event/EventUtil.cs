using FairyGUI;

public class EventUtil
{
    private static EventDispatcher _ed = new EventDispatcher();

    public static void on(string type, EventCallback0 callback,  bool isOnce = false)
    {
        if (isOnce)
        {
            EventCallback0 tmp = null;
            tmp = () => {
                callback();
                _ed.RemoveEventListener(type, tmp);
            };
            _ed.AddEventListener(type, tmp);
        }
        else
        {
            _ed.AddEventListener(type, callback);
        }
        
    }


    public static void on(string type, EventCallback1 callback, bool isOnce = false)
    {
        if (isOnce)
        {
            EventCallback1 tmp = null;
            tmp = (EventContext context) => { 
                callback(context);
                _ed.RemoveEventListener(type, tmp);
            };
            _ed.AddEventListener(type, tmp);
        }
        else
        {
            _ed.AddEventListener(type, callback);
        }
        
    }

    public static void once(string type, EventCallback0 callback)
    {
        on(type, callback, true);
    }

    public static void once(string type, EventCallback1 callback)
    {
        on(type, callback, true);
    }

    public static void off(string type, EventCallback0 callback)
    {
        _ed.RemoveEventListener(type, callback);
    }

    public static void off(string type, EventCallback1 callback)
    {
        _ed.RemoveEventListener(type, callback);
    }

    public static void dispatch(string type, object[] data = null)
    {
        _ed.DispatchEvent(type, data);
    }
}
