/**
 * VERSION: 0.84
 * DATE: 7/9/2009
 * AS3 (AS2 is also available)
 * UPDATES & DOCUMENTATION AT: http://www.TweenLite.com
 **/
package gs {
	import flash.display.*;
	import flash.events.*;
	import flash.utils.*;
/**
 * 	TweenNano is a super-lightweight (1.6k in AS3 and 2k in AS2) version of <a href="http://www.TweenLite.com">TweenLite</a> 
 *  and is only recommended for situations where you absolutely cannot afford the extra 2.4k (4k total) that the normal 
 *  TweenLite engine would cost and your project doesn't require any plugins. Otherwise, I'd strongly advocate using 
 *  TweenLite because of the additional flexibility it provides via plugins and compatibility with TimelineLite and TimelineMax. 
 *  TweenNano can do everything TweenLite can do with the following exceptions:
 * 	<ul>
 * 		<li><b> No Plugins </b>- One of the great things about TweenLite is that you can activate
 * 			plugins in order to add features (like autoAlpha, tint, blurFilter, etc.). TweenNano, however, 
 * 			doesn't work with plugins. 
 * 		  
 * 		<li><b> Incompatible with TimelineLite and TimelineMax </b>- Complex sequencing and management of groups of 
 * 			tweens can be much easier with TimelineLite and TimelineMax, but TweenNano instances cannot be inserted into
 * 			TimelineLite or TimelineMax instances.
 * 		  
 * 		<li><b> Slight speed decrease </b>- Under very heavy loads, TweenNano won't perform quite as well as TweenLite, but
 * 			it is extremely unlikely that you'd notice unless you're tweening thousands of objects simultaneously.
 * 		  
 * 		<li><b> Fewer overwrite modes </b>- You can either overwrite all or none of the existing tweens of the same 
 * 			object (overwrite:true or overwrite:false) in TweenNano. TweenLite, however, can use OverwriteManager to expand 
 * 			its capabilities and use modes like AUTO and CONCURRENT (see <a href="http://blog.greensock.com/overwritemanager/">http://blog.greensock.com/overwritemanager/</a>
 * 			for details).
 * 
 * 		<li><b> onStart, easeParams, time, and defaultEase missing </b>- These properties/features don't exist in TweenNano.
 * 	</ul>
 * 
 * <hr>	
 * <b>SPECIAL PROPERTIES:</b>
 * <br /><br />
 * 
 * Any of the following special properties can optionally be passed in through the vars object (the third parameter):
 * 
 * <ul>
 * 	<li><b> delay : Number</b>			Amount of delay before the tween should begin (in seconds).
 * 	
 * 	<li><b> useFrames : Boolean</b>		If useFrames is set to true, the tweens's timing mode will be based on frames. 
 * 										Otherwise, it will be based on seconds/time.
 * 	
 * 	<li><b> ease : Function</b>			Use any standard easing equation to control the rate of change. For example, 
 * 										gs.easing.Elastic.easeOut. The Default is Regular.easeOut.
 * 	
 * 	<li><b> onUpdate : Function</b>		A function that should be called every time the tween's time/position is updated 
 * 										(on every frame while the timeline is active)
 * 	
 * 	<li><b> onUpdateParams : Array</b>	An Array of parameters to pass the onUpdate function
 * 	
 * 	<li><b> onComplete : Function</b>	A function that should be called when the tween has finished 
 * 	
 * 	<li><b> onCompleteParams : Array</b> An Array of parameters to pass the onComplete function.
 * 	
 * 	<li><b> immediateRender : Boolean</b> Normally when you create a from() tween, it renders the starting state immediately even
 * 										  if you define a delay which in typical "animate in" scenarios is very desirable, but
 * 										  if you prefer to override this behavior and have the from() tween render only after any 
 * 										  delay has elapsed, set immediateRender to false. 
 * 	
 * 	<li><b> overwrite : Boolean</b>		Controls how other tweens of the same object are handled when this tween is created. Here are the options:
 * 										<ul>
 * 			  							<li><b> false (NONE):</b> No tweens are overwritten. This is the fastest mode, but you need to be careful not to create any 
 * 			  										tweens with overlapping properties, otherwise they'll conflict with each other. 
 * 											
 * 										<li><b> true (ALL):</b> This is the default mode in TweenNano. All tweens of the same object are completely overwritten immediately when the tween is created. <br /><code>
 * 												   		TweenNano.to(mc, 1, {x:100, y:200});<br />
 * 														TweenNano.to(mc, 1, {x:300, delay:2, overwrite:true}); //immediately overwrites the previous tween</code>
 * 										</ul>
 * 	</ul>
 * 
 * <b>EXAMPLES:</b> <br /><br />
 * 
 * 	Tween the the MovieClip "mc" to an alpha value of 0.5 (50% transparent) and an x-coordinate of 120 
 * 	over the course of 1.5 seconds like so:<br /><br />
 * 	
 * <code>
 * 		import gs.TweenNano;<br /><br />
 * 		TweenNano.to(mc, 1.5, {alpha:0.5, x:120});
 * 	</code><br /><br />
 *  
 * 	To tween the "mc" MovieClip's alpha property to 0.5, its x property to 120 using the "Back.easeOut" easing 
 *  function, delay starting the whole tween by 2 seconds, and then call a function named "onFinishTween" when it 
 *  has completed (it will have a duration of 5 seconds) and pass a few parameters to that function (a value of 
 *  5 and a reference to the mc), you'd do so like:<br /><br />
 * 		
 * 	<code>
 * 		import gs.TweenNano;<br />
 * 		import gs.easing.Back;<br /><br />
 * 
 * 		TweenNano.to(mc, 5, {alpha:0.5, x:120, ease:Back.easeOut, delay:2, onComplete:onFinishTween, onCompleteParams:[5, mc]});<br />
 * 		function onFinishTween(param1:Number, param2:MovieClip):void {<br />
 * 			trace("The tween has finished! param1 = " + param1 + ", and param2 = " + param2);<br />
 * 		}
 * 	</code><br /><br />
 *  
 * 	If you have a MovieClip on the stage that is already in it's end position and you just want to animate it into 
 * 	place over 5 seconds (drop it into place by changing its y property to 100 pixels higher on the screen and 
 * 	dropping it from there), you could:<br /><br />
 * 	
 *  <code>
 * 		import gs.TweenNano;<br />
 * 		import gs.easing.Elastic;<br /><br />
 * 
 * 		TweenNano.from(mc, 5, {y:"-100", ease:Elastic.easeOut});		
 *  </code><br /><br />
 * 
 * <b>NOTES:</b><br /><br />
 * <ul>
 * 	<li> The base TweenNano class adds about 1.6k to your Flash file.
 * 	  
 * 	<li> Passing values as Strings will make the tween relative to the current value. For example, if you do
 * 	  <code>TweenNano.to(mc, 2, {x:"-20"});</code> it'll move the mc.x to the left 20 pixels which is the same as doing
 * 	  <code>TweenNano.to(mc, 2, {x:mc.x - 20});</code> You could also cast it like: <code>TweenNano.to(mc, 2, {x:String(myVariable)});</code>
 * 	
 * 	<li> Kill all tweens for a particular object anytime with the <code>TweenNano.killTweensOf(mc); </code>
 * 	  function. If you want to have the tweens forced to completion, pass true as the second parameter, 
 * 	  like <code>TweenNano.killTweensOf(mc, true);</code>
 * 	  
 * 	<li> You can kill all delayedCalls to a particular function using <code>TweenNano.killTweensOf(myFunction);</code>
 * 	  This can be helpful if you want to preempt a call.
 * 	  
 * 	<li> Use the <code>TweenNano.from()</code> method to animate things into place. For example, if you have things set up on 
 * 	  the stage in the spot where they should end up, and you just want to animate them into place, you can 
 * 	  pass in the beginning x and/or y and/or alpha (or whatever properties you want).
 * 	  
 * 	<li> If you find this class useful, please consider joining Club GreenSock which not only contributes
 * 	  to ongoing development, but also gets you bonus classes (and other benefits) that are ONLY available 
 * 	  to members. Learn more at <a href="http://blog.greensock.com/club/">http://blog.greensock.com/club/</a>
 * </ul>
 * 
 * <b>Copyright 2009, GreenSock. All rights reserved.</b> This work is subject to the terms in <a href="http://www.greensock.com/terms_of_use.html">http://www.greensock.com/terms_of_use.html</a> or for corporate Club GreenSock members, the software agreement that was issued with the corporate membership.
 * 
 * @author Jack Doyle, jack@greensock.com
 */	 
	public class TweenNano {
		/** @private **/
		protected static var _currentTime:Number;
		/** @private **/
		protected static var _currentFrame:uint;
		/** @private Holds references to all our tweens based on their targets (an Array for each target) **/
		protected static var _masterList:Dictionary = new Dictionary(false); 
		/** @private A reference to the Shape that we use to drive all our ENTER_FRAME events. **/
		protected static var _shape:Shape = new Shape(); 
		/** @private Indicates whether or not the TweenNano class has been initted. **/
		protected static var _tnInitted:Boolean; 
		/** @private **/
		protected static var _reservedProps:Object = {ease:1, delay:1, useFrames:1, overwrite:1, onComplete:1, onCompleteParams:1, runBackwards:1, immediateRender:1, onUpdate:1, onUpdateParams:1};
	
		/** Duration of the tween in seconds (or in frames if "useFrames" is true). **/
		public var duration:Number; 
		/** Stores variables (things like "alpha", "y" or whatever we're tweening, as well as special properties like "onComplete"). **/
		public var vars:Object; 
		/** @private Start time in seconds (or frames for frames-based tweens) **/
		public var startTime:Number;
		/** Target object whose properties this tween affects. This can be ANY object, not just a DisplayObject. **/
		public var target:Object;
		/** @private Indicates whether or not the tween is currently active **/
		public var active:Boolean; 
		/** @private Flagged for garbage collection **/
		public var gc:Boolean;
		/** Indicates that frames should be used instead of seconds for timing purposes. So if useFrames is true and the tween's duration is 10, it would mean that the tween should take 10 frames to complete, not 10 seconds. **/
		public var useFrames:Boolean;
		
		/** @private Easing method to use which determines how the values animate over time. Examples are Elastic.easeOut and Strong.easeIn. Many are found in the fl.motion.easing package or gs.easing. **/
		protected var _ease:Function;
		/** @private Indicates whether or not init() has been called (where all the tween property start/end value information is recorded) **/
		protected var _initted:Boolean;
		/** @private Contains parsed data for each property that's being tweened (property name, start, and change) **/
		protected var _propTweens:Array; 
		
		/**
		 * Constructor
		 *  
		 * @param target Target object whose properties this tween affects. This can be ANY object, not just a DisplayObject. 
		 * @param duration Duration in seconds (or in frames if "useFrames" is true)
		 * @param vars An object containing the end values of the properties you're tweening, like {x:100, y:50}. It can also contain special properties like "onComplete", "ease", "delay", etc.
		 */
		public function TweenNano(target:Object, duration:Number, vars:Object) {
			if (!_tnInitted) {			
				_currentTime = getTimer() * 0.001;
				_currentFrame = 0;
				_shape.addEventListener(Event.ENTER_FRAME, updateAll, false, 0, true);
				_tnInitted = true;
			}
			this.vars = vars;
			this.duration = duration;
			this.active = Boolean(duration == 0 && this.vars.delay == 0 && this.vars.immediateRender != false);
			this.target = target;
			if (typeof(this.vars.ease) != "function") {
				_ease = TweenNano.easeOut;
			} else {
				_ease = this.vars.ease;
			}
			_propTweens = [];
			this.useFrames = Boolean(vars.useFrames == true);
			var delay:Number = this.vars.delay || 0;
			this.startTime = (this.useFrames) ? _currentFrame + delay : _currentTime + delay;
			
			if (!(target in _masterList) || int(this.vars.overwrite) != 0) { 
				_masterList[target] = [this];
			} else {
				_masterList[target].push(this);
			}
			
			if (this.vars.immediateRender == true || this.active) {
				renderTime(0);
			}
		}
		
		/**
		 * @private
		 * Initializes the property tweens, determining their start values and amount of change. 
		 * Also triggers overwriting if necessary and sets the _hasUpdate variable.
		 */
		public function init():void {
			for (var p:String in this.vars) {
				if (p in _reservedProps) { 
					//ignore
				} else {
					_propTweens[_propTweens.length] = [p, this.target[p], (typeof(this.vars[p]) == "number") ? this.vars[p] - this.target[p] : Number(this.vars[p])]; //[property, start, change]
				}
			}
			if (this.vars.runBackwards == true) {
				var pt:Array;
				var i:int = _propTweens.length;
				while (i-- > 0) {
					pt = _propTweens[i];
					pt[1] += pt[2];
					pt[2] *= -1;
				}
			}
			_initted = true;
		}
		
		/**
		 * Renders the tween at a particular time (or frame number for frames-based tweens)
		 * WITHOUT changing its startTime, meaning if the tween is in progress when you call
		 * renderTime(), it will not adjust the tween's timing to continue from the new time. 
		 * The time is based simply on the overall duration. For example, if a tween's duration
		 * is 3, renderTime(1.5) would render it at the halfway finished point.
		 * 
		 * @param time time (or frame number for frames-based tweens) to render.
		 */
		public function renderTime(time:Number):void {
			if (!_initted) {
				init();
			}
			var factor:Number, pt:Array, i:int = _propTweens.length;
			if (time >= this.duration) {
				time = this.duration;
				factor = 1;
			} else if (time <= 0) {
				factor = 0;
			} else {
				factor = _ease(time, 0, 1, this.duration);			
			}
			while (i-- > 0) {
				pt = _propTweens[i];
				this.target[pt[0]] = pt[1] + (factor * pt[2]); 
			}
			if (this.vars.onUpdate) {
				this.vars.onUpdate.apply(null, this.vars.onUpdateParams);
			}
			if (time == this.duration) {
				complete(true);
			}
		}
		
		/**
		 * Forces the tween to completion.
		 * 
		 * @param skipRender To skip rendering the final state of the tween, set skipRender to true. 
		 */
		public function complete(skipRender:Boolean=false):void {
			if (!skipRender) {
				renderTime(this.duration);
				return;
			}
			kill();
			if (this.vars.onComplete) {
				this.vars.onComplete.apply(null, this.vars.onCompleteParams);
			}
		}
		
		/** Kills the tween, stopping it immediately. **/
		public function kill():void {
			this.gc = true;
			this.active = false;
		}
		
		
//---- STATIC FUNCTIONS -------------------------------------------------------------------------
		
		/**
		 * Static method for creating a TweenNano instance which can be more intuitive for some developers 
		 * and shields them from potential garbage collection issues that could arise when assigning a
		 * tween instance to a variable that persists. The following lines of code all produce exactly 
		 * the same result: <br /><br /><code>
		 * 
		 * var myTween:TweenNano = new TweenNano(mc, 1, {x:100}); <br />
		 * TweenNano.to(mc, 1, {x:100}); <br />
		 * var myTween:TweenNano = TweenNano.to(mc, 1, {x:100});</code>
		 * 
		 * @param target Target object whose properties this tween affects. This can be ANY object, not just a DisplayObject. 
		 * @param duration Duration in seconds (or frames if "useFrames" is true)
		 * @param vars An object containing the end values of the properties you're tweening, like {x:100, y:50}. It can also contain special properties like "onComplete", "ease", "delay", etc.
		 * @return TweenNano instance
		 */
		public static function to(target:Object, duration:Number, vars:Object):TweenNano {
			return new TweenNano(target, duration, vars);
		}
		
		/**
		 * Static method for creating a TweenNano instance that tweens in the opposite direction
		 * compared to a TweenNano.to() tween. In other words, you define the START values in the 
		 * vars object instead of the end values, and the tween will use the current values as 
		 * the end values. This can be very useful for animating things into place on the stage
		 * because you can build them in their end positions and do some simple TweenNano.from()
		 * calls to animate them into place. <b>NOTE:</b> By default, <code>immediateRender</code>
		 * is <code>true</code> in from() tweens, meaning that they immediately render their starting state 
		 * regardless of any delay that is specified. You can override this behavior by passing 
		 * <code>immediateRender:false</code> in the <code>vars</code> object so that it will wait to 
		 * render until the tween actually begins. To illustrate the default behavior, the following code 
		 * will immediately set the <code>alpha</code> of <code>mc</code> to 0 and then wait 2 seconds 
		 * before tweening the <code>alpha</code> back to 1 over the course of 1.5 seconds:<br /><br /><code>
		 * 
		 * TweenNano.from(mc, 1.5, {alpha:0, delay:2});</code>
		 * 
		 * @param target Target object whose properties this tween affects. This can be ANY object, not just a DisplayObject. 
		 * @param duration Duration in seconds (or frames if "useFrames" is true)
		 * @param vars An object containing the start values of the properties you're tweening like {x:100, y:50}. It can also contain special properties like "onComplete", "ease", "delay", etc.
		 * @return TweenNano instance
		 */
		public static function from(target:Object, duration:Number, vars:Object):TweenNano {
			vars.runBackwards = true;
			if (vars.immediateRender == undefined) {
				vars.immediateRender = true;
			}
			return new TweenNano(target, duration, vars);
		}
		
		/**
		 * Provides a simple way to call a function after a set amount of time (or frames). You can
		 * optionally pass any number of parameters to the function too. For example:<br /><br /><code>
		 * 
		 * TweenNano.delayedCall(1, myFunction, ["param1", 2]); <br />
		 * function myFunction(param1:String, param2:Number):void { <br />
		 *     trace("called myFunction and passed params: " + param1 + ", " + param2); <br />
		 * } </code>
		 * 
		 * @param delay Delay in seconds (or frames if "useFrames" is true) before the function should be called
		 * @param onComplete Function to call
		 * @param onCompleteParams An Array of parameters to pass the function.
		 * @param useFrames If the delay should be measured in frames instead of seconds, set useFrames to true (default is false)
		 * @return TweenNano instance
		 */
		public static function delayedCall(delay:Number, onComplete:Function, onCompleteParams:Array=null, useFrames:Boolean=false):TweenNano {
			return new TweenNano(onComplete, 0, {delay:delay, onComplete:onComplete, onCompleteParams:onCompleteParams, useFrames:useFrames, overwrite:0});
		}
		
		/**
		 * @private
		 * Updates active tweens and activates those whose startTime is before the _currentTime/_currentFrame.
		 * 
		 * @param e ENTER_FRAME Event
		 */
		public static function updateAll(e:Event=null):void {
			_currentFrame++;
			_currentTime = getTimer() * 0.001;
			var ml:Dictionary = _masterList, a:Array, tgt:Object, i:int, t:Number, tween:TweenNano;
			for (tgt in ml) {
				a = ml[tgt];
				i = a.length;
				while (i-- > 0) {
					tween = a[i];
					t = (tween.useFrames) ? _currentFrame : _currentTime;
					if (tween.active || (!tween.gc && t >= tween.startTime)) {
						tween.renderTime(t - tween.startTime);
					} else if (tween.gc) {
						a.splice(i, 1);
					}
				}
				if (a.length == 0) {
					delete ml[tgt];
				}
			}
		}
		
		/**
		 * Kills all the tweens of a particular object, optionally forcing them to completion too.
		 * 
		 * @param target Object whose tweens should be immediately killed
		 * @param complete Indicates whether or not the tweens should be forced to completion before being killed.
		 */
		 public static function killTweensOf(target:Object, complete:Boolean=false):void {
			if (target in _masterList) {
				if (complete) {
					var a:Array = _masterList[target];
					var i:int = a.length;
					while (i-- > 0) {
						if (!a[i].gc) {
							a[i].complete(false);
						}
					}
				}
				delete _masterList[target];
			}
		}
		
		/** @private **/
		public static function easeOut(t:Number, b:Number, c:Number, d:Number):Number {
			return -c * (t /= d) * (t - 2) + b;
		}
		
	}
}