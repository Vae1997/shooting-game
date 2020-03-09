using System;
using System.Net;

class MainClass
{
    public static void Main(string[] args)
    {
        Console.WriteLine("服务器启动中,请稍后......");
        string hostName = Dns.GetHostName();   //获取本机名
        Console.WriteLine("服务器主机名:" + hostName);
        IPHostEntry localhost = Dns.GetHostEntry(hostName);//Dns.GetHostByName(hostName);    //获取所有IPv4的地址 
        for(int i = 0; i < localhost.AddressList.Length; i++)
        {
            IPAddress localaddr = localhost.AddressList[i];
            Console.WriteLine("服务器地址" +(i+1)+":"+ localaddr);
        }
        Serv serv = new Serv();
        for (int i = 0; i < localhost.AddressList.Length; i++)
        {
            IPAddress localaddr = localhost.AddressList[i];
            if(localaddr.ToString().StartsWith("192.168"))
            //选择一个在同一个局域网的ip地址做为服务器端地址，端口指定9001
                serv.Start(localaddr.ToString(), 9001);
        }
        Console.WriteLine("服务器运行中,输入quit命令关闭服务器...");
        
        while (true)
        {
            string str = Console.ReadLine();
            switch (str)
            {
                case "quit":
                    return;
            }
        }
    }
}
