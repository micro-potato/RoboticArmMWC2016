package com.MC
{

	import flash.events.MouseEvent;
	import flash.events.Event;
	import flash.net.SharedObject;
	import com.BaseMovie;
	import com.MC.MC_Config;

	import net.SocketEvent;
	import net.SocketServer;
	import flash.display.MovieClip;

	public class MC_Scene extends BaseMovie
	{

		private const shareName = "mwc165g";
		private var mc_Config:MC_Config;

		private var sock:SocketServer;
		protected var ip:String = "127.0.0.1";
		protected var port:uint = 6065;

		private var arr:Array;
		public function MC_Scene()
		{
			arr=new Array();
		}
		override protected function Init():void
		{
			super.Init();

			var share:SharedObject = SharedObject.getLocal(shareName);
			if (share.data.ip == undefined)
			{
				share.data.ip = '127.0.0.1';
				share.data.port = '6065';
			}

			share.flush(1000);
			this.ip = share.data.ip;
			this.port = uint(share.data.port);

			arr = [pattern_playhockey,pattern_drawing,pattern_dancing];

			NetInit();
			MCInit();

		}

		private function MCInit():void
		{
			for(var i:int=0;i<arr.length;i++){
				arr[i].buttonMode=true;
				arr[i].addEventListener(MouseEvent.CLICK,onClickHandler);
			}
			btn_config.addEventListener(MouseEvent.CLICK, on_config_click);
			btn_switch.stop();
			btn_switch.addEventListener(MouseEvent.CLICK, on_switch_click);
		}
		
		private function on_switch_click(e:MouseEvent)
		{
			if(btn_switch.currentFrame==1)
			{
				Send("forward:pause");
				btn_switch.gotoAndStop(2);
			}
			else
			{
				Send("forward:start");
				btn_switch.gotoAndStop(1);
			}
			
		}
		//*************************************************************配置页面*************************************************************
		private function on_config_click(e:MouseEvent):void
		{
			InitConfigScene();
		}

		public function InitConfigScene():void
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
				sock = new SocketServer(ip,port);
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
		private function on_Data_In(e:SocketEvent):void
		{
			var msg:String = e.message;
			trace("in:" + msg);

		}

		//Socket发送统一管理
		public function Send(data:String):void
		{
			if (sock)
			{
				trace("out:"+data+"|");
				sock.sendMessage(data+"|");
			}
		}

		private function onClickHandler(e:MouseEvent):void
		{
			var cmdtype:String=e.currentTarget.name.split('_')[0];
			var cmdarg:String=e.currentTarget.name.split('_')[1];
			Send(cmdtype+":"+cmdarg);
		}
	}
}