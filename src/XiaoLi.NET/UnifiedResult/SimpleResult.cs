namespace XiaoLi.NET.UnifiedResult
{
    public class SimpleResult:IUnifiedResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ExceptionResult : SimpleResult
    {
        /// <summary>
        /// 详细信息
        /// </summary>
        public string MessageDetails { get; set; }
    }
    
    public class 
}