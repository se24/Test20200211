
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ServerFrom : Form
    {
        public ServerFrom()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        EventingBasicConsumer consumer;
        private static String QUEUE_NAME = "test_queue_work2";

        private static String EXCHANGE_NAME = "test_exchange_fanout";
        private void button1_Click(object sender, EventArgs e)
        {
            Test();
            ////1）创建与服务器之间的连接
            //String TempRes = "";
            //String TempHostName = txtHostName.Text;
            //String TempUserName = txtUserName.Text;
            //String TempPassword = txtPassword.Text;
            //MQContion TempMQContion = new MQContion();
            //IConnection TempConnection = TempMQContion.Connection(TempHostName, TempUserName, TempPassword, ref TempRes);
            //if (TempRes != "OK")
            //{
            //    MessageBox.Show(TempRes);
            //}

            //var TempChannel = TempConnection.CreateModel();
            //TempChannel.QueueDeclare(QUEUE_NAME, true, false, false, null);
            //TempChannel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "");
            //TempChannel.BasicQos(0, 1, false);
            ////2)从消息队列中订阅消息 
            //consumer = new EventingBasicConsumer(TempChannel);
            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body;
            //    var message = Encoding.UTF8.GetString(body);//这里是我们订阅到的消息

            //    txtLog.AppendText(message);
            //    txtLog.AppendText("\r\n");

            //    // 手动发送消息确认信号。消息从队列中删除, 如果 [TAGA] 处代码 第二个参数为true那么这行代码就可以不要了.一般为了系统稳定都需要手工应答确保任务成功完成,再从队列中删除消息.
            //    TempChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            //};
            ////TAGA 
            //TempChannel.BasicConsume(textQueueName.Text, false, consumer);
        }


        public void Test()
        {
            try
            { //(1）创建与服务器之间的连接
                String TempRes = "";
                String TempHostName = txtHostName.Text;
                String TempUserName = txtUserName.Text;
                String TempPassword = txtPassword.Text;
                MQContion TempMQContion = new MQContion();
                IConnection TempConnection = TempMQContion.Connection(TempHostName, TempUserName, TempPassword, ref TempRes);
                if (TempRes != "OK")
                {
                    MessageBox.Show(TempRes);
                }
                //从连接中获取一个通道
                var TempChannel = TempConnection.CreateModel();
                //声明交换机（分发:发布/订阅模式）
                TempChannel.ExchangeDeclare(EXCHANGE_NAME, "fanout");
                //声明队列
                TempChannel.QueueDeclare(QUEUE_NAME, true, false, false, null);
                //将队列绑定到交换机
                TempChannel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "");
                //保证一次只分发一个  
                TempChannel.BasicQos(0, 1, false);
                consumer = new EventingBasicConsumer(TempChannel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);//这里是我们订阅到的消息

                    txtLog.AppendText(message);
                    txtLog.AppendText("\r\n");

                    // 手动发送消息确认信号。消息从队列中删除, 如果 [TAGA] 处代码 第二个参数为true那么这行代码就可以不要了.一般为了系统稳定都需要手工应答确保任务成功完成,再从队列中删除消息.
                    TempChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                };
                //TAGA 
                TempChannel.BasicConsume(QUEUE_NAME, false, consumer);
            }
            catch (Exception TempE)
            {
                String TempR = TempE.Message;
            }
        }
     
    }
}

