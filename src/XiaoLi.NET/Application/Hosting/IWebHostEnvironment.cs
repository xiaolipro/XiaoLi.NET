using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace XiaoLi.NET.Application.Hosting
{
    public interface IWebHostEnvironment:IHostEnvironment
    {
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }
}