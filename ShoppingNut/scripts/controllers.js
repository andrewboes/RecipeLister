'use strict';

function recipeListCtrl($scope, $http) {
	$http.get('/Home/GetAllRecipes').success(function (data) {
		$scope.recipes = data;
		angular.forEach($scope.recipes, function(recipe) {
			recipe.imgSrc = $scope.getImage();
		});
	}).error(function (data, status, headers, config) {
		alert("error");
	});
	
	var images = ['images/apple-pie.jpg',
			'img/berry-pie.jpg',
			'img/black-bean.jpg',
			'img/coconut-cream.jpg',
			'img/fajitas.jpg',
			'img/pumpkin_soup_recipe.jpg'
		];

	$scope.getImage = function () {
		var r = Math.random();
		var to = images.length;
		var randIndex = Math.floor(r * to);
		return images[randIndex];
	};
}

function shoppingListCtrl($scope, $http) {
	$http.get('/Home/GetShoppingLists').success(function (data) {
		$scope.lists = data;
	}).error(function (data, status, headers, config) {
		alert("error");
	});
}

function recipeDetailCtrl($scope, $routeParams, $http) {
	$http.get('/Home/GetRecipeById?id=' + $routeParams.recipeId).success(function (data) {
		$scope.recipe = data;
		var sum = 0;
		var ings = $scope.recipe.Ingredients;
		for (var i = 0; i < ings.length; i++) {
			sum += ings[i].Calories;
		}
		$scope.recipe.CaloriesPerServing = sum / $scope.recipe.Servings;

	}).error(function (data, status, headers, config) {
		alert("error");
	});
}

function recipeAddCtrl($scope, $http, $location, $routeParams) {
	$("#foodSearch").focus(function () {
		$('html, body').animate({ scrollTop: $("#foodSearch").offset().top - 50 }, 'slow');
	});
	var id = $routeParams.recipeId;
	if (id) {
		$http.get('/Home/GetRecipeById?id=' + $routeParams.recipeId).success(function (data) {
			$scope.recipe = data;
		}).error(function (data, status, headers, config) {
			alert("error");
		});
	}
	else {
		$scope.recipe = { Name: '', Description: '', CaloriesPerServing: '', Ingredients: [], Instructions: [] };
	}
	
	$scope.save = function () {
		$http.post('/Home/AddRecipe', $scope.recipe).success(function (data) {
			if (data.Success) {
				$location.path('/');
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.addIngredient = function () {
		var ingredient = {
			Food: $scope.currentSelectedFood,
			Quantity: $scope.quantity,
			QuantityType: $scope.quantityType,
			FoodId: $scope.currentSelectedFood.Id,
			QuantityTypeId: $scope.quantityType.Id
		};
		$scope.recipe.Ingredients.push(ingredient);
		$scope.foodSearch = '';
		$scope.quantity = '';
		$scope.ingredientQuantityTypes = '';
	};

	$scope.addInstruction = function () {
		var instruction = { Id: 0, Text: $scope.newInstruction };
		$scope.recipe.Instructions.push(instruction);
		$scope.newInstruction = '';
	};

	$scope.deleteIngredient = function (index) {
		$scope.recipe.Ingredients.splice(index, 1);
	};

	$scope.deleteInstruction = function (index) {
		$scope.recipe.Instructions.splice(index, 1);
	};

	$scope.foodSelected = function($item) {
		$http.get('/Home/GetFoodQuantityTypes?id=' + $item.Id).success(function (data) {
			$scope.ingredientQuantityTypes = data;
		});
		$scope.currentSelectedFood = $item;
	};

	$scope.foods = function (foodQuery) {
		if (foodQuery != null && foodQuery.length > 2) {
			return $http.get('/Home/GetFoods?filter=' + foodQuery).then(function(response) {
				return response.data;
			});
		} else {
			return [];
		}
	};
}

function listAddCtrl($scope, $http, $location) {
	$scope.list = { Name: '', Items: [] };
	
	$http.get('/Home/GetAllRecipesWithIngredients').success(function (data) {
		$scope.recipes = data;
	}).error(function (data, status, headers, config) {
		alert("error");
	});

	$scope.updateRecipeAndAddIngredients = function(index, data) {
		$scope.recipes[index] = data;
		$scope.addIngredientsForRecipe(index, data);
	};

	$scope.addIngredientsForRecipe = function (index, data) {
		for (var i = 0; i < data.Ingredients.length; i++) {
			$scope.addIngredient(index, i);
		}
	};

	$scope.addAllIngredients = function (index) {
		if (!$scope.recipes[index].Ingredients) {
			$scope.getIngredientsForRecipe(index, $scope.updateRecipeAndAddIngredients);
		} else {
			$scope.addIngredientsForRecipe(index, $scope.recipes[index]);
		}
	};

	$scope.addIngredient = function (recipeIndex, itemIndex) {
		var ingredient = $scope.recipes[recipeIndex].Ingredients[itemIndex];
		var updated = false;
		for (var i = 0; i < $scope.list.Items.length; i++) {
			if ($scope.list.Items[i].FoodId == ingredient.FoodId && $scope.list.Items[i].QuantityTypeId == ingredient.QuantityTypeId) {
				$scope.list.Items[i].Quantity += ingredient.Quantity;
				updated = true;
			}
		}
		if (!updated) {
			$scope.list.Items.push({
				Quantity: ingredient.Quantity,
				QuantityType: ingredient.QuantityType,
				QuantityTypeId: ingredient.QuantityTypeId,
				Name: ingredient.Food.Name,
				FoodId: ingredient.FoodId
			});
		}
	};

	$scope.addItem = function () {
		var item = {
			Name: $scope.foodSearch,
			Quantity: $scope.quantity,
			QuantityType: $scope.quantityType
		};
		if ($scope.currentSelectedFood) 
			item.FoodId = $scope.currentSelectedFood.Id;
		if ($scope.quantityType) {
			item.QuantityTypeId = $scope.quantityType.Id;
		}
		$scope.list.Items.push(item);
		$scope.foodSearch = '';
		$scope.quantity = '';
		$scope.ingredientQuantityTypes = '';
		$scope.quantityType = '';
		$scope.currentSelectedFood = '';

	};

	$scope.save = function () {
		$http.post('/Home/InsertOrUpdateShoppingList', $scope.list).success(function (data) {
			if (data.Success) {
				$location.path('/');
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
		});
	};
	
	$scope.foods = function (foodQuery) {
		if (foodQuery != null && foodQuery.length > 2) {
			return $http.get('/Home/GetFoods?filter=' + foodQuery).then(function (response) {
				return response.data;
			});
		} else {
			return [];
		}
	};
	
	$scope.foodSelected = function ($item) {
		$http.get('/Home/GetFoodQuantityTypes?id=' + $item.Id).success(function (data) {
			$scope.ingredientQuantityTypes = data;
		});
		$scope.currentSelectedFood = $item;
	};
}

function listDetailCtrl($scope, $http, $routeParams) {
	$http.get('/Home/GetListById?id=' + $routeParams.listId).success(function (data) {
		$scope.list = data;
	}).error(function (data, status, headers, config) {
		alert("error");
	});
}