﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.NET.App.Helpers
{
	public static partial class Helper
	{
        /// <summary>
        /// 将 Stream 转成 byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
		{
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);

			//  设置当前流的位置为流的开始 
			stream.Seek(0, SeekOrigin.Begin);
			return bytes;
		}

        /// <summary>
        /// 将 Stream 转成 byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> StreamToBytesAsync(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, bytes.Length);

            //  设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
		/// 将 byte[] 转成 Stream
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
