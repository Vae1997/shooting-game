using System;
using System.Net;
using System.Net.Sockets;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
public class Serv
{
    //监听嵌套字
    public Socket listenfd;
    //客户端链接
    public Conn[] conns;
    //最大链接数
    public int maxConn = 50;
    //当前已连接数
    public int curPlayerCount = 0;
    //数据库
    MySqlConnection sqlConn;

    //获取链接池索引，返回负数表示获取失败
    public int NewIndex()
    {
        if (conns == null)
            return -1;
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
            {
                conns[i] = new Conn();
                return i;
            }
            else if (conns[i].isUse == false)
                return i;
        }
        return -1;
    }

    //开启服务器
    public void Start(string host, int port)
    {
        //连接mysql数据库
        string s = "Database=msgboard;Data Source=127.0.0.1;";
        s += "User Id=root;Password=227613zh;port=3306";
        sqlConn = new MySqlConnection(s);
        try
        {
            sqlConn.Open();
            Console.Write("[数据库]链接成功!\n");
        }
        catch (Exception e)
        {
            Console.Write("[数据库]链接失败" + e.Message + "\n");
            return;
        }
        //链接池
        conns = new Conn[maxConn];
        for (int i = 0; i < maxConn; i++)
            conns[i] = new Conn();
        //Socket
        listenfd = new Socket(AddressFamily.InterNetwork,
                              SocketType.Stream, ProtocolType.Tcp);
        //Bind
        IPAddress ipAdr = IPAddress.Parse(host);
        IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
        listenfd.Bind(ipEp);
        //Listen
        listenfd.Listen(maxConn);
        //Accept
        listenfd.BeginAccept(AcceptCb, null);
        Console.WriteLine("[服务器]启动成功!");
    }

    //Accept回调
    private void AcceptCb(IAsyncResult ar)
    {
        try
        {
            Socket socket = listenfd.EndAccept(ar);
            int index = NewIndex();
            if (index < 0)
            {
                socket.Close();
                Console.Write("[警告]服务器已满负载");
            }
            else
            {
                Conn conn = conns[index];
                conn.Init(socket);
                string adr = conn.GetAdress();
                Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                curPlayerCount++;
                Console.WriteLine("当前连接数：" + curPlayerCount);
                SendPlayerCount(curPlayerCount);
                conn.socket.BeginReceive(conn.readBuff,
                                         conn.buffCount, conn.BuffRemain(),
                                         SocketFlags.None, ReceiveCb, conn);
            }
            listenfd.BeginAccept(AcceptCb, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("AcceptCb失败:" + e.Message);
        }
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        Conn conn = (Conn)ar.AsyncState;
        try
        {
            int count = conn.socket.EndReceive(ar);
            //关闭信号
            if (count <= 0)
            {
                Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接");
                curPlayerCount--;
                Console.WriteLine("当前连接数：" + curPlayerCount);
                SendPlayerCount(curPlayerCount);
                conn.Close();
                return;
            }
            //数据处理
            string str = System.Text.Encoding.UTF8.GetString(conn.readBuff, 0, count);
            //Console.WriteLine("收到 [" + conn.GetAdress() + "] 数据：" + str);
            HandleMsg(conn, str);
            if (str == "Leave The Room......")
            {
                curPlayerCount--;
                Console.WriteLine("当前连接数：" + curPlayerCount);
                SendPlayerCount(curPlayerCount);
                SendMsg(conn, str);
                conn.Close();
                return;
            }
            else if(str.StartsWith("POS"))
            {
                SendPlayerPos(conn, str);
            }
            else if(str.StartsWith("DIRPOS"))
            {
                SendPlayerDIRPos(conn, str);
            }
            else if(str.StartsWith("BulletPOS"))
            {
                SendBulletPos(conn, str);
            }
            else if(str.StartsWith("LEAVE"))
            {
                curPlayerCount--;
                Console.WriteLine("当前连接数：" + curPlayerCount);
                SendPlayerCount(curPlayerCount);
                SendLeave(conn, str);
                conn.Close();
                return;
            }
            else
            {                
                SendMsg(conn, str);
            }
            //继续接收	
            conn.socket.BeginReceive(conn.readBuff,
                                     conn.buffCount, conn.BuffRemain(),
                                     SocketFlags.None, ReceiveCb, conn);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception e:"+e);
        }
    }

    public void HandleMsg(Conn conn, string str)
    {
        if (str == "Enter The Room......" || str.StartsWith("POS") || str.StartsWith("DIRPOS")
            || str.StartsWith("BulletPOS"))
            return;
        else if (str == "Leave The Room......" || str.StartsWith("LEAVE"))
        {
            //清空msg表
            string cmdStrFormat = "truncate table msg";
            string cmdStr = string.Format(cmdStrFormat);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据表msg]清空失败 " + e.Message);
            }
            return;
        }
        //获取数据
        /*else if (str == "_GET")
        {
            string cmdStr = "select * from msg order by id desc limit 10;";
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                str = "";
                while (dataReader.Read())
                {
                    str += dataReader["name"] + ":" + dataReader["msg"] + "\n\r";
                }
                dataReader.Close();
                byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
                conn.socket.Send(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库]查询失败 " + e.Message);
            }
        }*/
        //插入数据
        else
        {
            string cmdStrFormat = "insert into msg set name ='{0}' ,msg ='{1}';";
            string cmdStr = string.Format(cmdStrFormat, conn.GetAdress(), str);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("[数据库]插入失败 " + e.Message);
            }
        }
    }

    public void SendPlayerCount(int count)
    {
        byte[] bytes = System.Text.Encoding.Default.GetBytes("$"+count.ToString()+"$");
        //广播当前玩家数
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            //Console.WriteLine("playerCountTo: " + conns[i].GetAdress());
            conns[i].socket.Send(bytes);
        }
    }

    public void SendMsg(Conn conn ,string s)
    {
        if(!s.StartsWith("MSG"))
            s = conn.GetAdress() + ":" + s;
        else
        {
            string[] str = s.Split(' ');
            s = "MSG "+conn.GetAdress() + ":" + str[1];
        }
            
        byte[] msg = System.Text.Encoding.Default.GetBytes(s);
        //广播
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            //Console.WriteLine("MsgTo: " + conns[i].GetAdress());
            conns[i].socket.Send(msg);
        }
    }

    public void SendPlayerPos(Conn conn, string s)
    {
        byte[] POS = System.Text.Encoding.Default.GetBytes(s);
        //广播
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            //Console.WriteLine("PosTo: " + conns[i].GetAdress());
            conns[i].socket.Send(POS);
        }
    }

    public void SendBulletPos(Conn conn, string s)
    {
        byte[] POS = System.Text.Encoding.Default.GetBytes(s);
        //广播
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            //Console.WriteLine("BulletPosTo: " + conns[i].GetAdress());
            conns[i].socket.Send(POS);
        }
    }

    public void SendPlayerDIRPos(Conn conn, string s)
    {
        byte[] DIRPOS = System.Text.Encoding.Default.GetBytes(s);
        //广播
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            //Console.WriteLine("DirPosTo: " + conns[i].GetAdress());
            conns[i].socket.Send(DIRPOS);
        }
    }

    public void SendLeave(Conn conn, string s)
    {
        byte[] playerLEAVE = System.Text.Encoding.Default.GetBytes(s);
        //广播
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
                continue;
            if (!conns[i].isUse)
                continue;
            //Console.WriteLine("LEAVETo: " + conns[i].GetAdress());
            conns[i].socket.Send(playerLEAVE);
        }
    }
}
