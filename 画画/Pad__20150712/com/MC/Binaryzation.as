package com.MC
{
	import flash.display.BitmapData;

	public class Binaryzation
	{
		public static function toBinaryImg(bmd:BitmapData, threshold:uint = 128):void
		{
			var w:uint = bmd.width;
			var h:uint = bmd.height;
			for(var i:int = 0; i < w; i++)
			{
				for(var j:int = 0; j < h; j++)
				{
					var color:uint = bmd.getPixel(i, j);
					var r:int = color >> 16;
					var g:int = color >> 8 && 0xFF;
					var b:int = color & 0xFF;
					var gray:uint = r * 0.6 + g * 0.3 + b * 0.1;
					if(gray > threshold)
					{
						bmd.setPixel(i, j, 0xFFFFFF);
					}
					else
						bmd.setPixel(i, j, 0);
				}
			}
		}
	}
}