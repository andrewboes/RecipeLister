'use strict';

/* App Module */

angular.module('shoppingNut', ['ui.bootstrap', 'angularFileUpload'])
	.directive('ngModelOnblur', function () {
		return {
			restrict: 'A',
			require: 'ngModel',
			link: function (scope, elm, attr, ngModelCtrl) {
				if (attr.type === 'radio' || attr.type === 'checkbox') return;

				elm.unbind('input').unbind('keydown').unbind('change');
				elm.bind('blur', function () {
					scope.$apply(function () {
						ngModelCtrl.$setViewValue(elm.val());
					});
				});
			}
		};
	})
	.directive('pin', function ($window, $timeout) {
		return function ($scope) {
			var colCount = 0;
			var colWidth = 270;
			var margin = 10;
			var spaceLeft = 0;
			var blocks = [];
			var windowWidth = 0;

			$scope.setupBlocks = function() {
				blocks = [];
				windowWidth = $(window).width();
				colCount = Math.floor(windowWidth / (colWidth + margin * 2));
				spaceLeft = (windowWidth - ((colWidth * colCount) + (margin * (colCount - 1)))) / 2;
				for (var i = 0; i < colCount; i++) {
					blocks.push(margin);
				}
				positionBlocks();
			};

			function positionBlocks() {
				$('.block').each(function (i) {
					var min = Array.min(blocks);
					var index = $.inArray(min, blocks);
					var leftPos = margin + (index * (colWidth + margin));
					$(this).css({
						'left': (leftPos + spaceLeft) + 'px',
						'top': (min + 140) + 'px'
					});
					blocks[index] = min + $(this).outerHeight() + margin;
				});
				
			}
			
			Array.min = function (array) {
				return Math.min.apply(Math, array);
			};
			
			var w = angular.element($window);
			w.bind('resize', function () {
				$timeout(function () {
					$scope.setupBlocks();
				}, 200);
			});

			if ($scope.$last) {
				$timeout(function () {
					$scope.setupBlocks();
				}, 100);
			}
			
			//This doesn't work!
			$('#searchRecipeTextBox').keypress(function () {
				$scope.setupBlocks();
			});
		};
	})
	.directive('ngSortable',  function () {
		return function ($scope, element) {
			var elementToMove;
		
			var sortEventStart = function (event, objectCalling) {
				event.preventDefault();
				elementToMove = objectCalling;
				console.log();
			};

			var sortEventEnd = function (event, objectCalling, lastY) {
				$('#linePlaceHolder').remove();
				//var v = $(objectCalling);
				//v.remove();
				$(objectCalling).css('position', '');
				$(objectCalling).css('left', '');
				$(objectCalling).css('top', '');
				//findDropSpot(objectCalling, lastY);
				var newIndex = -1;
				var myChildren = $(element).children();
				for (var i = 0; i < myChildren.length; i++) {
					var myChild = myChildren[i];
					if (lastY < $(myChild).offset().top) {
						newIndex = Number($(myChild).attr("id"));
						break;
					}
				}
				if(newIndex === -1)
					newIndex = Number((element).children().last().attr("id")) + 1;
				$scope.finishedSorting($(elementToMove).attr("id"), newIndex);
				elementToMove = null;
			};

			var findDropSpot = function (myElement, location) {
				var myChildren = $(element).children();
				for (var i = 0; i < myChildren.length; i++) {
					var myChild = myChildren[i];
					if (location < $(myChild).offset().top) {
						$(myElement).insertBefore($(myChild));
						return;
					}
				}
				$(myElement).insertAfter($(element).children().last());
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
			var lastY = null;

			$(element).delegate('li .handle', 'touchend', function (event) {
				sortEventEnd(event, this.parentNode, lastY);
			});

			$(element).delegate('li .handle', 'touchstart', function (event) {
				sortEventStart(event, this.parentNode);
			});

			$(element).delegate('li .handle', 'touchmove', function () {
				var x = event.targetTouches[0].pageX;
				var y = event.targetTouches[0].pageY;
				move(x, y, event.targetTouches[0].clientY);
				lastY = y;
			});

			$(element).delegate('li .handle', 'mouseup', function (event) {
				sortEventEnd(event, this.parentNode, event.pageY);
			});

			$(element).delegate('li .handle', 'mousedown', function (e) {
				sortEventStart(e, this.parentNode);
			});

			document.onmousemove = function (e) {
				move(e.pageX, e.pageY, e.clientY);
			};
			//link: function (scope, element) {
			//	angular.element(element).draggableTouch({ onSortEndCallback: function () { scope.finishedSorting(); } });
			//}
		};
	})
	.config(['$routeProvider', function ($routeProvider) {
  	$routeProvider.
				when('/lists', { templateUrl: 'html/listList.html', controller: shoppingListCtrl }).
				when('/lists/add', { templateUrl: 'html/listAdd.html', controller: listAddCtrl }).
				when('/lists/detail/:listId', { templateUrl: 'html/listDetail.html', controller: listDetailCtrl }).
				when('/lists/edit/:listId', { templateUrl: 'html/listAdd.html', controller: listAddCtrl }).
				when('/recipes', { templateUrl: 'html/recipeList.html', controller: recipeListCtrl }).
				when('/recipes/all/:all', { templateUrl: 'html/recipeList.html', controller: recipeListCtrl }).
				when('/recipes/add', { templateUrl: 'html/recipeAdd.html', controller: recipeAddCtrl }).
				when('/recipes/edit/:recipeId', { templateUrl: 'html/recipeAdd.html', controller: recipeAddCtrl }).
				when('/recipes/copy', { templateUrl: 'html/recipeCopy.html', controller: recipeCopyCtrl }).
				otherwise({ redirectTo: '/recipes' });
  }]);