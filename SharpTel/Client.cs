using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Data.SqlClient;

namespace SharpTel{
    class Client{

        StreamReader input = null;
        

        public Client(StreamReader input){
            this.input = input;
        }

        public void Run(){
            String line;

            while ((line=input.ReadLine())!=null){
                Conexion(line);
                Console.Write(line+"\r\n");
            }
        }

        void Conexion(string ingresar)
        {
            string connetionString;
            SqlConnection cnn;
            connetionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=SMRD;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            //Console.WriteLine("DATABASE connection open");

            String sql = "INSERT INTO [dbo].[Registros] (registro) VALUES ( '" + ingresar.Replace("'","''") + "')";
            using (SqlCommand command = new SqlCommand(sql, cnn))
            {
                command.ExecuteNonQuery();
                //Console.WriteLine("Done.");
            }

            cnn.Close();
            //Console.WriteLine("DATABASE connection close");
        }

        static void Main(string[] args){
                Console.Write("Conecting to Host: ");
                //string host=Console.ReadLine();
                string host="10.0.0.1" +
                "";
                Console.Write("Conecting to port: ");
                //int port;
                //try{
                //    port=int.Parse(Console.ReadLine());
                //}
                //catch(Exception){
                //    Console.WriteLine("Wrong parameter. Using 23 instead");
                //    port=23;
                //}
                int port=2300;
                TcpClient socket=null;
                try{
                    socket = new TcpClient(host, port);
                    Console.Write("Successful connection to PBX: ");
                 }
                catch(SocketException){
                    Console.WriteLine("Unknown host - " + host + ". Quitting");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                NetworkStream stream = socket.GetStream();
                StreamWriter output = new StreamWriter(stream);
                StreamReader input = new StreamReader(stream);

                Client cliobj = new Client(input);
                Thread t = new Thread(new ThreadStart(cliobj.Run));
                t.Start();

                int login = 0;
                while(true){

                switch (login)
                {
                    case 0:
                        login = login + 1;
                        output.Write("SMDR" + "\r\n");
                        output.Flush();
                        break;
                    case 1:
                        login = login + 1;
                        output.Write("PCCSMDR" + "\r\n");
                        output.Flush();
                        break;
                    default:
                        string salida = Console.ReadLine();
                        output.Write( salida + "\r\n");
                        output.Flush();
                        
                        break;
                }

                }


        }




    }
}
