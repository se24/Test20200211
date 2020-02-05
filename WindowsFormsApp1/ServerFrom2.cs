using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ServerFrom2 : Form
    {
        public ServerFrom2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        EventingBasicConsumer consumer;
        private static String QUEUE_NAME = "test_queue_work";

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
            //TempChannel.QueueDeclare(QUEUE_NAME, false, false, false, null);

            //// 绑定队列到交换机
            //TempChannel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "");

            //// 同一时刻服务器只会发一条消息给消费者
            ////TempChannel.BasicQos(1);



            ////TempChannel.QueueDeclare(queue: textQueueName.Text,// "rabbitmq_log",
            ////       durable: true,
            ////       exclusive: false,
            ////       autoDelete: false,
            ////       arguments: null);



            ///*
            //平衡调度 模式
            //    你可能注意到，目前的分发机制仍然不理想。例如，如果有两个工作者，当奇数的消息十分复杂，而偶数的消息很简单时，一个工作者就会非常繁忙，而另一个工作者几乎没有任务可做。但RabbitMQ不知道这种情况，仍然会平均分配任务。
            //    因为只要有消息进入队列，RabbitMQ就会分发消息。它不会检查每一个消费者未确认的消息个数。它只是盲目的将第N个消息发送给第N个消费者。
            //    为了防止这种情况，要使用basicQos 方法，并设置prefetchCount 参数的值为1。这样RabbitMQ就不会同时发送多个消息给同一个工作者。也就是说，在工作者处理并确认前一个消息之前，不会分发新的消息给工作者。它会把消息发送给下一个不忙的工作者。
            //*/
            //TempChannel.BasicQos(0, 1, false);

            ////绑定队列与
            ////TempChannel.QueueBind(textQueueName.Text, "fanout", txtRoutingKey.Text );

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
