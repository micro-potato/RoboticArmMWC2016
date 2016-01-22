package net
{
	import flash.events.Event;
	import flash.events.EventDispatcher;
	
	/**
	 * 
	 * @author 陈天柱
	 * 接受到服务器发过来的消息时派发此事件
	 * message 是服务器发送过来的消息
	 * 
	 */
	public class SocketEvent extends Event
	{
		public static const RECEIVED_MESSAGE:String="received_message";
		public static const CONNECTED:String = 'connected';
		public static const CLOSED:String = 'closed';
		public static const CONNECTED_FAILED:String='connected_failed';
		public var socketIndex:int=-1;
		private var _message:String='';//接收到的消息
		public function SocketEvent(_type:String,_message:String='', bubbles:Boolean=false, cancelable:Boolean=false)
		{
			message=_message;
			super(_type, bubbles, cancelable);
		}

		public function get message():String
		{
			return _message;
		}

		public function set message(value:String):void
		{
			_message = value;
		}
		public function sendReceiveMessage(sendObject_:EventDispatcher,message_:String):void{
			//sendObject_.dispatchEvent(
		}

	}
}