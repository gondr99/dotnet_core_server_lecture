using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreSystem.Util
{
    public static class TaskExtensions
    {
        public static async void Forget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}