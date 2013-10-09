'use strict';

function recipeListCtrl($scope, $http) {
	$http.get('/Home/GetAllRecipes').success(function (data) {
		$scope.recipes = data;
		angular.forEach($scope.recipes, function (recipe) {
			if(recipe.Images.length == 0)
				recipe.imgSrc = $scope.getImage();
			else {
				recipe.imgSrc = '/Home/Image/' + recipe.Images[0];
			}
		});
	}).error(function (data, status, headers, config) {
		alert("error");
	});
	
	var images = [
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
		angular.forEach($scope.lists, function (list) {
			var pickedUp = 0;
			angular.forEach(list.Items, function(item) {
				if (item.PickedUp)
					pickedUp++;
			});
			list.numberPickedUp = pickedUp;
			list.percentComplete = pickedUp / list.Items.length;
		});
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
			$scope.getImages();
		}).error(function (data, status, headers, config) {
			alert("error");
		});
	}
	else {
		$scope.recipe = { Name: '', Description: '', CaloriesPerServing: '', Ingredients: [], Instructions: [], Id: 0 };
	}
	$scope.saved = true;

	$scope.insertOrUpdate = function () {
		$scope.saved = false;
		$http.post('/Home/InsertOrUpdateRecipe', $scope.recipe).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				if ($scope.recipe.Id == 0)
					$scope.recipe.Id = data.RecipeId;
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
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
			QuantityTypeId: $scope.quantityType.Id,
			RecipeId: $scope.recipe.Id
		};
		$http.post('/Home/InsertOrUpdateIngredient', ingredient).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				if (!$scope.recipe.Id == 0)
					$scope.recipe.Id = data.RecipeId;
				ingredient.Id = data.IngredientId;
				$scope.recipe.Ingredients.push(ingredient);
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
		$scope.foodSearch = '';
		$scope.quantity = '';
		$scope.ingredientQuantityTypes = '';
	};

	$scope.updateIngredient = function (index) {
		$scope.saved = false;
		$http.post('/Home/InsertOrUpdateIngredient', $scope.recipe.Ingredients[index]).success(function (data) {
			$scope.saved = true;
			if (!data.Success) {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.addInstruction = function () {
		$scope.saved = false;
		var instruction = { Id: 0, Text: $scope.newInstruction, RecipeId: $scope.recipe.Id };
		$http.post('/Home/InsertOrUpdateInstruction', instruction).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				if (!$scope.recipe.Id == 0)
					$scope.recipe.Id = data.RecipeId;
				instruction.Id = data.InstructionId;
				$scope.recipe.Instructions.push(instruction);
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
		$scope.newInstruction = '';
	};

	$scope.updateInstruction = function (index) {
		$scope.saved = false;
		$http.post('/Home/InsertOrUpdateInstruction', $scope.recipe.Instructions[index]).success(function (data) {
			$scope.saved = true;
			if (!data.Success) {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.selectedFiles = [];

	$scope.onFileSelect = function($files, index) {
		$scope.uploadResult = [];
		$scope.selectedFile = index == 1 ? $files[0] : null;
		$scope.selectedFiles = index == 1 ? null : $files;
		for (var i = 0; i < $files.length; i++) {
			var $file = $files[i];
			$http.uploadFile({
				url: '/Home/UploadImages',
				data: {recipeId: $scope.recipe.Id},
				file: $file
			}).success(function(data, status, headers, config) {
				$scope.uploadResult.push(data.toString());
				$scope.getImages();
				// to fix IE not refreshing the model
				if (!$scope.$$phase) {
					$scope.$apply();
				}
			});
		}
	};

	$scope.getImages = function () {
		$http.get('/Home/GetRecipeImages?recipeId=' + $routeParams.recipeId).success(function (data) {
			$scope.images = data;
		}).error(function (data, status, headers, config) {
			alert("error");
		});
	};

	$scope.deleteIngredient = function (index) {
		$scope.saved = false;
		$http.post('/Home/DeleteIngredient/' + $scope.recipe.Ingredients[index].Id).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				$scope.recipe.Ingredients.splice(index, 1);
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.deleteInstruction = function (index) {
		$scope.saved = false;
		$http.post('/Home/DeleteInstruction/' + $scope.recipe.Instructions[index].Id).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				$scope.recipe.Instructions.splice(index, 1);
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
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
		angular.forEach($scope.list.Items, function (item) {
			item.checked = item.PickedUp;
		});
	}).error(function (data, status, headers, config) {
		alert("error");
	});

	$scope.remaining = function () {
		var count = 0;
		angular.forEach($scope.list.Items, function (item) {
			count += item.checked ? 0 : 1;
		});
		return count;
	};

	$scope.pickedUpChanged = function (index) {
		$http.post('/Home/ItemPickedUp?id=' + $scope.list.Items[index].Id + '&pickedUp=' + $scope.list.Items[index].checked).success(function (data) { })
			.error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
		});
	};
}