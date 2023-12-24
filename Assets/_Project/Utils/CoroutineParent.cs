﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Utils.MonoBehaviours;
using UnityEngine;

namespace _Project.Utils
{
    public class CoroutineParent : SingletonBehaviour<CoroutineParent>
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsMap = new ();

        private readonly Dictionary<object, Action> _updaters = new();
        private readonly Dictionary<object, Coroutine> _actions = new();
        
        public static WaitForSeconds CachedWaiter(float seconds)
        {
            seconds = (float) Math.Round(seconds, 6);
            if (WaitForSecondsMap.TryGetValue(seconds, out var waitForSeconds))
                return waitForSeconds;

            waitForSeconds = new WaitForSeconds(seconds);
            WaitForSecondsMap[seconds] = waitForSeconds;
            return waitForSeconds;
        }

        public static void AddUpdater(object obj, Action updateAction) => Instance._updaters[obj] = updateAction;
        public static void RemoveUpdater(object obj) => Instance._updaters.Remove(obj);

        public static Coroutine InvokeAfterSecondsWithCanceling(object sender, float seconds, Action action)
        {
            if (Instance._actions.TryGetValue(sender, out var coroutine) && coroutine != null)
                Instance.StopCoroutine(coroutine);
            coroutine = InvokeAfterSeconds(seconds, action);
            Instance._actions[sender] = coroutine;
            return coroutine;
        }
        
        public static Coroutine InvokeAfterAsyncMethodWithCanceling<T>(object sender, Func<Task<T>> asyncAction, Action<Task<T>> action)
        {
            if (Instance._actions.TryGetValue(sender, out var coroutine) && coroutine != null)
                Instance.StopCoroutine(coroutine);
            coroutine = InvokeAfterAsyncMethod(asyncAction, action);
            Instance._actions[sender] = coroutine;
            return coroutine;
        }
        
        public static Coroutine InvokeAfterAsyncMethod<T>(Func<Task<T>> asyncAction, Action<Task<T>> action)
        {
            var task = asyncAction.Invoke();
            return Instance.StartCoroutine(WaitAndInvokeC(new WaitUntil(() => task.IsCompleted), () => action(task)));
        }

        public static Coroutine InvokeAfterSeconds(float seconds, Action action)
        {
            if (seconds < 0)
                return null;
            if (seconds <= Vector2.kEpsilon)
            {
                action?.Invoke();
                return null;
            }
            return Instance.StartCoroutine(WaitAndInvokeC(CachedWaiter(seconds), action));
        }

        public static Coroutine WaitAndInvoke(IEnumerator yieldInstruction, Action action)
        {
            return Instance.StartCoroutine(WaitAndInvokeC(yieldInstruction, action));
        }
        
        public static IEnumerator WaitAndInvokeC(YieldInstruction yieldInstruction, Action action)
        {
            yield return yieldInstruction;
            action?.Invoke();
        }       
        
        public static IEnumerator WaitAndInvokeC(IEnumerator yieldInstruction, Action action)
        {
            yield return yieldInstruction;
            action?.Invoke();
        }

        private void Update()
        {
            if (_updaters.Count == 0)
                return;

            foreach (var updater in _updaters.Values)
            {
                updater();
            }
        }
    }
}