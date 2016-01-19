package com.MC 
{
	import com.BaseMovie;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.net.SharedObject;
	/**
	 * ...
	 * @author ...
	 */
	public class MC_Config extends BaseMovie
	{
		public const OnCancelEvent:String = "OnCancelEvent";
		private var recordIP:String;
		private var recordPort:uint;
		private var shareName:String = "";
		public function MC_Config(_shareName:String) 
		{
			//this.recordIP = _recordIP;
			//this.recordPort = _recordPort;
			
			//write = new WriteFile(_filePath, _fileName);
			
			this.shareName = _shareName;
			var share:SharedObject = SharedObject.getLocal(this.shareName);
			this.recordIP = share.data.ip;
			this.recordPort = uint(share.data.port);
		}
		
		override protected function Init():void 
		{
			
			super.Init();
			
			this.x = 0;
			this.y = 0;
			
			txt_ip.text = recordIP;
			txt_port.text = String(recordPort);
			
			this.btn_close.addEventListener(MouseEvent.CLICK, on_close_click);
			this.btn_consume.addEventListener(MouseEvent.CLICK, on_consume_click);
		}
		
		private function on_close_click(e:MouseEvent):void 
		{
			IsChange = false;
			this.dispatchEvent(new Event(OnCancelEvent));
		}
		
		private function on_consume_click(e:MouseEvent):void 
		{
			IsChange = true;
			var share:SharedObject = SharedObject.getLocal(shareName);
			share.data.ip = txt_ip.text;
			share.data.port = txt_port.text;
			
			this.dispatchEvent(new Event(OnCancelEvent));
		}
		
		private var isChange:Boolean = false;
		public function get IsChange():Boolean
		{
			return isChange;
		}
		public function set IsChange(value:Boolean):void 
		{
			isChange = value;
		}
		
		
		public function get IP():String
		{
			return txt_ip.text;
		}
		public function set IP(value:String):void 
		{
			
		}
		
		public function get Port():uint
		{
			return uint(txt_port.text);
		}
		public function set Port(value:uint):void 
		{
			
		}
		
		override protected function removed_from_stage(e:Event):void 
		{
			super.removed_from_stage(e);
			
			this.btn_close.removeEventListener(MouseEvent.CLICK, on_close_click);
			this.btn_consume.removeEventListener(MouseEvent.CLICK, on_consume_click);
		}
		
	}

}