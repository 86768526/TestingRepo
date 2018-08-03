using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace SerialPortTest
{
    public partial class Form1 : Form
    {
        private static int cacheSize = 1024; //缓存文件大小
        SerialPort sp1 = new SerialPort();
        Boolean issending = false;
        /// <summary>
        /// 初始化窗体
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }
            //添加串口项目  
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {//获取有多少个COM口  
                cbSerial.Items.Add(s);
            }

        }




        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void btnSwitch_Click(object sender, EventArgs e)
        {
            dealwithswitch();
        }
        private void send_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                if (issending == false)
                {
                    dealwithsend();
                    timer.Start();
                    send.Text = "stopsending";
                    issending = true;
                }
                else
                {
                    timer.Stop();
                    send.Text = "sending";
                    issending = false;
                }
            }
            else
            {
                dealwithsend();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            txtReceive.Clear();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            dealwithsend();
        }

        /// <summary>
        /// 串口开关方法
        /// </summary>
        ///         
        private void dealwithswitch()
        {
            if (sp1.IsOpen == false)
            {
                try
                {
                    //设置串口号  
                    string serialName = cbSerial.SelectedItem.ToString();
                    sp1.PortName = serialName;

                    //设置各“串口设置”  
                    string strBaudRate = cbBaudRate.Text;
                    string strDateBits = cbDataBits.Text;
                    Int32 iBaudRate = Convert.ToInt32(strBaudRate);
                    Int32 iDateBits = Convert.ToInt32(strDateBits);

                    sp1.BaudRate = iBaudRate;       //波特率  
                    sp1.DataBits = iDateBits;       //数据位  

                    switch (cbStop.Text)            //停止位  
                    {
                        case "1":
                            sp1.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            sp1.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            sp1.StopBits = StopBits.Two;
                            break;
                        default:
                            MessageBox.Show("Error：参数不正确!", "Error");
                            break;
                    }
                    switch (cbParity.Text)             //校验位  
                    {
                        case "无":
                            sp1.Parity = Parity.None;
                            break;
                        case "奇校验":
                            sp1.Parity = Parity.Odd;
                            break;
                        case "偶校验":
                            sp1.Parity = Parity.Even;
                            break;
                        default:
                            MessageBox.Show("Error：参数不正确!", "Error");
                            cbSerial.Enabled = true;
                            cbBaudRate.Enabled = true;
                            cbDataBits.Enabled = true;
                            cbStop.Enabled = true;
                            cbParity.Enabled = true;
                            checkBox1.Enabled = false;
                            break;
                    }
                    if (sp1.IsOpen == true)//如果打开状态，则先关闭一下  
                    {
                        sp1.Close();
                    }
                    //状态栏设置  
                    tsSpNum.Text = "串口号：" + sp1.PortName + "|";
                    tsBaudRate.Text = "波特率：" + sp1.BaudRate + "|";
                    tsDataBits.Text = "数据位：" + sp1.DataBits + "|";
                    tsStopBits.Text = "停止位：" + sp1.StopBits + "|";
                    tsParity.Text = "校验位：" + sp1.Parity + "|";

                    //设置必要控件不可用  
                    cbSerial.Enabled = false;
                    cbBaudRate.Enabled = false;
                    cbDataBits.Enabled = false;
                    cbStop.Enabled = false;
                    cbParity.Enabled = false;
                    checkBox1.Enabled = true;
                    sp1.Open();     //打开串口  
                    btnSwitch.Text = "关闭串口";
                    Control.CheckForIllegalCrossThreadCalls = false;
                    sp1.DataReceived += new SerialDataReceivedEventHandler(sp1_DataReceived);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message, "Error");
                    cbSerial.Enabled = true;
                    cbBaudRate.Enabled = true;
                    cbDataBits.Enabled = true;
                    cbStop.Enabled = true;
                    cbParity.Enabled = true;
                    checkBox1.Enabled = false;
                }
            }
            else
            {
                //状态栏设置  
                tsSpNum.Text = "串口号：未指定|";
                tsBaudRate.Text = "波特率：未指定|";
                tsDataBits.Text = "数据位：未指定|";
                tsStopBits.Text = "停止位：未指定|";
                tsParity.Text = "校验位：未指定|";
                //恢复控件功能  
                //设置必要控件不可用  
                cbSerial.Enabled = true;
                cbBaudRate.Enabled = true;
                cbDataBits.Enabled = true;
                cbStop.Enabled = true;
                cbParity.Enabled = true;
                checkBox1.Enabled = false;

                sp1.Close();    //关闭串口  
                btnSwitch.Text = "打开串口";
            }
        }

        /// <summary>
        /// 发送方法
        /// </summary>
        private void dealwithsend()
        {
            if (!sp1.IsOpen) //如果没打开  
            {
                MessageBox.Show("请先打开串口！", "Error");
                return;
            }

            String strSend = txtSend.Text;
            //以字符串形式发送时   
            sp1.WriteLine(txtSend.Text);    //写入数据  
            if (checkBox1.Checked == false)
            {
                txtSend.Clear();
            }
        }




        /// <summary>
        /// /接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sp1.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了  
            {
                try
                {
                    txtReceive.Text += "【" + DateTime.Now.ToString(" hh:mm:ss.fff") + "】:" + sp1.ReadLine() + "\r\n";
                    save_file("【" + DateTime.Now.ToString(" hh:mm:ss.fff") + "】:" + sp1.ReadLine() + "\r\n");
                    sp1.DiscardInBuffer();
                    //Byte[] receivedData = new Byte[sp1.BytesToRead];        //创建接收字节数组  
                    //sp1.Read(receivedData, 0, receivedData.Length);         //读取数据                         
                    //sp1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer  
                    //string strRcv = null;
                    //if (checkBox2.Checked)
                    //{
                    //    for (int i = 0; i < receivedData.Length; i++) //窗体显示  
                    //    {
                    //        strRcv += receivedData[i].ToString("X2");  //16进制显示  
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < receivedData.Length; i++) //窗体显示  
                    //    {
                    //        strRcv += receivedData[i].ToString();  //16进制显示  
                    //    }
                    //}
                    //txtReceive.Text += strRcv + "\r\n";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "出错提示");
                    txtSend.Text = "";
                }
            }
            else
            {
                MessageBox.Show("请打开某个串口", "错误提示");
            }
        }




        /// <summary>
        /// 缓存文件
        /// </summary>
        /// 
        int fileNo = 1;  //缓存文件序号
        string Dirpath = @"D:\cachefiledir\"; //缓存目录
        Boolean ArriveContinueSpot = false; // 到达续写点标志

        private void save_file(string str) 
        {
            string cachefilepath = string.Format("{0}" + "{1}", Dirpath, fileNo);
            ///
            if (!File.Exists(cachefilepath)) //判断缓存文件是否存在
            {
                try
                {
                    ArriveContinueSpot = true;//标志 续写点

                    FileStream fs = File.Create(cachefilepath);

                    StreamWriter cachewriter = new StreamWriter(fs);

                    cachewriter.Write(str);

                    cachewriter.Close();

                    fs.Close();

                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString().Trim());
                }
            }
 
            ///文件已存在
            else
            {
                while(cacheTansferCompelete(fileNo)) //判断缓存文件是否写满
                {
                    cachefilepath = string.Format(Dirpath + "{0}", ++fileNo);

                    if (ArriveContinueSpot)  //判断到达 续写点；
                    {
                        try
                        {
                            FileStream fs = File.Create(cachefilepath);

                            fs.Close();

                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show(ee.ToString().Trim());
                        }
                    }
                }
       
                    ///若未写满
                    try
                    {
                        ArriveContinueSpot = true;//标志续写点

                        StreamWriter cachewriter = File.AppendText(cachefilepath);

                        cachewriter.Write(str);

                        cachewriter.Close();

                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.ToString().Trim());
                    }
                
            }
        }

        /// 缓存文件可写判断
        /// <param name="No">缓存文件文件名（序列号） </param>
        /// <returns></returns>
        private Boolean cacheTansferCompelete(int No) //判断缓存文件写满方法
        {
            string p = string.Format("{0}" + "{1}", Dirpath, No);
            if (File.Exists(p))
            {
                FileInfo fileinfo = new FileInfo(p);
                if (fileinfo.Length < cacheSize)
                {
                    return false;
                }                               
                else
                {
                    return true;
                }
            }
            else
            {
                FileStream fs = File.Create(p);
                fs.Close();
                return false;
            }
        }
    }
}

