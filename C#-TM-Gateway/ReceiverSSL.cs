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
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Authentication;

namespace CSharp_TM_Gateway
{
	public class ReceiverSSL
	{
		private NetworkStream networkStream;
		private SslStream sslCo;
		private X509Certificate cert = null;
		private StreamWriter writer;
		private StreamReader reader;
		private SSLSender s;
		private TcpClient client = null;
		private Thread senderThread;
		
		public ReceiverSSL (TcpClient c)
		{
			client = c;
			s = new SSLSender(this);
			
			networkStream = client.GetStream();
			sslCo = new SslStream(networkStream, false);
			cert = new X509Certificate2("mycert.pfx", "123");
			sslCo.AuthenticateAsServer(cert, false, SslProtocols.Ssl3 , true);
			writer = new StreamWriter(sslCo);
			reader = new StreamReader(sslCo);
			writer.AutoFlush = true;
			senderThread = new Thread(startSender);
			senderThread.Start();
			this.send("C#-TM-Gateway v" + MainClass.version);
			this.send("Dogecoin Donation: " + MainClass.dogeaddress);
		}

		public void send(string msg)
		{
			writer.Write(msg + "\r\n");
		}
		
		public void recv()
		{
			string buffer = null;
			while((buffer = reader.ReadLine()) != null){
				if(!Command.CommandAct(writer, buffer)){
					s.send(buffer);
				}
				if(buffer.ToLower().StartsWith("quit")){
					break;
				}
			}
		}
		
		public bool isConnected()
		{
			return client.Connected;
		}
		
		public void die(){
			try {
				senderThread.Join();
				sslCo.Close();
				networkStream.Close();
				client.Close();
				s.die();
            }catch (SocketException) { }
		}
		
		public void startSender(){
			s.recv();
			this.die();
		}
		
		}
}

