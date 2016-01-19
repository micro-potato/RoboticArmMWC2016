package com 
{
	import com.MC.MC_Camera;
	import com.MC.MC_CameraComplete;
	import com.MC.MC_Config;
	import com.MC.MC_ImageGraphics;
	import com.Model.M_Data;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.net.SharedObject;
	import net.SocketEvent;
	import net.SocketServer;
	import flash.events.Event;
	import com.MC.MC_Config;
	import flash.display.StageDisplayState;
	import flash.display.StageScaleMode;
	
	/**
	 * ...
	 * @author ...
	 */
	public class Main extends BaseMovie
	{
		private const shareName = "Mechanic_iwinad_2015";
		private var mc_Config:MC_Config;
		
		//socket
		private var sock:SocketServer;
		protected var ip:String;
		protected var port:uint;
		
		private var scene:BaseMovie;
		public static var Instance:Main;
		public function Main() 
		{
			Instance = this;
		}
		
		override protected function Init():void 
		{
			super.Init();
			
			stage.displayState = StageDisplayState.FULL_SCREEN;
			stage.scaleMode = StageScaleMode.EXACT_FIT;
			
			//Net
			var share:SharedObject = SharedObject.getLocal(shareName);
			if (share.data.ip == undefined)
			{
				share.data.ip = '127.0.0.1';
				share.data.port = '8898';
			}
			
			share.flush(1000);
			this.ip = share.data.ip;
			this.port = uint(share.data.port);
			NetInit();
			
			MCInit();
			
			ChangeScene("ImageGraphics");
		}
		
		private function MCInit():void 
		{
			btn_config.addEventListener(MouseEvent.CLICK, on_config_click);
		}
		
		private function on_clear_click(e:MouseEvent):void 
		{
			//(mc_scene as MC_Scene).GraphicsClear();
			e.stopPropagation();
		}
		
		private function on_image_click(e:MouseEvent):void 
		{
			trace(M_Data.lineList2);
			//(mc_scene as MC_Scene).ImageTest(M_Data.lineList2);
			e.stopPropagation();
		}
		
		public function ChangeScene(sceneName:String):void 
		{
			RemoveScene();
			switch(sceneName)
			{
				case "ImageGraphics":
					scene = new MC_ImageGraphics();
					break;
				case "Camera":
					scene = new MC_Camera();
					break;
				case "CameraComplete":
					scene = new MC_CameraComplete();
					break;
			}
			addChild(scene);
			addChild(btn_config);
		}
		
		private function RemoveScene():void 
		{
			if (scene)
			{
				if (this.contains(scene))
				{
					this.removeChild(scene);
				}
			}
		}
		
		//*************************************************************配置页面*************************************************************
		private function on_config_click(e:MouseEvent):void 
		{
			InitConfigScene();
			e.stopPropagation();
		}
		
		private function InitConfigScene():void 
		{
			RemoveConfig();
			mc_Config = new MC_Config(shareName);
			mc_Config.addEventListener(mc_Config.OnCancelEvent, on_config_cancel);
			addChild(mc_Config);
		}
		
		private function on_config_cancel(e:Event):void 
		{
			if (mc_Config.IsChange)
			{
				var share:SharedObject = SharedObject.getLocal(shareName);
				this.ip = share.data.ip;
				this.port = uint(share.data.port);
				NetInit();
			}
			RemoveConfig();
		}
		
		private function RemoveConfig():void 
		{
			if (mc_Config)
			{
				mc_Config.removeEventListener(mc_Config.OnCancelEvent, on_config_cancel);
				if (this.contains(mc_Config))
				{
					removeChild(mc_Config);
				}
			}
		}
		//********************************************************************************************************************************************
		
		//***********************************************************************socket通讯***********************************************************
		//初始化通讯
		private function NetInit():void 
		{
			//NetDispose();
			if (sock == null)
			{
				sock = new SocketServer(ip, port);
				sock.addEventListener(SocketEvent.CONNECTED, on_connect);
				sock.addEventListener(SocketEvent.CLOSED, on_closed);
				//sock.addEventListener(SocketEvent.RECEIVED_MESSAGE, on_Data_In);
			}
			else
			{
				sock.connectServer(ip, port);
			}
		}
		
		//socket连线
		private function on_connect(e:SocketEvent):void
		{
			//do something
		}
		
		//socket断线
		private function on_closed(e:SocketEvent):void 
		{
			//do something
		}
		
		//socket信息进入
		/*private function on_Data_In(e:SocketEvent):void 
		{
			var msg:String = e.message;
			trace("in:" + msg);
			var dataArray:Array = msg.split("|");
			for each(var info in dataArray)
			{
				if (info != "")
				{
					var title:String = info.split(":")[0];
					var content:String = info.split(":")[1];
				}
			}
		}*/
		
		//Socket发送统一管理
		public function Send(data:String):void 
		{
			if (sock)
			{
				//trace("out:" + data);
				sock.sendMessage(data);
			}
		}
		//********************************************************************************************************************************************
		
		override protected function removed_from_stage(e:Event):void 
		{
			super.removed_from_stage(e);
		}
	}
}