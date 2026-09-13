using Networking.Dtos;
using UnityEngine;

namespace Networking
{
    public static class Session
    {
        public static UserResponse CurrentUser { get; set; }
        
        public static void Clear() => CurrentUser = null;

        //씬 로드보다 먼저 호출해서, static 값을 클리어해준다.(유니티 빠른실행을 위해서)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetStatics()
        {
            CurrentUser = null;
        }
    }
}