using System;
using System.Collections.Generic;
using System.Linq;
using OTA.Logging;
using System.Collections.Concurrent;
using OTA.Plugin;
using OTA;

namespace TDSM.Core
{
    public partial class Entry
    {
        private Dictionary<ComponentEvent, ConcurrentQueue<Action<Entry>>> _componentEvents = new Dictionary<ComponentEvent, ConcurrentQueue<Action<Entry>>>();

        public void AddComponents<T>()
        {
            var methods = typeof(T).Assembly.GetTypesLoaded()
                .SelectMany(t => t.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                .Select(m => new { Method = m, Attributes = m.GetCustomAttributes(typeof(TDSMComponentAttribute), true) })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0);

            foreach (var method in methods)
            {
                var evt = method.Attributes[0] as TDSMComponentAttribute;

                ConcurrentQueue<Action<Entry>> calls;
                if (!_componentEvents.ContainsKey(evt.ComponentEvent))
                {
                    calls = new ConcurrentQueue<Action<Entry>>();
                    _componentEvents.Add(evt.ComponentEvent, calls);
                }
                else calls = _componentEvents[evt.ComponentEvent];

                try
                {
                    calls.Enqueue((Action<Entry>)Delegate.CreateDelegate(typeof(Action<Entry>), method.Method));
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e, "Failed to add component " + method.Method.Name);
                }
            }
        }

        public void RunComponent(ComponentEvent componentEvent)
        {
            if (_componentEvents.ContainsKey(componentEvent))
            {
                foreach (var callback in _componentEvents[componentEvent])
                {
                    callback(this);
                }
            }
        }
    }
}

