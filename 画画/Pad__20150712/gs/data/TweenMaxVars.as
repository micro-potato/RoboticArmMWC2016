/**
 * VERSION: 3.1
 * DATE: 6/30/2009
 * AS3
 * UPDATES & DOCUMENTATION AT: http://www.TweenLite.com
 **/
package gs.data {
	import gs.TweenLiteVars;
/**
 * 	There are 2 primary benefits of using this utility to define your TweenMax variables:
 *  <ol>
 * 		<li> In most code editors, code hinting will be activated which helps remind you which special properties are available in TweenMax
 * 		<li> It allows you to code using strict datatyping (although it doesn't force you to). 
 *  </ol>
 * 
 * <b>USAGE:</b><br /><br />
 * 	
 * 	Instead of TweenMax.to(my_mc, 1, {x:300, tint:0xFF0000, onComplete:myFunction}), you could use this utility like:<br /><br /><code>
 * 	
 * 		var myVars:TweenMaxVars = new TweenMaxVars();<br />
 * 		myVars.addProp("x", 300); // use addProp() to add any property that doesn't already exist in the TweenMaxVars instance.<br />
 * 		myVars.tint = 0xFF0000;<br />
 * 		myVars.onComplete = myFunction;<br />
 * 		TweenMax.to(my_mc, 1, myVars);<br /><br /></code>
 * 		
 * 	Or if you just want to add multiple properties with one function, you can add up to 15 with the addProps() function, like:<br /><br /><code>
 * 	
 * 		var myVars:TweenMaxVars = new TweenMaxVars();<br />
 * 		myVars.addProps("x", 300, false, "y", 100, false, "scaleX", 1.5, false, "scaleY", 1.5, false);<br />
 * 		myVars.onComplete = myFunction;<br />
 * 		TweenMax.to(my_mc, 1, myVars);<br /><br /></code>
 * 		
 * <b>NOTES:</b>
 * <ul>
 * 		<li> This class adds about 14 Kb to your published SWF.
 * 		<li> This utility is completely optional. If you prefer the shorter synatax in the regular TweenMax class, feel
 * 	  		 free to use it. The purpose of this utility is simply to enable code hinting and to allow for strict datatyping.
 * 		<li> You may add custom properties to this class if you want, but in order to expose them to TweenMax, make sure
 * 	  		 you also add a getter and a setter that adds the property to the _exposedVars Object.
 * 		<li> You can reuse a single TweenMaxVars Object for multiple tweens if you want, but be aware that there are a few
 * 	  		 properties that must be handled in a special way, and once you set them, you cannot remove them. Those properties
 * 	  		 are: frame, visible, tint, and volume. If you are altering these values, it might be better to avoid reusing a TweenMaxVars
 * 	  		 Object.
 * </ul>
 * 
 * <b>Copyright 2009, GreenSock. All rights reserved.</b> This work is subject to the terms in <a href="http://www.greensock.com/terms_of_use.html">http://www.greensock.com/terms_of_use.html</a> or for corporate Club GreenSock members, the software agreement that was issued with the corporate membership.
 * 
 * @author Jack Doyle, jack@greensock.com
 */	 
	dynamic public class TweenMaxVars extends TweenLiteVars {
		/** @private **/
		public static const version:Number = 3.0;
		/** A function to which the TweenMax instance should dispatch a TweenEvent when it begins. This is the same as doing myTweenMaxInstance.addEventListener(TweenEvent.START, myFunction); **/
		public var onStartListener:Function;
		/** A function to which the TweenMax instance should dispatch a TweenEvent every time it updates values. This is the same as doing myTweenMaxInstance.addEventListener(TweenEvent.UPDATE, myFunction); **/
		public var onUpdateListener:Function;
		/** A function to which the TweenMax instance should dispatch a TweenEvent when it completes. This is the same as doing myTweenMaxInstance.addEventListener(TweenEvent.COMPLETE, myFunction); **/
		public var onCompleteListener:Function;
		/**  A function that should be called when the tween has reached its starting point again after having been reversed **/
		public var onReverseComplete : Function;
		/** An Array of parameters to pass the onReverseComplete functions **/
		public var onReverseCompleteParams : Array; 
		/** A function that should be called every time the tween repeats **/
		public var onRepeat : Function; 
 		/** An Array of parameters to pass the onRepeat function **/
 		public var onRepeatParams : Array; 
		/** Works in conjunction with the repeat property, determining the behavior of each cycle. When yoyo is true, the tween will go back and forth, appearing to reverse every other cycle (this has no affect on the "reversed" property though). **/
		public var yoyo:Boolean = false;
		/** Number of times that the tween should repeat. To repeat indefinitely, use -1. **/
		public var repeat:int = 0;
		/** Amount of time in seconds (or frames for frames-based tween) between repeats. **/
		public var repeatDelay:Number = 0;
		/** If true, the tween will be paused initially. **/
		public var paused : Boolean; 
		/** If true, the tween will be reversed initially. This does not swap the starting/ending values in the tween - it literally changes its orientation/direction. Imagine the playhead moving backwards instead of forwards. **/
		public var reversed : Boolean; 
		/** Allows you to define the starting values for each property. Typically, TweenMax uses the current value (whatever it happens to be at the time the tween begins) as the start value, but startAt allows you to override that behavior. Simply pass an object in with whatever properties you'd liketo set just before the tween begins. For example, if mc.x is currently 100, and you'd like to tween it from 0 to 500, do TweenMax.to(mc, 2, {x:500, startAt:{x:0}});  **/
		public var startAt:TweenLiteVars;
		/** @private **/
		protected var _roundProps:Array;
		
		/**
		 * @param vars An Object containing properties that correspond to the properties you'd like to add to this TweenMaxVars Object. For example, TweenMaxVars({blurFilter:{blurX:10, blurY:20}, onComplete:myFunction})
		 */
		public function TweenMaxVars(vars:Object = null) {
			super(vars);
		}
		
		/**
		 * Clones the TweenMaxVars object.
		 */
		override public function clone():TweenLiteVars {
			var vars:Object = {protectedVars:{}};
			appendCloneVars(vars, vars.protectedVars);
			return new TweenMaxVars(vars);
		}
		
		/**
		 * Works with clone() to copy all the necessary properties. Split apart from clone() to take advantage of inheritence
		 */
		override protected function appendCloneVars(vars:Object, protectedVars:Object):void {
			super.appendCloneVars(vars, protectedVars);
			var props:Array = ["onStartListener","onUpdateListener","onCompleteListener","onRepeat","onRepeatParams","onReverseComplete","onReverseCompleteParams","yoyo","repeat","repeatDelay","startAt","reversed","paused"];
			for (var i:int = props.length - 1; i > -1; i--) {
				vars[props[i]] = this[props[i]];
			}
			protectedVars._roundProps = _roundProps;
		}
		
		
//---- GETTERS / SETTERS ---------------------------------------------------------------------------------------------
		
		/**
		 * @param a An Array of the names of properties that should be rounded to the nearest integer when tweening
		 */
		public function set roundProps(a:Array):void {
			_roundProps = _exposedVars.roundProps = a;
		}
		public function get roundProps():Array {
			return _roundProps;
		}
		
		
	}
}