(function ($) {

	$.fn.draggableTouch = function (options) {
		var settings = $.extend({
			onSortEndCallback: function (){}
		}, options);
		var elementToMove;
		var ulElement;

		var sortEventStart = function (event, objectCalling) {
			event.preventDefault();
			elementToMove = objectCalling;
		};

		var sortEventEnd = function (event, objectCalling, lastY) {
			$('#linePlaceHolder').remove();
			var v = $(objectCalling);
			v.remove();
			v.css('position', '');
			v.css('left', '');
			v.css('top', '');
			var order = findDropSpot(v, lastY);
			console.log(elementToMove);
			settings.onSortEndCallback.call(this, 0, Number(order) + 1 );
			elementToMove = null;
		};

		var findDropSpot = function (element, location) {
			var myChildren = $(ulElement).children();
			for (var i = 0; i < myChildren.length; i++) {
				var myChild = myChildren[i];
				if (location < $(myChild).offset().top) {
					$(element).insertBefore($(myChild));
					return; //$(element).find(".itemOrder").text();
				}
			}
			$(element).insertAfter($(ulElement).children().last());
		};

		var move = function (x, y, clientY) {
			if (elementToMove) {
				elementToMove.style.position = "absolute";
				elementToMove.style.left = x + "px";
				elementToMove.style.top = y + "px";
				$('#linePlaceHolder').remove();
				var v = "<div id='linePlaceHolder' style='border-color:rgb(51,51,51); border-style: solid; border-width: 1px'><div></div></div>";
				findDropSpot(v, y);
				if (clientY < 20) {
					parent.window.scrollBy(0, -10);
				}
				if ($(window).height() - clientY < 50) {
					parent.window.scrollBy(0, 10);
				}
			}
		};
		
		this.each(function () {
			ulElement = this;
			var lastY = null;

			$(this).delegate('li .handle', 'touchend', function (event) {
				sortEventEnd(event, this.parentNode, lastY);
			});

			$(this).delegate('li .handle', 'touchstart', function (event) {
				sortEventStart(event, this.parentNode);
			});

			$(this).delegate('li .handle', 'touchmove', function () {
				var x = event.targetTouches[0].pageX;
				var y = event.targetTouches[0].pageY;
				move(x, y, event.targetTouches[0].clientY);
				lastY = y;
			});

			$(this).delegate('li .handle', 'mouseup', function (event) {
				sortEventEnd(event, this.parentNode, event.pageY);
			});

			$(this).delegate('li .handle', 'mousedown', function (e) {
				sortEventStart(e, this.parentNode);
			});

			document.onmousemove = function (e) {
				move(e.pageX, e.pageY, e.clientY);
			};
		});
		return this;
	};
})(jQuery);