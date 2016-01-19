package com.MC 
{
	import com.BaseMovie;
	import com.Main;
	import com.Model.M_Data;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.ui.Mouse;
	/**
	 * ...
	 * @author ...
	 */
	public class MC_CameraComplete extends BaseMovie
	{
		private var iniX:uint = 300;
		private var iniY:uint = 300;
		private var sendData:String = "";
		public function MC_CameraComplete() 
		{
			
		}
		
		override protected function Init():void 
		{
			super.Init();
			
			btn_draw.addEventListener(MouseEvent.CLICK, on_draw_click);
			btn_retry.addEventListener(MouseEvent.CLICK, on_retry_click);
			btn_back.addEventListener(MouseEvent.CLICK, on_back_click);
			
			if (M_Data.line_Array != null)
			{
				DrawImage(M_Data.line_Array);
			}
		}
		
		private function on_draw_click(e:MouseEvent):void 
		{
			//机械臂绘制
			Main.Instance.Send(sendData);
		}
		private function on_retry_click(e:MouseEvent):void 
		{
			Main.Instance.ChangeScene("Camera");
		}
		private function on_back_click(e:MouseEvent):void 
		{
			Main.Instance.ChangeScene("ImageGraphics");
		}
		
		public function DrawImage(list:Array):void 
		{
			//trace(list);
			var count :uint = 0;
			//trace(list.length);
			for (var i:uint = 0; i < list.length; i++)
			{
				sendData += "Track:";
				for (var j:uint = 0; j < (list[i] as Array).length; j++)
				{
					var mx:Number = (list[i] as Array)[j][0] * 3;
					var my:Number = (list[i] as Array)[j][1] * 3;

					if (j == 0)
					{
						mc_draw.graphics.lineStyle(8); 
						mc_draw.graphics.moveTo(mx+iniX, my+iniY); 
					}
					else
					{
						mc_draw.graphics.lineTo(mx+iniX, my+iniY); 
					}
					
					sendData += ((mx * 1.5) + 100).toFixed(0).toString() + "," + ((my * 1.5) + 100).toFixed(0).toString();
					
					if (j < (list[i] as Array).length - 1)
					{
						sendData += "^";
					}
					else
					{
						sendData += "|";
					}
				}
			}
		}
		
		override protected function removed_from_stage(e:Event):void 
		{
			super.removed_from_stage(e);
			btn_draw.addEventListener(MouseEvent.CLICK, on_draw_click);
			btn_retry.addEventListener(MouseEvent.CLICK, on_retry_click);
			btn_back.addEventListener(MouseEvent.CLICK, on_back_click);
		}
		
		
	}

}