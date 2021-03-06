﻿using System;
using System.Collections.Generic;
using CatLib.API.Network;
using CatLib.API.Buffer;

namespace CatLib.NetPackage
{
    
    /// <summary>
    /// CatLib Frame协议拆包器
    /// 协议格式为 总包长+包体，其中包长为4字节网络字节序的整数，包体可以是普通文本或者二进制数据。
    /// </summary>
    public class FramePacking : IPacking
    {

        [Dependency]
        public IBufferBuilder Buffer { get; set; }

        [Dependency]
        public IBufferBuilder EncodeBuffer { get; set; }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[][] Decode(byte[] bytes) {

            Buffer.Push(bytes);
            if(Buffer.Length < 4){ return null; }

            List<byte[]> package = null;
            int totalSize;
            byte[] bodyBuffer;
            while(Buffer.Length >= 4){

                totalSize = BitConverter.ToInt32(Buffer.Peek(4) , 0);
                if(totalSize > Buffer.Length){ break; }

                Buffer.Shift(4);
                bodyBuffer = Buffer.Shift(totalSize - 4);

                if(package == null){ package = new List<byte[]>(); }
                package.Add(bodyBuffer);

            }

            if(package == null){ return null; }

            return package.ToArray();

        }

        /// <summary>
        /// 封包
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public byte[] Encode(byte[] bytes){

            EncodeBuffer.Byte = bytes;
            EncodeBuffer.Unshift((EncodeBuffer.Length + 4).ToString().ToByte());
            return EncodeBuffer.Byte;

        }

        /// <summary>
        /// 清空缓存区
        /// </summary>
        public void Clear()
        {
            Buffer.Clear();
        }

    }
}
