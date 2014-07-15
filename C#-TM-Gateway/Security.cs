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
using System.IO;

namespace CSharp_TM_Gateway
{
	public class Security
	{
		public static string IPisBanned(string ip)
		{
			StreamReader streamread = null; 
            try {
				streamread = new StreamReader("BanList.txt");
				string l = streamread.ReadLine(); 
				do{
					string[] iptest = l.Split();
					if(iptest[0] == ip){
						string reason = "";
						for(int i = 1; i < iptest.Length; i++){
							if(i == 1) {
								reason = iptest[i];
							}else{
								reason += " " + iptest[i];
							}
						}
						return reason;
					}
					l = streamread.ReadLine();
                } while(l != null);
				return null;
			}catch(Exception) {
				return null;
			}
		}
		
	}
}

