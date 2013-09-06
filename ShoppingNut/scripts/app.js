'use strict';

/* App Module */

angular.module('shoppingNut', ['ui.bootstrap']).
  config(['$routeProvider', function ($routeProvider) {
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