/**
 * VERSION: 3.1
 * DATE: 6/30/2009
 * AS3
 * UPDATES & DOCUMENTATION AT: http://www.TweenLite.com
 **/

package gs.data {
	import gs.TweenLite;
/**
 * 	There are 2 primary benefits of using this utility to define your TweenLite variables:
 *  <ol>
 *		<li> In most code editors, code hinting will be activated which helps remind you which special properties are available in TweenLite
 *		<li> It allows you to code using strict datatyping (although it doesn't force you to).
 *  </ol>
 *
 * <b>USAGE:</b><br /><br />
 *	
 *	Instead of <code>TweenLite.to(mc, 1, {x:300, tint:0xFF0000, onComplete:myFunction})</code>, you could use this utility like:<br /><br /><code>
 *	
 *		var myVars:TweenLiteVars = new TweenLiteVars();<br />
 *		myVars.addProp("x", 300); // use addProp() to add any property that doesn't already exist in the TweenLiteVars instance.<br />
 *		myVars.tint = 0xFF0000;<br />
 *		myVars.onComplete = myFunction;<br />
 *		TweenLite.to(mc, 1, myVars);<br /><br /></code>
 *		
 *	Or if you just want to add multiple properties with one function, you can add up to 15 with the addProps() function, like:<br /><br /><code>
 *	
 *		var myVars:TweenLiteVars = new TweenLiteVars();<br />
 *		myVars.addProps("x", 300, false, "y", 100, false, "scaleX", 1.5, false, "scaleY", 1.5, false);<br />
 *		myVars.onComplete = myFunction;<br />
 *		TweenLite.to(my_mc, 1, myVars);<br /><br /></code>
 *		
 * <b>NOTES:</b><br />
 * <ul>
 *	<li> This class adds about 13 Kb to your published SWF (including all dependencies).
 *	<li> This utility is completely optional. If you prefer the shorter synatax in the regular TweenLite class, feel
 *	  	 free to use it. The purpose of this utility is simply to enable code hinting and to allow for strict datatyping.
 *	<li> You may add custom properties to this class if you want, but in order to expose them to TweenLite, make sure
 *	 	 you also add a getter and a setter that adds the property to the _exposedVars Object.
 *	<li> You can reuse a single TweenLiteVars Object for multiple tweens if you want, but be aware that there are a few
 *	 	 properties that must be handled in a special way, and once you set them, you cannot remove them. Those properties
 *	 	 are: frame, visible, tint, and volume. If you are altering these values, it might be better to avoid reusing a TweenLiteVars
 *	 	 Object.
 * </ul>
 * 
 * <b>Copyright 2009, GreenSock. All rights reserved.</b> This work is subject to the terms in <a href="http://www.greensock.com/terms_of_use.html">http://www.greensock.com/terms_of_use.html</a> or for corporate Club GreenSock members, the software agreement that was issued with the corporate membership.
 * 
 * @author Jack Doyle, jack@greensock.com
 */	 
	dynamic public class TweenLiteVars {
		/** @private **/
		public static const version:Number = 3.0;
		/** @private (stands for "isTweenVars") - Just gives us a way to check inside TweenLite to see if the Object is a TweenLiteVars without having to embed the class. This is helpful when handling tint, visible, and other properties that the user didn't necessarily define, but this utility class forces to be present. **/
		public const isTV:Boolean = true; 
		/** Any data that you'd like associated with your tween. **/
		public var data:*;
		/** The number of seconds (or frames for frames-based tweens) to delay before the tween begins. **/
		public var delay:Number = 0;
		/** An easing function (i.e. fl.motion.easing.Elastic.easeOut) The default is Regular.easeOut. **/
		public var ease:Function;
		/** An Array of extra parameter values to feed the easing equation (beyond the standard 4). This can be useful with easing equations like Elastic that accept extra parameters like the amplitude and period. Most easing equations, however, don't require extra parameters so you won't need to pass in any easeParams. **/
		public var easeParams:Array;
		/** A function to call when the tween begins. This can be useful when there's a delay and you want something to happen just as the tween begins. **/
		public var onStart:Function; 
		/** An Array of parameters to pass the onStart function. **/
		public var onStartParams:Array;
		/** A function to call whenever the tweening values are updated (on every frame during the time the tween is active). **/
		public var onUpdate:Function;
		/** An Array of parameters to pass the onUpdate function **/
		public var onUpdateParams:Array;
		/** A function to call when the tween has completed.  **/
		public var onComplete:Function;
		/** An Array of parameters to pass the onComplete function **/
		public var onCompleteParams:Array; 
		/** NONE = 0, ALL = 1, AUTO* = 2, CONCURRENT* = 3  Only available with the optional OverwriteManager add-on class which must be initted once for TweenLite, like OverwriteManager.init(). TweenMax automatically inits OverwriteManager. **/
		public var overwrite:int = 2;  
		/** Normally when you create a tween, it begins rendering on the very next frame (when the Flash Player dispatches an ENTER_FRAME event) unless you specify a "delay". This allows you to insert tweens into timelines and perform other actions that may affect its timing. However, if you prefer to force the tween to render immediately when it is created, set immediateRender to true. Or to prevent a tween with a duration of zero from rendering immediately, set this to false. **/
		public var immediateRender:Boolean = false;
		/** Primarily used in from() calls - forces the values to get flipped. **/
		public var runBackwards:Boolean = false;
		/** If useFrames is set to true, the tweens's timing mode will be based on frames. Otherwise, it will be based on seconds/time. NOTE: a tween's timing mode is always determined by its parent timeline. **/
		public var useFrames : Boolean = false;
		
		/** @private **/
		protected var _exposedVars:Object; // Gives us a way to make certain non-dynamic properties enumerable.
		/** @private **/
		protected var _autoAlpha:Number;
		/** @private **/
		protected var _endArray:Array; 
		/** @private **/
		protected var _frame:int;
		/** @private **/
		protected var _frameLabel:String;
		/** @private **/
		protected var _removeTint:Boolean;
		/** @private **/
		protected var _tint:uint; 
		/** @private **/
		protected var _visible:Boolean = true; 
		/** @private **/
		protected var _volume:Number;
		/** @private **/
		protected var _bevelFilter:BevelFilterVars;
		/** @private **/
		protected var _bezier:Array;
		/** @private **/
		protected var _bezierThrough:Array;
		/** @private **/
		protected var _blurFilter:BlurFilterVars;
		/** @private **/
		protected var _colorMatrixFilter:ColorMatrixFilterVars;
		/** @private **/
		protected var _dropShadowFilter:DropShadowFilterVars;
		/** @private **/
		protected var _glowFilter:GlowFilterVars;
		/** @private **/
		protected var _hexColors:Object;
		/** @private **/
		protected var _orientToBezier:Array;
		/** @private **/
		protected var _quaternions:Object;
		/** @private **/
		protected var _setSize:Object;
 		/** @private **/
		protected var _shortRotation:Object;
 		/** @private **/
		protected var _transformAroundPoint:TransformAroundPointVars;
 		/** @private **/
		protected var _transformAroundCenter:TransformAroundCenterVars;
 		/** @private **/
		protected var _colorTransform:ColorTransformVars;
		/** @private **/
		protected var _motionBlur:Object;
		
		
		/**
		 * @param vars An Object containing properties that correspond to the properties you'd like to add to this TweenLiteVars Object. For example, TweenLiteVars({x:300, onComplete:myFunction})
		 */
		public function TweenLiteVars(vars:Object = null) {
			_exposedVars = {};
			if (vars != null) {
				for (var p:String in vars) {
					if (p == "blurFilter" || p == "glowFilter" || p == "colorMatrixFilter" || p == "bevelFilter" || p == "dropShadowFilter" || p == "transformAroundPoint" || p == "transformAroundCenter" || p == "colorTransform") {
						//ignore SubVars - they must be handled differently later...
					} else if (p != "protectedVars") {
						this[p] = vars[p];
					}
				}
				
				if (vars.blurFilter != null) {
					this.blurFilter = BlurFilterVars.createFromGeneric(vars.blurFilter);
				}
				if (vars.bevelFilter != null) {
					this.bevelFilter = BevelFilterVars.createFromGeneric(vars.bevelFilter);
				}
				if (vars.colorMatrixFilter != null) {
					this.colorMatrixFilter = ColorMatrixFilterVars.createFromGeneric(vars.colorMatrixFilter);
				}
				if (vars.dropShadowFilter != null) {
					this.dropShadowFilter = DropShadowFilterVars.createFromGeneric(vars.dropShadowFilter);
				}
				if (vars.glowFilter != null) {
					this.glowFilter = GlowFilterVars.createFromGeneric(vars.glowFilter);
				}
				if (vars.transformAroundPoint != null) {
					this.transformAroundPoint = TransformAroundPointVars.createFromGeneric(vars.transformAroundPoint);
				}
				if (vars.transformAroundCenter != null) {
					this.transformAroundCenter = TransformAroundCenterVars.createFromGeneric(vars.transformAroundCenter);
				}
				if (vars.colorTransform != null) {
					this.colorTransform = ColorTransformVars.createFromGeneric(vars.colorTransform);
				}
				
				if (vars.protectedVars != null) { //used for clone()-ing protected vars
					var pv:Object = vars.protectedVars;
					for (p in pv) {
						this[p] = pv[p];
					}
				}
			}
			if (TweenLite.version < 11) {
				trace("TweenLiteVars error! Please update your TweenLite class or try deleting your ASO files. TweenLiteVars requires a more recent version. Download updates at http://www.TweenLite.com.");
			}
		}
		
		/**
		 * Adds a dynamic property for tweening and allows you to set whether the end value is relative or not
		 * 
		 * @param name Property name
		 * @param value Numeric end value (or beginning value for from() calls)
		 * @param relative If true, the value will be relative to the target's current value. For example, if my_mc.x is currently 300 and you do addProp("x", 200, true), the end value will be 500.
		 */
		public function addProp(name:String, value:Number, relative:Boolean = false):void {
			if (relative) {
				this[name] = String(value);
			} else {
				this[name] = value;
			}
		}
		
		/**
		 * Adds up to 10 dynamic properties at once (just like doing addProp() multiple times). Saves time and reduces code.
		 */
		public function addProps(name1:String, value1:Number, relative1:Boolean = false,
								 name2:String = null, value2:Number = 0, relative2:Boolean = false,
								 name3:String = null, value3:Number = 0, relative3:Boolean = false,
								 name4:String = null, value4:Number = 0, relative4:Boolean = false,
								 name5:String = null, value5:Number = 0, relative5:Boolean = false,
								 name6:String = null, value6:Number = 0, relative6:Boolean = false,
								 name7:String = null, value7:Number = 0, relative7:Boolean = false,
								 name8:String = null, value8:Number = 0, relative8:Boolean = false,
								 name9:String = null, value9:Number = 0, relative9:Boolean = false,
								 name10:String = null, value10:Number = 0, relative10:Boolean = false):void {
			addProp(name1, value1, relative1);
			if (name2 != null) {
				addProp(name2, value2, relative2);
			}
			if (name3 != null) {
				addProp(name3, value3, relative3);
			}
			if (name4 != null) {
				addProp(name4, value4, relative4);
			}
			if (name5 != null) {
				addProp(name5, value5, relative5);
			}
			if (name6 != null) {
				addProp(name6, value6, relative6);
			}
			if (name7 != null) {
				addProp(name7, value7, relative7);
			}
			if (name8 != null) {
				addProp(name8, value8, relative8);
			}
			if (name9 != null) {
				addProp(name9, value9, relative9);
			}
			if (name10 != null) {
				addProp(name10, value10, relative10);
			}
		}
		
		/**
		 * Clones the TweenLiteVars object.
		 */
		public function clone():TweenLiteVars {
			var vars:Object = {protectedVars:{}};
			appendCloneVars(vars, vars.protectedVars);
			return new TweenLiteVars(vars);
		}
		
		/**
		 * Works with clone() to copy all the necessary properties. Split apart from clone() to take advantage of inheritence for TweenMaxVars
		 */
		protected function appendCloneVars(vars:Object, protectedVars:Object):void {
			var props:Array, special:Array, i:int, p:String;
			props = ["delay","ease","easeParams","onStart","onStartParams","onUpdate","onUpdateParams","onComplete","onCompleteParams","overwrite","immediateRender","runBackwards","useFrames","data"];
			for (i = props.length - 1; i > -1; i--) {
				vars[props[i]] = this[props[i]];
			}
			special = ["_autoAlpha",
					   "_bevelFilter",
					   "_bezier",
					   "_bezierThrough",
					   "_blurFilter",
					   "_colorMatrixFilter",
					   "_colorTransform",
					   "_dropShadowFilter",
					   "_endArray",
					   "_frame",
					   "_frameLabel",
					   "_glowFilter",
					   "_hexColors",
					   "_motionBlur",
					   "_orientToBezier",
					   "_quaternions",
					   "_removeTint",
					   "_setSize",
 					   "_shortRotation",
					   "_tint",
					   "_transformAroundCenter",
					   "_transformAroundPoint",
					   "_visible",
					   "_volume",
					   "_exposedVars"];
			
			for (i = special.length - 1; i > -1; i--) {
				protectedVars[special[i]] = this[special[i]];
			}
			for (p in this) {
				vars[p] = this[p]; //add all the dynamic properties.
			}
		}
		
		
//---- GETTERS / SETTERS -------------------------------------------------------------------------------------------------------------

		/**
		 * @return Exposes enumerable properties.
		 */
		public function get exposedVars():Object {
			var o:Object = {}, p:String;
			for (p in _exposedVars) {
				o[p] = _exposedVars[p];
			}
			for (p in this) {
				o[p] = this[p]; //add all the dynamic properties.
			}
			return o;
		}
		
		/**
		 * @param n Same as changing the "alpha" property but with the additional feature of toggling the "visible" property to false when alpha is 0.
		 */
		public function set autoAlpha(n:Number):void {
			_autoAlpha = _exposedVars.autoAlpha = n;
		}
		public function get autoAlpha():Number {
			return _autoAlpha;
		}
		
		/**
		 * @param a An Array containing numeric end values of the target Array. Keep in mind that the target of the tween must be an Array with at least the same length as the endArray. 
		 */
		public function set endArray(a:Array):void {
			_endArray = _exposedVars.endArray = a;
		}
		public function get endArray():Array {
			return _endArray;
		}
		
		/**
		 * @param b To remove the tint from a DisplayObject, set removeTint to true. 
		 */
		public function set removeTint(b:Boolean):void {
			_removeTint = _exposedVars.removeTint = b;
		}
		public function get removeTint():Boolean {
			return _removeTint;
		}
		
		/**
		 * @param b To set a DisplayObject's "visible" property at the end of the tween, use this special property.
		 */
		public function set visible(b:Boolean):void {
			_visible = _exposedVars.visible = b;
		}
		public function get visible():Boolean {
			return _visible;
		}
		
		/**
		 * @param n Tweens a MovieClip to a particular frame.
		 */
		public function set frame(n:int):void {
			_frame = _exposedVars.frame = n;
		}
		public function get frame():int {
			return _frame;
		}
		
		/**
		 * @param n Tweens a MovieClip to a particular frame.
		 */
		public function set frameLabel(s:String):void {
			_frameLabel = _exposedVars.frameLabel = s;
		}
		public function get frameLabel():String {
			return _frameLabel;
		}
		
		/**
		 * @param n To change a DisplayObject's tint, set this to the hex value of the color you'd like the DisplayObject to end up at(or begin at if you're using TweenLite.from()). An example hex value would be 0xFF0000. If you'd like to remove the tint from a DisplayObject, use the removeTint special property.
		 */
		public function set tint(n:uint):void {
			_tint = _exposedVars.tint = n;
		}
		public function get tint():uint {
			return _tint;
		}
		
		/**
		 * @param n To change a MovieClip's (or SoundChannel's) volume, just set this to the value you'd like the MovieClip to end up at (or begin at if you're using TweenLite.from()).
		 */
		public function set volume(n:Number):void { 
			_volume = _exposedVars.volume = n;
		}
		public function get volume():Number {
			return _volume;
		}
		
		/**
		 * @param f Applies a BevelFilter tween (use the BevelFilterVars utility class to define the values).
		 */
		public function set bevelFilter(f:BevelFilterVars):void {
			_bevelFilter = _exposedVars.bevelFilter = f;
		}
		public function get bevelFilter():BevelFilterVars {
			return _bevelFilter;
		}
		
		/**
		 * @param a Array of Objects, one for each "control point" (see documentation on Flash's curveTo() drawing method for more about how control points work). In this example, let's say the control point would be at x/y coordinates 250,50. Just make sure your my_mc is at coordinates 0,0 and then do: TweenMax.to(my_mc, 3, {_x:500, _y:0, bezier:[{_x:250, _y:50}]});
		 */
		public function set bezier(a:Array):void {
			_bezier = _exposedVars.bezier = a;
		}
		public function get bezier():Array {
			return _bezier;
		}
		
		/**
		 * @param a Identical to bezier except that instead of passing Bezier control point values, you pass values through which the Bezier values should move. This can be more intuitive than using control points.
		 */
		public function set bezierThrough(a:Array):void {
			_bezierThrough = _exposedVars.bezierThrough = a;
		}
		public function get bezierThrough():Array {
			return _bezierThrough;
		}
		
		/**
		 * @param f Applies a BlurFilter tween (use the BlurFilterVars utility class to define the values).
		 */
		public function set blurFilter(f:BlurFilterVars):void {
			_blurFilter = _exposedVars.blurFilter = f;
		}
		public function get blurFilter():BlurFilterVars {
			return _blurFilter;
		}
		
		/**
		 * @param f Applies a ColorMatrixFilter tween (use the ColorMatrixFilterVars utility class to define the values).
		 */
		public function set colorMatrixFilter(f:ColorMatrixFilterVars):void {
			_colorMatrixFilter = _exposedVars.colorMatrixFilter = f;
		}
		public function get colorMatrixFilter():ColorMatrixFilterVars {
			return _colorMatrixFilter;
		}
		
		/**
		 * @param ct Applies a ColorTransform tween (use the ColorTransformVars utility class to define the values).
		 */
		public function set colorTransform(ct:ColorTransformVars):void {
			_colorTransform = _exposedVars.colorTransform = ct;
		}
		public function get colorTransform():ColorTransformVars {
			return _colorTransform;
		}
		
		/**
		 * @param f Applies a DropShadowFilter tween (use the DropShadowFilterVars utility class to define the values).
		 */
		public function set dropShadowFilter(f:DropShadowFilterVars):void {
			_dropShadowFilter = _exposedVars.dropShadowFilter = f;
		}
		public function get dropShadowFilter():DropShadowFilterVars {
			return _dropShadowFilter;
		}
		
		/**
		 * @param f Applies a GlowFilter tween (use the GlowFilterVars utility class to define the values).
		 */
		public function set glowFilter(f:GlowFilterVars):void {
			_glowFilter = _exposedVars.glowFilter = f;
		}
		public function get glowFilter():GlowFilterVars {
			return _glowFilter;
		}
		
		/**
		 * @param o Although hex colors are technically numbers, if you try to tween them conventionally, you'll notice that they don't tween smoothly. To tween them properly, the red, green, and blue components must be extracted and tweened independently. TweenMax makes it easy. To tween a property of your object that's a hex color to another hex color, use this special hexColors property of TweenMax. It must be an OBJECT with properties named the same as your object's hex color properties. For example, if your my_obj object has a "myHexColor" property that you'd like to tween to red (0xFF0000) over the course of 2 seconds, do: TweenMax.to(my_obj, 2, {hexColors:{myHexColor:0xFF0000}}); You can pass in any number of hexColor properties.
		 */
		public function set hexColors(o:Object):void {
			_hexColors = _exposedVars.hexColors = o;
		}
		public function get hexColors():Object {
			return _hexColors;
		}
		
		/**
		 * @param a A common effect that designers/developers want is for a MovieClip/Sprite to orient itself in the direction of a Bezier path (alter its rotation). orientToBezier makes it easy. In order to alter a rotation property accurately, TweenMax needs 4 pieces of information:
		 * 
		 * 1. Position property 1 (typically "x")
		 * 2. Position property 2 (typically "y")
		 * 3. Rotational property (typically "rotation")
		 * 4. Number of degrees to add (optional - makes it easy to orient your MovieClip/Sprite properly)
		 * 
		 * The orientToBezier property should be an Array containing one Array for each set of these values. For maximum flexibility, you can pass in any number of Arrays inside the container Array, one for each rotational property. This can be convenient when working in 3D because you can rotate on multiple axis. If you're doing a standard 2D x/y tween on a bezier, you can simply pass in a boolean value of true and TweenMax will use a typical setup, [["x", "y", "rotation", 0]]. Hint: Don't forget the container Array (notice the double outer brackets)  
		 */
		public function set orientToBezier(a:*):void {
			if (a is Array) {
				_orientToBezier = _exposedVars.orientToBezier = a;
			} else if (a == true) {
				_orientToBezier = _exposedVars.orientToBezier = [["x", "y", "rotation", 0]];
			} else {
				_orientToBezier = null;
				delete _exposedVars.orientToBezier;
			}
		}
		public function get orientToBezier():* {
			return _orientToBezier;
		}
		
		/**
		 * @param q An object with properties that correspond to the quaternion properties of the target object. For example, if your my3DObject has "orientation" and "childOrientation" properties that contain quaternions, and you'd like to tween them both, you'd do: {orientation:myTargetQuaternion1, childOrientation:myTargetQuaternion2}. Quaternions must have the following properties: x, y, z, and w.
		 */
		public function set quaternions(q:Object):void {
			_quaternions = _exposedVars.quaternions = q;
		}
		public function get quaternions():Object {
			return _quaternions;
		}
		
		/**
		 * @param o An object containing a "width" and/or "height" property which will be tweened over time and applied using setSize() on every frame during the course of the tween.
		 */
		public function set setSize(o:Object):void {
			_setSize = _exposedVars.setSize = o;
		}
		public function get setSize():Object {
			return _setSize;
		}
		
		/**
		 * @param o To tween any rotation property (even multiple properties) of the target object in the shortest direction, use shortRotation. For example, if myObject.rotation is currently 170 degrees and you want to tween it to -170 degrees, a normal rotation tween would travel a total of 340 degrees in the counter-clockwise direction, but if you use shortRotation, it would travel 20 degrees in the clockwise direction instead. Pass in an object in with properties that correspond to the rotation values of the target, like {rotation:-170} or {rotationX:-170, rotationY:50}
		 */
 		public function set shortRotation(o:Object):void {
 			_shortRotation = _exposedVars.shortRotation = o;
 		}
 		public function get shortRotation():Object {
 			return _shortRotation;
 		}
 		
 		/**
		 * @param tp Applies a transformAroundCenter tween (use the TransformAroundCenterVars utility class to define the values).
		 */
		public function set transformAroundCenter(tp:TransformAroundCenterVars):void {
			_transformAroundCenter = _exposedVars.transformAroundCenter = tp;
		}
		public function get transformAroundCenter():TransformAroundCenterVars {
			return _transformAroundCenter;
		}
 		
 		/**
		 * @param tp Applies a transformAroundPoint tween (use the TransformAroundPointVars utility class to define the values).
		 */
		public function set transformAroundPoint(tp:TransformAroundPointVars):void {
			_transformAroundPoint = _exposedVars.transformAroundPoint = tp;
		}
		public function get transformAroundPoint():TransformAroundPointVars {
			return _transformAroundPoint;
		}
		
		/**
		 * @param tp Applies a motionBlur tween.
		 */
		public function set motionBlur(o:Object):void {
			_motionBlur = _exposedVars.motionBlur = o;
		}
		public function get motionBlur():Object {
			return _motionBlur;
		}
		
	}
}