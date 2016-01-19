package com.MC
{
	import com.BaseMovie;
	import com.Main;
	import com.Model.M_Data;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import com.powerflasher.as3potrace.POTrace;
	import com.powerflasher.as3potrace.backend.GraphicsDataBackend;

	import flash.display.Bitmap;
	import flash.display.BitmapData;
	import flash.display.CapsStyle;
	import flash.display.GraphicsEndFill;
	import flash.display.GraphicsPath;
	import flash.display.GraphicsSolidFill;
	import flash.display.GraphicsStroke;
	import flash.display.IGraphicsData;
	import flash.display.JointStyle;
	import flash.display.LineScaleMode;
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.display.Sprite;
	import flash.filters.ColorMatrixFilter;
	import flash.filters.ConvolutionFilter;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.media.Camera;
	import flash.media.Video;
	import flash.net.FileFilter;
	import flash.net.FileReference;
	import flash.sensors.Accelerometer;
	import flash.text.TextField;
	import flash.text.TextFieldAutoSize;
	/**
	 * ...
	 * @author ...
	 */
	public class MC_Camera extends BaseMovie
	{
		private var mc_cameraComplete:MC_CameraComplete;

		private var container:POTrace;
		private var file:FileReference = new FileReference();
		private var cam:Camera;
		private var bmd:BitmapData = new BitmapData(300,240);
		private var bmp:Bitmap;
		private var video:Video = new Video();

		private var line_array:Array = null;
		public function MC_Camera()
		{

		}

		override protected function Init():void
		{
			super.Init();

			btn_commit.addEventListener(MouseEvent.CLICK, on_commit_click);
			btn_back.addEventListener(MouseEvent.CLICK, on_back_click);
			btn_change.addEventListener(MouseEvent.CLICK, on_change_click);
			
			InitCamera();
		}
		
		private function InitCamera():void 
		{
			DisposeCamera();
			
			cam = Camera.getCamera(M_Data.cameraIndex.toString());
			cam.setMode(320,240,12);
			video.attachCamera(cam);

			if (cam != null)
			{
				container = new POTrace();
				container.x = 300;
				container.y = 300;
				//container.width = 640;
				//container.height = 480;
				addChild(container);
				container.scaleX = container.scaleY = 3;
				bmp = new Bitmap();
				//addChild(bmp);
				addEventListener(Event.ENTER_FRAME, render);
			}
		}
		
		private function DisposeCamera():void 
		{
			if (cam)
			{
				cam= Camera.getCamera(null);
				cam= null;
				video.attachCamera(null);
			}
			
			if (container)
			{
				if (this.contains(container))
				{
					this.removeChild(container);
				}
			}
			
			
		}

		private function render(e:Event):void
		{
			bmd.draw(video);
			bmp.bitmapData = CartoonFilter.Cartoonize(bmd);
			toVec(bmp);
		}

		private function toVec(bmp:Bitmap):void
		{
			container.graphics.clear();
			container.graphics.lineStyle(3, 0);
			line_array = container.potrace_trace(bmp.bitmapData);

		}

		private function on_commit_click(e:MouseEvent):void
		{
			/*
			if (mc_cameraComplete)
			{
			if (this.contains(mc_cameraComplete))
			{
			this.removeChild(mc_cameraComplete);
			}
			}
			mc_cameraComplete = new MC_CameraComplete();
			addChild(mc_cameraComplete);
			*/

			if (line_array != null)
			{
				M_Data.line_Array = line_array;
			}
			
			cam= Camera.getCamera(null);
			cam= null;
			video.attachCamera(null);
			
			Main.Instance.ChangeScene("CameraComplete");
		}

		private function on_back_click(e:MouseEvent):void
		{
			cam= Camera.getCamera(null);
			cam= null;
			video.attachCamera(null);
			
			Main.Instance.ChangeScene("ImageGraphics");
		}
		
		private function on_change_click(e:MouseEvent):void 
		{
			if (M_Data.cameraIndex == 1)
			{
				M_Data.cameraIndex = 0;
			}
			else 
			{
				M_Data.cameraIndex = 1;
			}
			InitCamera();
		}

		override protected function removed_from_stage(e:Event):void
		{
			super.removed_from_stage(e);
			btn_commit.removeEventListener(MouseEvent.CLICK, on_commit_click);
			btn_back.removeEventListener(MouseEvent.CLICK, on_back_click);
			btn_change.removeEventListener(MouseEvent.CLICK, on_change_click);
			removeEventListener(Event.ENTER_FRAME, render);
		}

	}

}