
namespace XiaoLi.NET.UnifiedResult
{
    public interface IUnifiedResult
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; }
        
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; }
    }
}