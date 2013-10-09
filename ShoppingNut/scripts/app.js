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
			var colWidth = 340;
			var margin = 10;
			var spaceLeft = 0;
			var blocks = [];
			var windowWidth = 0;

			$scope.setupBlocks = function() {
				blocks = [];
				windowWidth = $(window).width();
				colCount = Math.floor(windowWidth / (colWidth + margin * 2));
				spaceLeft = (windowWidth - ((colWidth * colCount) + (margin * (colCount - 1)))) / 2;
				console.log(spaceLeft);
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
					console.log("min: "+min+ ", index: " + index+", leftPos: " + leftPos);
					blocks[index] = min + $(this).outerHeight() + margin;
					console.log("outer Height: " + $(this).outerHeight());
				});
				
			}
			
			Array.min = function (array) {
				return Math.min.apply(Math, array);
			};
			
			var w = angular.element($window);
			w.bind('resize', function () {
				$scope.setupBlocks();
			});

			if ($scope.$last) {
				$timeout(function () {
					$scope.setupBlocks();
				}, 200);
			}
			
			$('#searchRecipeTextBox').keypress(function () {
				$scope.setupBlocks();
			});
		};
	})
	.config(['$routeProvider', function ($routeProvider) {
  	$routeProvider.
				when('/lists', { templateUrl: 'html/listList.html', controller: shoppingListCtrl }).
				when('/lists/add', { templateUrl: 'html/listAdd.html', controller: listAddCtrl }).
				when('/lists/:listId', { templateUrl: 'html/listDetail.html', controller: listDetailCtrl }).
				when('/recipes', { templateUrl: 'html/recipeList.html', controller: recipeListCtrl }).
				when('/recipes/add', { templateUrl: 'html/recipeAdd.html', controller: recipeAddCtrl }).
				when('/recipes/edit/:recipeId', { templateUrl: 'html/recipeAdd.html', controller: recipeAddCtrl }).
				when('/recipes/:recipeId', { templateUrl: 'html/recipeDetail.html', controller: recipeDetailCtrl }).
				otherwise({ redirectTo: '/recipes' });
  }]);