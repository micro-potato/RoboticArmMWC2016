package com {
	
	import flash.display.StageScaleMode;
	import flash.display.StageDisplayState;
	
	public class Main extends BaseMovie {
		
		public function Main() {
			// constructor code
		}
		override protected function Init():void {
			super.Init();
			
			stage.scaleMode=StageScaleMode.EXACT_FIT;
			//stage.displayState=StageDisplayState.FULL_SCREEN_INTERACTIVE;
			
		}
	}
}
