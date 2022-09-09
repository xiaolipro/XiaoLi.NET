
namespace XiaoLi.NET.UnifiedResult
{
    public class UnifiedResponse
    {
        public StatusCode Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }


        public UnifiedResponse():this(StatusCode.Success,"成功")
        {
            
        }
        public UnifiedResponse(StatusCode code, string message = "", object data = null)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}