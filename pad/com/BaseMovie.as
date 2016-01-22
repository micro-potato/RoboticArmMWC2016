package com 
{
	import flash.display.MovieClip;
	import flash.events.Event;
	/**
	 * ...
	 * @author ...
	 */
	public class BaseMovie extends MovieClip
	{
		public function BaseMovie()
		{
			this.addEventListener(Event.ADDED_TO_STAGE, added_to_stage);
		}
		
		private function added_to_stage(e:Event):void 
		{
			this.removeEventListener(Event.ADDED_TO_STAGE, added_to_stage);
			this.addEventListener(Event.REMOVED_FROM_STAGE, removed_from_stage);
			Init();
		}
		
		protected function Init():void 
		{
			
		}
		
		protected function removed_from_stage(e:Event):void 
		{	
			this.removeEventListener(Event.REMOVED_FROM_STAGE, removed_from_stage);
		}
		
	}

}