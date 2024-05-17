using System;
using System.Collections.Generic;

namespace FairyGUI
{
    public delegate void EventCallback0();
    public delegate void EventCallback1(EventContext context);

    /// <summary>
    /// 
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        Dictionary<string, EventBridge> _dic;

        public EventDispatcher()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void AddEventListener(string strType, EventCallback1 callback)
        {
            GetBridge(strType).Add(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void AddEventListener(string strType, EventCallback0 callback)
        {
            GetBridge(strType).Add(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void RemoveEventListener(string strType, EventCallback1 callback)
        {
            if (_dic == null)
                return;

            EventBridge bridge = null;
            if (_dic.TryGetValue(strType, out bridge))
                bridge.Remove(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void RemoveEventListener(string strType, EventCallback0 callback)
        {
            if (_dic == null)
                return;

            EventBridge bridge = null;
            if (_dic.TryGetValue(strType, out bridge))
                bridge.Remove(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void AddCapture(string strType, EventCallback1 callback)
        {
            GetBridge(strType).AddCapture(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="callback"></param>
        public void RemoveCapture(string strType, EventCallback1 callback)
        {
            if (_dic == null)
                return;

            EventBridge bridge = null;
            if (_dic.TryGetValue(strType, out bridge))
                bridge.RemoveCapture(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveEventListeners()
        {
            RemoveEventListeners(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        public void RemoveEventListeners(string strType)
        {
            if (_dic == null)
                return;

            if (strType != null)
            {
                EventBridge bridge;
                if (_dic.TryGetValue(strType, out bridge))
                    bridge.Clear();
            }
            else
            {
                foreach (KeyValuePair<string, EventBridge> kv in _dic)
                    kv.Value.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public bool hasEventListeners(string strType)
        {
            EventBridge bridge = TryGetEventBridge(strType);
            if (bridge == null)
                return false;

            return !bridge.isEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public bool isDispatching(string strType)
        {
            EventBridge bridge = TryGetEventBridge(strType);
            if (bridge == null)
                return false;

            return bridge._dispatching;
        }

        internal EventBridge TryGetEventBridge(string strType)
        {
            if (_dic == null)
                return null;

            EventBridge bridge = null;
            _dic.TryGetValue(strType, out bridge);
            return bridge;
        }

        internal EventBridge GetEventBridge(string strType)
        {
            if (_dic == null)
                _dic = new Dictionary<string, EventBridge>();

            EventBridge bridge = null;
            if (!_dic.TryGetValue(strType, out bridge))
            {
                bridge = new EventBridge(this);
                _dic[strType] = bridge;
            }
            return bridge;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <returns></returns>
        public bool DispatchEvent(string strType)
        {
            return DispatchEvent(strType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool DispatchEvent(string strType, object data)
        {
            return InternalDispatchEvent(strType, null, data, null);
        }

        public bool DispatchEvent(string strType, object data, object initiator)
        {
            return InternalDispatchEvent(strType, null, data, initiator);
        }

        internal bool InternalDispatchEvent(string strType, EventBridge bridge, object data, object initiator)
        {
            if (bridge == null)
            {
                bridge = TryGetEventBridge(strType);
            }


            bool b1 = bridge != null && !bridge.isEmpty;
            if (b1)
            {
                EventContext context = EventContext.Get();
                context.initiator = initiator != null ? initiator : this;
                context.type = strType;
                context.data = data;

                if (b1)
                {
                    bridge.CallCaptureInternal(context);
                    bridge.CallInternal(context);
                }

                EventContext.Return(context);
                context.initiator = null;
                context.sender = null;
                context.data = null;

                return context._defaultPrevented;
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool DispatchEvent(EventContext context)
        {
            EventBridge bridge = TryGetEventBridge(context.type);

            EventDispatcher savedSender = context.sender;

            if (bridge != null && !bridge.isEmpty)
            {
                bridge.CallCaptureInternal(context);
                bridge.CallInternal(context);
            }
            context.sender = savedSender;
            return context._defaultPrevented;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool BroadcastEvent(string strType, object data)
        {
            EventContext context = EventContext.Get();
            context.initiator = this;
            context.type = strType;
            context.data = data;
            List<EventBridge> bubbleChain = context.callChain;
            bubbleChain.Clear();

            int length = bubbleChain.Count;
            for (int i = 0; i < length; ++i)
                bubbleChain[i].CallInternal(context);

            EventContext.Return(context);
            context.initiator = null;
            context.sender = null;
            context.data = null;
            return context._defaultPrevented;
        }

        EventBridge GetBridge(string strType)
        {
            if (strType == null)
                throw new Exception("event type cant be null");

            if (_dic == null)
                _dic = new Dictionary<string, EventBridge>();

            EventBridge bridge = null;
            if (!_dic.TryGetValue(strType, out bridge))
            {
                bridge = new EventBridge(this);
                _dic[strType] = bridge;
            }

            return bridge;
        }
    }
}
