using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetAsyn : MonoBehaviour
{
    //服务器IP和端口
    public InputField hostInput;
    public InputField portInput;
    //显示客户端收到的消息
    public Text recvText;
    public string recvStr;
    //显示客户端IP和端口
    public Text clientText;
    //Debug
    public Text debug;
    //count
    public Text playerCountText;
    public string countStr;
    //聊天输入框
    public InputField textInput;
    //canvas
    public GameObject canvas;
    //Socket和接收缓冲区
    public static Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    //玩家连接成功后进行第一次广播
    public bool firstSend = false;
    //玩家断开连接进行最后一次广播
    public bool endSend = false;
    //ip
    public static string id = "Player";

    //因为只有主线程能够修改UI组件属性
    //因此在Update里更换文本
    void Update()
    {
        recvText.text = recvStr;
        playerCountText.text = countStr;
    }
    //连接
    public void Connetion()
    {
        //清理text
        recvText.text = "";
        playerCountText.text = "";
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream, ProtocolType.Tcp);
        //Connect
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                debug.text += "\n当前客户端尚未连网,网络连接失败!\n"; 
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                debug.text += "\n当前客户端网络连接方式:4G\n该网络暂不支持连接\n";
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                debug.text += "\n当前客户端网络连接方式:wifi\n";
                break;
        }
        string hostName = Dns.GetHostName();   //获取客户端名
        debug.text += "当前客户端名:" + hostName + "\n";
        IPHostEntry localhost = Dns.GetHostByName(hostName);    //获取的地址 

        List<string> host = new List<string>();
        for (int i = 0; i < localhost.AddressList.Length; i++)
        {
            IPAddress localaddr = localhost.AddressList[i];
            if (!localaddr.ToString().StartsWith("fe") && !localaddr.ToString().StartsWith("2001"))
                debug.text += "客户端IP地址" + (i + 1) + ":" + localaddr + "\n";
            if (localaddr.ToString().StartsWith("192.168"))
                host.Add(localaddr.ToString());
        }
#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
        for (int i = 0; i < host.Count; i++)
        {
            string[] part = host[i].Split('.');
            if (part[3].Length == 1)
            {
                hostInput.text = host[i].Substring(0, localhost.AddressList[0].ToString().Length - 1) + "1";
                break;
            }
            if (part[3].Length == 2)
            {
                hostInput.text = "192.168.43.82";
            }
        }
#endif
        try
        {
            //和服务器在同一局域网内
            socket.Connect("192.168.43.82", int.Parse(portInput.text));
            string[] ip = socket.LocalEndPoint.ToString().Split(':');
            id = ip[1];
            clientText.text = "你的IP地址 " + socket.LocalEndPoint.ToString();
            recvStr = "你已经进入房间,和大家打声招呼吧！\n";
            firstSend = true;
            Send();
            //Recv
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
    //接收回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            //count是接收数据的大小
            int count = socket.EndReceive(ar);
            //关闭信号
            if (count <= 0)
            {
                socket.Close();
                return;
            }
            //数据处理
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            //Debug.Log("str：" + str);
            //服务器发送的玩家数信息
            if (str.StartsWith("$"))
                countStr = str.Substring(1, 1);
            //服务器发送的位置信息
            else if(str.StartsWith("POS") || str.StartsWith("LEAVE") || str.StartsWith("DIRPOS") || str.StartsWith("BulletPOS"))
                MultiLevel.msgList.Add(str);
            else if(str.Contains("Enter The Room......") || str.StartsWith("MSG") || str.Contains("Leave The Room......"))
            {
                if (recvStr.Length > 300) recvStr = "";
                if (str.StartsWith("MSG"))
                {
                    string[] s = str.Split(' ');
                    recvStr += s[1] + "\n";
                }
                else
                    recvStr += str + "\n";
            }
            //继续接收	
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        } 
    }
    //发送数据
    public void Send()
    {
        byte[] bytes;
        if (firstSend)
        {
            firstSend = false;
            bytes = System.Text.Encoding.Default.GetBytes("Enter The Room......");
        }
        else if(endSend)
        {
            endSend = false;
            bytes = System.Text.Encoding.Default.GetBytes("Leave The Room......");
        }
        else
        {
            bytes = System.Text.Encoding.Default.GetBytes("MSG " + textInput.text);
        }
            
        try
        {
            socket.Send(bytes);
        }
        catch { }
    }
    //当在房间中退出时，断开连接
    public void Quit()
    {
        //清理recvStr
        recvStr = "";
        countStr = "";
        id = "Player";
        debug.text = "debug:";
        endSend = true;
        Send();
    }
}
