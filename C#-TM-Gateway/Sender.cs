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
using System.Net.Sockets;
using System.IO;

namespace CSharp_TM_Gateway
{
	public class Sender
	{
		private Receiver r;
		private NetworkStream networkStream;
		private StreamWriter writer;
		private StreamReader reader;
		private TcpClient clientConn = null;
		public Sender (Receiver rec)
		{
			r = rec;
			clientConn = new TcpClient(MainClass.host, 6667);
			networkStream = clientConn.GetStream();
			writer = new StreamWriter(networkStream);
			reader = new StreamReader(networkStream);
			writer.AutoFlush = true;
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
					r.send(buffer);
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

