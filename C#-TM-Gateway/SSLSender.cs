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
using System.Net.Security;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Starksoft.Net.Proxy;

namespace CSharp_TM_Gateway
{
	public class SSLSender
	{
		private ReceiverSSL r;
		private NetworkStream networkStream;
		private SslStream sslCo;
		private StreamWriter writer;
		private StreamReader reader;
		private TcpClient clientConn = null;
		
		public SSLSender (ReceiverSSL SSLrec)
		{
			r = SSLrec;
			//clientConn = new TcpClient(MainClass.host, 6697);
			Socks4aProxyClient proxy = new Socks4aProxyClient("127.0.0.1", 9050);
			clientConn = proxy.CreateConnection(MainClass.host, 6697);
			networkStream = clientConn.GetStream();
			sslCo = new SslStream(networkStream, false, new RemoteCertificateValidationCallback(trustCert));
			sslCo.AuthenticateAsClient(MainClass.host);
			writer = new StreamWriter(sslCo);
			reader = new StreamReader(sslCo);
			writer.AutoFlush = true;
		}
		public bool trustCert(object sender, X509Certificate cert, 
              X509Chain Certchain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
		public void recv()
		{
			string buffer = null;
			while((buffer = reader.ReadLine()) != null){
			    if(buffer.ToLower().Contains("\x1version\x1")){
					string[] nicks = buffer.Split('!');
					string nick = nicks[0];
					nick = nick.Replace(":", "");
					this.send("NOTICE " + nick + " :\x1VERSION \x2 C#-TM-Gateway\x2 v" + MainClass.version + '\x1');

				}else{
					r.send(buffer.ToString());
					//r.send(buffer);
				}
			}
		}
		
		public void send(string msg)
		{
			writer.Write(msg + "\r\n");
		}
		
		public bool isConnected()
		{
			return clientConn.Connected;
		}
		
		public void die(){
			clientConn.Close();
		}
	}
}

