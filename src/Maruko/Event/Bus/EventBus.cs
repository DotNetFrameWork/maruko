using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using log4net;
using Maruko.Event.Bus.Factories;
using Maruko.Event.Bus.Factories.Internals;
using Maruko.Event.Bus.Handlers;
using Maruko.Event.Bus.Handlers.Internals;
using Maruko.Logger;
using Maruko.Utils;

namespace Maruko.Event.Bus
{
    /// <summary>
    ///     Implements EventBus as Singleton pattern.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly IEventStore _eventStore;

        /// <summary>
        ///     Gets the default <see cref="EventBus" /> instance.
        /// </summary>
        public static EventBus Default { get; } = new EventBus();

        /// <summary>
        ///     Reference to the Logger.
        /// </summary>
        public ILog Logger { get; set; }

        public EventBus()
        {
            _eventStore = ContainerManager.Current.Resolve<IEventStore>();
            Logger = LogHelper.Log4NetInstance.LogFactory(typeof(EventBus));
        }
        
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            //���¼�ӳ�伯�����ȡƥ�䵱ǰEventData(�¼�Դ����)��Handler
            var handlerTypes = _eventStore.GetHandlersForEvent(eventData.GetType()).ToList();
            if (handlerTypes.Count <= 0) return;
            //ѭ��ִ���¼�������
            foreach (var handlerType in handlerTypes)
            {
                var handlerInterface = handlerType.GetInterface("IEventHandler`1");
                //������Ҫ����Name����Resolve����Ϊע�����ʱ��ʹ����������ʽ(���ͬһ�¼���������¼������������)+
                var eventHandler = ContainerManager.Current.ResolveNamed(handlerType.Name, handlerInterface);
                if (eventHandler.GetType().FullName == handlerType.FullName)
                {
                    var handler = eventHandler as IEventHandler<TEventData>;
                    handler?.HandleEvent(eventData);
                }
            }
        }

        public void Trigger<TEventData>(Type eventHandlerType, TEventData eventData) where TEventData : IEventData
        {
            if (_eventStore.HasRegisterForEvent<TEventData>())
            {
                var handlers = _eventStore.GetHandlersForEvent<TEventData>();
                if (handlers.Any(th => th == eventHandlerType))
                {
                    //��ȡ����ʵ�ֵķ��ͽӿ�
                    var handlerInterface = eventHandlerType.GetInterface("IEventHandler`1");
                    var eventHandlers = ContainerManager.Current.Resolve(handlerInterface);
                    //ѭ������������������ʵ��������ӳ���ֵ����¼���������һ��ʱ���Ŵ����¼�
                    if (eventHandlers.GetType() == eventHandlerType)
                    {
                        var handler = eventHandlers as IEventHandler<TEventData>;
                        handler?.HandleEvent(eventData);
                    }
                }
            }
        }

        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return Task.Run(() => Trigger(eventData));
        }
        
        public Task TriggerAsync<TEventData>(Type eventHandlerType, TEventData eventData) where TEventData : IEventData
        {
            return Task.Run(() => Trigger(eventHandlerType, eventData));
        }

        public void UnRegister<TEventData>(Type handlerType) where TEventData : IEventData
        {
            _eventStore.RemoveRegister(typeof(TEventData), handlerType);
        }

        public void UnRegisterAll<TEventData>() where TEventData : IEventData
        {
            //��ȡ����ӳ���EventHandler
            var handlerTypes = _eventStore.GetHandlersForEvent(typeof(TEventData)).ToList();
            foreach (var handlerType in handlerTypes) _eventStore.RemoveRegister(typeof(TEventData), handlerType);
        }
    }
}