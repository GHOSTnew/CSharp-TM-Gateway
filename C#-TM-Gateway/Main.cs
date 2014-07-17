/*
 * This file is part of C#-TM-Gateway.
 *
 *  C#-TM-Gateway is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  C#-TM-Gateway is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with C#-TM-Gateway.  If not, see <http://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;


/**
 * TO-DO:
 * - if user use Tor disconnect him
 * - comming soon
 **/
namespace CSharp_TM_Gateway
{
	class MainClass
	{
		public static string host = "oghzthm3fgvkh5wo.onion";
		public static string version = "0.1";
		public static string dogeaddress = "DMP3meY5fy2ydX45qyXoexw1oLKkSpJYbG";
		public static bool run = true;
		public static void Main (string[] args)
		{
			Console.WriteLine ("C#-TM-Gateway v" + version);
			Console.WriteLine("Starting at:" + DateTime.Now);
			Console.WriteLine("Dogecoin Donation: " + dogeaddress);
			
			/*
			 * TCP server
			 */
			Thread s1 = new Thread(new ThreadStart(delegate { OpenSock(6667, false);}));
			s1.Start();
			Thread s2 = new Thread(new ThreadStart(delegate { OpenSock(6668, false);}));
			s2.Start();
			Thread s3 = new Thread(new ThreadStart(delegate { OpenSock(6669, false);}));
			s3.Start();
			
			/*
			 * SSL listener 
			 */
			Thread s1SSL = new Thread(new ThreadStart(delegate { OpenSock(6697, true);}));
			s1SSL.Start();
			Thread s2SSL = new Thread(new ThreadStart(delegate { OpenSock(7000, true);}));
			s2SSL.Start();
			Thread s3SSL = new Thread(new ThreadStart(delegate { OpenSock(9999, true);}));
			s3SSL.Start();
		}
		
		public static void OpenSock(int port, bool ssl)
		{
			//Console.WriteLine(port);
			ThreadPool.SetMinThreads(10, 10); 
            ThreadPool.SetMaxThreads(10, 10); 
            TcpListener servSock = null; 
            try { 
                servSock = new TcpListener(IPAddress.Any, port); 
                servSock.Start(); 
                TcpClient tcpClient = null; 

                while (MainClass.run) {
                    tcpClient = servSock.AcceptTcpClient();
					ThreadPool.QueueUserWorkItem(RunService,  new DataClient { ClientSock = tcpClient, SSL = ssl});
                } 

            } catch (Exception ex) { 
                Console.WriteLine("Error : {0}", ex.Message); 

            } finally { 
                servSock.Stop(); 
            }

		}
		
		public static void RunService(Object infos)
		{
			DataClient dat = infos as DataClient;
			String reason = Security.IPisBanned(((IPEndPoint)dat.ClientSock.Client.LocalEndPoint).Address.ToString());
			if(reason != null){
				StreamWriter writer = new StreamWriter(dat.ClientSock.GetStream());
				writer.Write("ERROR :Closing link: (user@" + ((IPEndPoint)dat.ClientSock.Client.LocalEndPoint).Address.ToString() + ") [Error you are banned: " + reason + "]\r\n");
				writer.Flush();
				dat.ClientSock.Close();
			}else{
				if(dat.SSL){
					ReceiverSSL sslr = new ReceiverSSL(dat.ClientSock);
					sslr.recv();
					sslr.die();
				}else{
			        Receiver r = new Receiver(dat.ClientSock);
			        r.recv();
				    try {
						r.die();
					}catch(Exception){
					}
				}
			}
		}
	}
	internal class DataClient { 
		public TcpClient ClientSock { get; set; }
		public bool SSL { get; set; }
    } 
}