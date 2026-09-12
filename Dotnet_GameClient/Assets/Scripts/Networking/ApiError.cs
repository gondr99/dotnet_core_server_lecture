using System.Collections.Generic;

namespace Networking
{
    public class ApiError
    {
        public string Message { get; set; }
        public int Status { get; set; }
        
        
        public Dictionary<string, string[]> Errors { get; set; } //key - field이름, value: 에러메시지가 되는거야
        public string Raw { get; set; } //서버가 준 Raw 스트링 

        public string ToUserMessage()
        {
            if (Errors != null)
            {
                foreach (KeyValuePair<string, string[]> kv in Errors)
                {
                    if(kv.Value != null && kv.Value.Length > 0)
                        return kv.Value[0];//첫번째 에러만 보내준다.
                }
            }
            
            return string.IsNullOrEmpty(Message) ? "오류가 발생했습니다" : Message;
        }
        
    }
}