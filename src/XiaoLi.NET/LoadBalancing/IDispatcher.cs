using System.Threading.Tasks;

namespace XiaoLi.NET.LoadBalancing
{
    public interface IDispatcher
    {
        /// <summary>
        /// 根据服务名称获取调度后的真实主机
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<string> GetRealHostAsync(string serviceName);
    }
}