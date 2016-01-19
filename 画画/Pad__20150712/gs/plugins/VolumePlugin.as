/**
 * VERSION: 1.01
 * DATE: 5/2/2009
 * ACTIONSCRIPT VERSION: 3.0 
 * UPDATES & MORE DETAILED DOCUMENTATION AT: http://www.TweenMax.com
 **/
package gs.plugins {
	import flash.display.*;
	import flash.media.SoundTransform;
	import gs.*;
	import gs.plugins.*;
/**
 * Tweens the volume of an object with a soundTransform property (MovieClip/SoundChannel/NetStream, etc.). <br /><br />
 * 
 * <b>USAGE:</b><br /><br />
 * <code>
 * 		import gs.TweenLite; <br />
 * 		import gs.plugins.VolumePlugin; <br />
 * 		TweenPlugin.activate([VolumePlugin]); //activation is permanent in the SWF, so this line only needs to be run once.<br /><br />
 * 
 * 		TweenLite.to(mc, 1, {volume:0}); <br /><br />
 * </code>
 *
 * Bytes added to SWF: 275 (not including dependencies)<br /><br />
 * 
 * <b>Copyright 2009, GreenSock. All rights reserved.</b> This work is subject to the terms in <a href="http://www.greensock.com/terms_of_use.html">http://www.greensock.com/terms_of_use.html</a> or for corporate Club GreenSock members, the software agreement that was issued with the corporate membership.
 * 
 * @author Jack Doyle, jack@greensock.com
 */
	public class VolumePlugin extends TweenPlugin {
		/** @private **/
		public static const VERSION:Number = 1.01;
		/** @private **/
		public static const API:Number = 1.0; //If the API/Framework for plugins changes in the future, this number helps determine compatibility
		
		/** @private **/
		protected var _target:Object;
		/** @private **/
		protected var _st:SoundTransform;
		
		/** @private **/
		public function VolumePlugin() {
			super();
			this.propName = "volume";
			this.overwriteProps = ["volume"];
		}
		
		/** @private **/
		override public function onInitTween(target:Object, value:*, tween:TweenLite):Boolean {
			if (isNaN(value) || !target.hasOwnProperty("soundTransform")) {
				return false;
			}
			_target = target;
			_st = _target.soundTransform;
			addTween(_st, "volume", _st.volume, value, "volume");
			return true;
		}
		
		/** @private **/
		override public function set changeFactor(n:Number):void {
			updateTweens(n);
			_target.soundTransform = _st;
		}
		
	}
}