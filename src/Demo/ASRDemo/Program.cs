﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using IflySdk;
using IflySdk.Enum;
using IflySdk.Model.Common;
using IflySdk.Model.IAT;

namespace ASRDemo
{
    class Program
    {
        static void Main()
        {
            ASR();
            //ASRAudio();

            Console.ReadKey(false);
        }

        /// <summary>
        /// 实时识别，可以一边输入音频，一边进行语音识别。
        /// 整个会话时长最多持续60s，或者超过10s未发送数据服务器会自动断开连接。但是识别过程不会停止，会自动开启一个新的会话进行识别。
        /// 
        /// 分片大小建议至少在3000以上。小于1000就可能影响到识别速率（也取决于输入频率）。
        /// </summary>
        static async void ASR()
        {
            string path = @"02.pcm";  //测试文件路径,自己修改
            int frameSize = 3200;
            byte[] data = File.ReadAllBytes(path);

            try
            {
                ASRApi iat = new ApiBuilder()
                    .WithAppSettings(new AppSettings()
                    {
                        ApiKey = "7b845bf729c3eeb97be6de4d29e0b446",
                        ApiSecret = "50c591a9cde3b1ce14d201db9d793b01",
                        AppID = "5c56f257"
                    })
                    .UseError((sender, e) =>
                    {
                        Console.WriteLine("错误：" + e.Message);
                    })
                    .UseMessage((sender, e) =>
                    {
                        Console.WriteLine("实时结果：" + e);
                    })
                    .BuildASR();

                //计算识别时间
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < data.Length; i += frameSize)
                {
                    //模拟说话暂停
                    await Task.Delay(80);
                    iat.Convert(SubArray(data, i, frameSize));
                }
                //结束本次会话
                if (iat.Stop())
                {
                    Console.WriteLine("语音识别已结束...");
                }
                sw.Stop();
                Console.WriteLine($"总共花费{Math.Round(sw.Elapsed.TotalSeconds, 2)}秒。");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 一次识别一个完整的音频文件
        /// </summary>
        static async void ASRAudio()
        {
            string path = @"02.pcm";  //测试文件路径,自己修改
            byte[] data = File.ReadAllBytes(path);

            try
            {
                ASRApi iat = new ApiBuilder()
                    .WithAppSettings(new AppSettings()
                    {
                        ApiKey = "7b845bf729c3eeb97be6de4d29e0b446",
                        ApiSecret = "50c591a9cde3b1ce14d201db9d793b01",
                        AppID = "5c56f257"
                    })
                    .UseError((sender, e) =>
                    {
                        Console.WriteLine("错误：" + e.Message);
                    })
                    .UseMessage((sender, e) =>
                    {
                        Console.WriteLine("实时结果：" + e);
                    })
                    .BuildASR();

                //计算识别时间
                Stopwatch sw = new Stopwatch();
                sw.Start();

                ResultModel<string> result = await iat.ConvertAudio(data);
                if (result.Code == ResultCode.Success)
                {
                    Console.WriteLine("\n识别结果：" + result.Data);
                }
                else
                {
                    Console.WriteLine("\n识别错误：" + result.Message);
                }

                sw.Stop();
                Console.WriteLine($"总共花费{Math.Round(sw.Elapsed.TotalSeconds, 2)}秒。");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static byte[] SubArray(byte[] source, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex > source.Length || length < 0)
            {
                return null;
            }
            byte[] Destination;
            if (startIndex + length <= source.Length)
            {
                Destination = new byte[length];
                Array.Copy(source, startIndex, Destination, 0, length);
            }
            else
            {
                Destination = new byte[(source.Length - startIndex)];
                Array.Copy(source, startIndex, Destination, 0, source.Length - startIndex);
            }
            return Destination;
        }

    }
}


