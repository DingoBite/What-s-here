using System;
using System.Collections.Generic;
using _Project.Utils.Extensions;
using UnityEngine;

namespace _Project.Utils
{
    public interface IMessageBusEvent<out TValue>
    {
        public event Action<TValue> OnMessageEvent;
    }

    public interface IMessageBusInvokable<in TValue>
    {
        public void Invoke(TValue value);
    }
    
    public interface IMessageBusEvent
    {
        public event Action OnMessageEvent;
    }

    public interface IMessageBusInvokable
    {
        public void Invoke();
    }
    
    public class MessageBus<TValue> : IMessageBusEvent<TValue>, IMessageBusInvokable<TValue>
    {
        public event Action<TValue> OnMessageEvent;

        public void Invoke(TValue value)
        {
            if (OnMessageEvent == null)
                Debug.LogWarning("Invoke empty event");
            else 
                OnMessageEvent.Invoke(value);
        }
    }

    public class MessageBus : IMessageBusEvent, IMessageBusInvokable
    {
        public event Action OnMessageEvent;
        
        public void Invoke()
        {
            if (OnMessageEvent == null)
                Debug.LogWarning("Invoke empty event");
            else 
                OnMessageEvent.Invoke();
        }
    }
    
    public class MessageRoute<TKey, TValue>
    {
        private readonly Dictionary<TKey, MessageBus<TValue>> _messageBuses = new ();

        public MessageBus<TValue> GetBus(TKey key) => _messageBuses.GetOrAddAndGet(key);
        public IMessageBusEvent<TValue> GetBusEvent(TKey key) => _messageBuses.GetOrAddAndGet(key);
        public IMessageBusInvokable<TValue> GetBusInvokable(TKey key) => _messageBuses.GetOrAddAndGet(key);

        public void Subscribe(TKey key, Action<TValue> action)
        {
            GetBus(key).OnMessageEvent += action;
        }

        public void Unsubscribe(TKey key, Action<TValue> action)
        {
            GetBus(key).OnMessageEvent -= action;
        }
    }
    
    public class MessageRoute<TKey>
    {
        private readonly Dictionary<TKey, MessageBus> _messageBuses = new ();

        public MessageBus GetBus(TKey key) => _messageBuses.GetOrAddAndGet(key);
        public IMessageBusEvent GetBusEvent(TKey key) => _messageBuses.GetOrAddAndGet(key);
        public IMessageBusInvokable GetBusInvokable(TKey key) => _messageBuses.GetOrAddAndGet(key);
    }

    public interface IMessagingRoot
    {
        MessageRoute<TKey, TValue> GetRoute<TKey, TValue>();
        MessageRoute<TKey> GetRoute<TKey>();
        public void Invoke<TKey, TValue>(TKey key, TValue value);
        public void Invoke<TKey>(TKey key);
        
        public void Subscribe<TKey, TValue>(TKey key, Action<TValue> action);
        public void Subscribe<TKey>(TKey key, Action action);
        public void Unsubscribe<TKey, TValue>(TKey key, Action<TValue> action);
        public void Unsubscribe<TKey>(TKey key, Action action);
    }

    public class MessagingRoot : IMessagingRoot
    {
        private static class MessageFork<TKey, TValue>
        {
            public static readonly Dictionary<object, MessageRoute<TKey, TValue>> MessageBuses = new ();
        }
        private static class MessageFork<TKey>
        {
            public static readonly Dictionary<object, MessageRoute<TKey>> MessageBuses = new ();
        }

        public MessageRoute<TKey, TValue> GetRoute<TKey, TValue>()
        {
            return MessageFork<TKey, TValue>.MessageBuses.GetOrAddAndGet(this);
        }

        public MessageRoute<TKey> GetRoute<TKey>()
        {
            return MessageFork<TKey>.MessageBuses.GetOrAddAndGet(this);
        }

        public void Invoke<TKey, TValue>(TKey key, TValue value)
        {
            GetRoute<TKey, TValue>().GetBusInvokable(key).Invoke(value);
        }
        
        public void Invoke<TKey>(TKey key)
        {
            GetRoute<TKey>().GetBusInvokable(key).Invoke();
        }
        
        public void Subscribe<TKey, TValue>(TKey key, Action<TValue> action)
        {
            GetRoute<TKey, TValue>().GetBusEvent(key).OnMessageEvent += action;
        }
        
        public void Subscribe<TKey>(TKey key, Action action)
        {
            GetRoute<TKey>().GetBusEvent(key).OnMessageEvent += action;
        }

        public void Unsubscribe<TKey, TValue>(TKey key, Action<TValue> action)
        {
            GetRoute<TKey, TValue>().GetBusEvent(key).OnMessageEvent -= action;
        }
        
        public void Unsubscribe<TKey>(TKey key, Action action)
        {
            GetRoute<TKey>().GetBusEvent(key).OnMessageEvent -= action;
        }
    }
}