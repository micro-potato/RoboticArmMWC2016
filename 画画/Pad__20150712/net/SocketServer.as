package net
{
	import flash.events.Event;
	import flash.events.EventDispatcher;
	import flash.events.IOErrorEvent;
	import flash.events.ProgressEvent;
	import flash.events.SecurityErrorEvent;
	import flash.events.TimerEvent;
	import flash.net.Socket;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.utils.ByteArray;
	import flash.utils.Timer;
	
	public class SocketServer extends EventDispatcher
	{
		private var _host:String;
		private var _port:int;
		private var _endCode:String='\n';
		private var messages:Array=new Array();
		
		private var urlLoader:URLLoader;
		public var isConnected:Boolean=false;
		private var _isAutoConnect:Boolean=false;
		
		private var socket:Socket;
		private var sendMessages:ByteArray=new ByteArray();
		private var timer:Timer = new Timer(3000);
		private var isControlClosed:Boolean = false;
		private var socIndex:int=0;
		private var isDispose:Boolean=false;//是否已经销毁此类
		public function SocketServer(ip:String,port:uint,isAutoConnect_:Boolean=true,socIndex_:int=-1)
		{
			_isAutoConnect=isAutoConnect_;
			socIndex=socIndex_;			
			connectServer(ip, port);
			super();
		}
		
		protected function timerHandle(event:TimerEvent):void
		{
			// TODO Auto-generated method stub
			//trace('timer');
			if(!isConnected){
				connectServer(_host, _port);
			}else {
				timer.stop();
				timer.reset();
			}
		}
		public function connectServer(host_:String, port_:int):void {
			if(isDispose){
				return;
			}
			trace('连接:' + host_+port_);
			isControlClosed = false;
			try{
				if(!socket){
					socket = new Socket();
					socket.timeout = 2000;
					socket.addEventListener(Event.CLOSE,closedHandle);
					socket.addEventListener(Event.CONNECT,connectedHandle);
					socket.addEventListener(ProgressEvent.SOCKET_DATA,receiveMessageHandle);
					socket.addEventListener(IOErrorEvent.IO_ERROR,errorHandle);
					socket.addEventListener(SecurityErrorEvent.SECURITY_ERROR,securityErrorHandle);
					timer.addEventListener(TimerEvent.TIMER,timerHandle);
				}
			}catch(error:Error){
				trace('connectError'+error.message);
			}
			_host=host_;
			_port = port_;
			if (socket &&　socket.connected) {
				socket.close();
			}
			socket.connect(_host,_port);
		}
		
		protected function securityErrorHandle(event:SecurityErrorEvent):void
		{
			// TODO Auto-generated method stub
			trace('安全问题');
		}
		//手动断开
		public function closed():void {
			trace("closed");
			isControlClosed = true;
			if (socket && socket.connected) {
				//sendMessage('close');
				socket.close();
			}
			timer.stop();
			timer.reset();
		}
		private function reConnect():void {
			if (_isAutoConnect && !isConnected) {
				if(!timer.running){
					timer.start();
				}
			}else{
				timer.reset();
				timer.stop();
			}
		}
		//了解出错
		protected function errorHandle(event:IOErrorEvent):void
		{
			// TODO Auto-generated method stub
			trace('连接错误');
			isConnected=false;
			if(!isControlClosed){
				reConnect();
			}
			isControlClosed=false;
			var socE:SocketEvent=new SocketEvent(SocketEvent.CONNECTED_FAILED);
			socE.socketIndex=socIndex;
			dispatchEvent(socE);
		}
		//接收到任何消息
		protected function receiveMessageHandle(event:ProgressEvent):void
		{
			// TODO Auto-generated method stub
			receiveMessage(socket.readUTFBytes(socket.bytesAvailable));
		}
		
		protected function connectedHandle(event:Event):void
		{
			trace('成功连接');
			isConnected = true;
			timer.stop();
			timer.reset();
			var socE:SocketEvent=new SocketEvent(SocketEvent.CONNECTED);
			socE.socketIndex=socIndex;
			dispatchEvent(socE);
		}
		
		protected function closedHandle(event:Event):void
		{
			trace("连接断开");
			isConnected = false;
			if(!isControlClosed){
				reConnect();
			}
			isControlClosed = false;
			var socE:SocketEvent=new SocketEvent(SocketEvent.CLOSED);
			socE.socketIndex=socIndex;
			dispatchEvent(socE);
		}
		
		private function receiveMessage(message_:String):void{
			
				var socE:SocketEvent=new SocketEvent(SocketEvent.RECEIVED_MESSAGE,message_);
				socE.socketIndex=socIndex;
				dispatchEvent(socE);
				//dispatchEvent(new SocketEvent(SocketEvent.RECEIVED_MESSAGE,tempMessage));
			
		}
		public function sendMessage(message_:String,endCode:String='\n'):void{
			try
			{
				if(socket.connected){
					_endCode=endCode;
					sendMessages.position=0;
					sendMessages.clear();
					sendMessages.writeUTFBytes(message_);
					socket.writeBytes(sendMessages);
					socket.flush();
				}
			} 
			catch(error:Error) 
			{
				trace("发送消息出错 "+error.message);
			}
		}
		public function sendByteMessage(message_:int):void{
			try
			{
				
				if(socket.connected){
					socket.writeByte(message_);
					socket.flush();
				}
			} 
			catch(error:Error) 
			{
				trace("发送消息出错 "+error.message);
			}
		}
		public function Dispose():void{
			isDispose=true;
			timer.removeEventListener(TimerEvent.TIMER,timerHandle);
			timer.stop();
			if(socket){
				if(socket.connected){
					socket.close();
				}
				socket.removeEventListener(Event.CLOSE,closedHandle);
				socket.removeEventListener(Event.CONNECT,connectedHandle);
				socket.removeEventListener(ProgressEvent.SOCKET_DATA,receiveMessageHandle);
				socket.removeEventListener(IOErrorEvent.IO_ERROR,errorHandle);
				socket.removeEventListener(SecurityErrorEvent.SECURITY_ERROR,securityErrorHandle);
			//	socket=null;
			}
		}
	}
}