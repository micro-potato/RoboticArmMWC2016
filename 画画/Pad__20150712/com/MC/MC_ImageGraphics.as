package com.MC 
{
	import com.BaseMovie;
	import com.Main;
	import com.Model.M_Data;
	import flash.events.MouseEvent;
	import flash.events.Event;

	/**
	 * ...
	 * @author ...
	 */
	public class MC_ImageGraphics extends BaseMovie
	{
		private var pointsList:Vector.<String> = new Vector.<String>();
		
		public function MC_ImageGraphics() 
		{
			
		}
		
		override protected function Init():void 
		{
			super.Init();
			
			
			//按钮
			mc_draw.addEventListener(MouseEvent.MOUSE_DOWN, onMouseDownHandler); 
			mc_draw.addEventListener(MouseEvent.MOUSE_OUT, onMouseOut);
			stage.addEventListener(MouseEvent.MOUSE_OUT, onStateMouseOut);
			
			btn_clear.addEventListener(MouseEvent.CLICK, on_clear_click);
			btn_camera.addEventListener(MouseEvent.CLICK, on_camera_click);
		}
		
		private function onMouseDownHandler(e:MouseEvent):void
		{
			mc_draw.graphics.lineStyle(12,0X000000); 
			mc_draw.graphics.moveTo(mouseX, mouseY); 
			mc_draw.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMoveHandler); 
			mc_draw.addEventListener(MouseEvent.MOUSE_UP, onMouseUpHandler); 
			
			pointsList = new Vector.<String>();
			//
			//Send("move:mouseX="+mouseX+",mouseY="+mouseY);
			
			var mX = Math.round(mouseX * 100) / 100;
			var mY = Math.round(mouseY * 100) / 100;
			
			//暂时不发送,改为发送机械笔放下
			pointsList.push(mX.toString() + "," + mY.toString());
			
			
			//var data:String = "Down:" + mX.toString() + "," + mY.toString() + "|";
			//Send(data);
		}
		
		private function onMouseMoveHandler(e:MouseEvent):void
		{
			mc_draw.graphics.lineTo(mouseX, mouseY); 
			
			var mX = Math.round(mouseX * 100) / 100;
			var mY = Math.round(mouseY * 100) / 100;
			//Send("P:" + mX.toString() + "," + mY.toString() + "|");
			pointsList.push(mX.toString() + "," + mY.toString());
		}
		
		private function onMouseUpHandler(e:MouseEvent):void
		{
			mc_draw.graphics.lineTo(mouseX+1, mouseY+1); 
			mc_draw.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMoveHandler);
			mc_draw.removeEventListener(MouseEvent.MOUSE_UP, onMouseUpHandler); 
			SendPointsData();
		}
		
		private function onMouseOut(e:MouseEvent):void 
		{
			mc_draw.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMoveHandler);
			mc_draw.removeEventListener(MouseEvent.MOUSE_UP, onMouseUpHandler); 
			SendPointsData();
		}
		
		private function onStateMouseOut(e:MouseEvent):void 
		{
			mc_draw.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMoveHandler);
			mc_draw.removeEventListener(MouseEvent.MOUSE_UP, onMouseUpHandler); 
			SendPointsData();
		}
		
		private function SendPointsData():void 
		{
			//trace(pointsList);
			if (pointsList.length > 0)
			{
				var sendData:String = "Track:";
				for (var i:uint = 0; i < pointsList.length; i++)
				{
					sendData += pointsList[i];
					if (i < pointsList.length - 1)
					{
						sendData += "^";
					}
					else
					{
						sendData += "|";
					}
				}
				Main.Instance.Send(sendData);
			}
			pointsList = new Vector.<String>();
		}
		
		private function on_clear_click(e:MouseEvent):void 
		{
			mc_draw.graphics.clear();
		}
		
		private function on_camera_click(e:MouseEvent):void 
		{
			Main.Instance.ChangeScene("Camera");
		}

		override protected function removed_from_stage(e:Event):void 
		{
			super.removed_from_stage(e);
		}
	}
}