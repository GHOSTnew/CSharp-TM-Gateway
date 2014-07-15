using System;
using System.IO;

namespace CSharp_TM_Gateway
{
	public class Command
	{
		public static bool CommandAct(StreamWriter st, string command)
		{
			string buffer = command.Replace("\r", "").Replace("\n", "").ToLower();
			if(buffer.Equals("bitcoin")){
			    st.Write("Our Bitcoin address is: 19wzjCe4m6YiiAheWHniA4tABkX98yrWqT\r\n");
			    return true;
			}else if(buffer.Equals("litecoin")){
				st.Write("Our Litecoin address is: 1LQEYsKhXdXMYBjLZCKkpBgdL2oExfhm9wP\r\n");
			    return true;
			}else if(buffer.Equals("anoncoin")){
				st.Write("Our Anoncoin address is: AGYkqtqpkEoC3io4YPncC2CKpet1819e29\r\n");
			    return true;
			}else{
				return false;
			}
		}
	}
}

