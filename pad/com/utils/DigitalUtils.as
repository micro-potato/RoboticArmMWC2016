package com.utils
{
	public class DigitalUtils
	{
		public function DigitalUtils(){
			
		}
		public static function Digital(str:String):Array
		{
			if (str.substr(0,1) == "$")
			{
				var arr:Array = str.substr(1).split(",");
				var newStr:String = "";
				for (var i:int=0; i<arr.length; i++)
				{
					newStr +=  arr[i];
				}
				if (newStr.length >= 4 && newStr.length < 7)
				{
					return [int(newStr),"$"+int(int(newStr)/1000)+","+newStr.substr(-3)];
				}
				else if (newStr.length>=7&&newStr.length<10)
				{
					return [int(newStr),"$"+int(int(newStr)/1000000)+","+newStr.substr(-6).substr(0,3)+","+newStr.substr(-3)];
				}
				else if (newStr.length>=10&&newStr.length<13)
				{
					return [int(newStr),"$"+int(int(newStr)/1000000000)+","+newStr.substr(-9).substr(0,3)+","+newStr.substr(-6).substr(0,3)+","+newStr.substr(-3)];
				}
				else
				{
					return [int(newStr),"$"+int(newStr)];
				}
			}
			else
			{
				var arr:Array = str.split(",");
				var newStr:String = "";
				for (var i:int=0; i<arr.length; i++)
				{
					newStr +=  arr[i];
				}
				if (newStr.length > 3 && newStr.length < 7)
				{
					return [int(newStr),int(int(newStr)/1000)+","+newStr.substr(-3)];
				}
				else if (newStr.length>=7&&newStr.length<10)
				{
					return [int(newStr),int(int(newStr)/1000000)+","+newStr.substr(-6).substr(0,3)+","+newStr.substr(-3)];
				}
				else if (newStr.length>=10&&newStr.length<13)
				{
					return [int(newStr),"$"+int(int(newStr)/1000000000)+","+newStr.substr(-9).substr(0,3)+","+newStr.substr(-6).substr(0,3)+","+newStr.substr(-3)];
				}
				else
				{
					return [int(newStr),newStr];
				}
			}
			return [0,0];
		}
	}
}