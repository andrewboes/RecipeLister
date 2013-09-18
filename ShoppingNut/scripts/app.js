'use strict';

/* App Module */

angular.module('shoppingNut', ['ui.bootstrap'])
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