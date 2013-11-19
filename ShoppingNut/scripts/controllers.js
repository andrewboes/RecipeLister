'use strict';

function recipeListCtrl($scope, $http, $routeParams) {
	var url = '/Home/GetUserRecipes';
	if ($routeParams.all)
		url = '/Home/GetAllRecipes';
	$http.get(url).success(function (data) {
		$scope.recipes = data;
		angular.forEach($scope.recipes, function (recipe) {
			if (recipe.Images.length === 0)
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
			angular.forEach(list.Items, function (item) {
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

function recipeAddCtrl($scope, $http, $location, $routeParams, $modal, $log) {
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
				if ($scope.recipe.Id === 0)
					$scope.recipe.Id = data.RecipeId;
					$location.path('/recipes/edit/' + $scope.recipe.Id);
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
			RecipeId: $scope.recipe.Id,
			Notes: $scope.newIngredientNotes
		};
		$http.post('/Home/InsertOrUpdateIngredient', ingredient).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				if ($scope.recipe.Id === 0) {
					$scope.recipe.Id = data.RecipeId;
					$location.path('/recipes/edit/' + $scope.recipe.Id);
				}
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
		$scope.newIngredientNotes = ''
		$scope.currentSelectedFood = '';
	};

	$scope.updateIngredient = function (index) {
		$scope.saved = false;
		$http.post('/Home/InsertOrUpdateIngredient', $scope.recipe.Ingredients[index]).success(function (data) {
			$scope.saved = true;
			var quantityTypes = $scope.recipe.Ingredients[index].Food.QuantityTypes;
			for (var i = 0; i < quantityTypes.length; i++) {
				if (quantityTypes[i].Id === $scope.recipe.Ingredients[index].QuantityTypeId)
					$scope.recipe.Ingredients[index].QuantityType = quantityTypes[i];
			}
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
				if ($scope.recipe.Id === 0) {
					$scope.recipe.Id = data.RecipeId;
					$location.path('/recipes/edit/' + $scope.recipe.Id);
				}
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

	$scope.onFileSelect = function ($files, index) {
		$scope.uploadResult = [];
		$scope.selectedFile = index == 1 ? $files[0] : null;
		$scope.selectedFiles = index == 1 ? null : $files;
		for (var i = 0; i < $files.length; i++) {
			var $file = $files[i];
			$http.uploadFile({
				url: '/Home/UploadImages',
				data: { recipeId: $scope.recipe.Id },
				file: $file
			}).success(function (data, status, headers, config) {
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

	$scope.foodSelected = function ($item) {
		$scope.currentSelectedFood = $item;
		$http.get('/Home/GetFoodQuantityTypes?id=' + $item.Id).success(function (data) {
			$scope.currentSelectedFood.QuantityTypes = data;
			$scope.quantityType = $scope.currentSelectedFood.QuantityTypes[0];
		});
	};

	$scope.foods = function (foodQuery) {
		if (foodQuery !== null && foodQuery.length > 2) {
			return $http.get('/Home/GetFoods?filter=' + foodQuery).then(function (response) {
				return response.data;
			});
		} else {
			return [];
		}
	};

	$scope.open = function () {

		var modalInstance = $modal.open({
			templateUrl: 'html/foodAdd.html',
			controller: newFoodCtrl,
			resolve: {
				items: function () {
					return $scope.items;
				}
			}
		});

		modalInstance.result.then(function (selectedItem) {
			$scope.foodSearch = selectedItem.Name;
			$scope.currentSelectedFood = selectedItem;
			$scope.ingredientQuantityTypes = selectedItem.QuantityTypes;
		}, function () {
			$log.info('Modal dismissed at: ' + new Date());
		});
	};
}

function newFoodCtrl($scope, $modalInstance, $http) {
	$scope.food = { Name: '', FoodGroup: {}, QuantityTypes: [], Id: 0 };
	$http.get('/Home/GetFoodAllGroups').success(function (data) {
		$scope.foodGroups = data;
	}).error(function (data, status, headers, config) {
		alert("error");
	});

	$scope.ok = function () {
		var valid = true;
		if ($scope.food.Name == '') {
			alert("Enter a name");
			valid = false;
		}
		if (!$scope.food.FoodGroup.Description) {
			alert("Select a food group");
			valid = false;
		}
		if ($scope.food.QuantityTypes.length === 0) {
			alert("One quantity type is required");
			valid = false;
		}
		if (valid) {
			$http.post('/Home/InsertFood', $scope.food).success(function (data) {
				if(data.Success)
					$modalInstance.close(data.object);
				else {
					alert(data.Message);
				}
			}).error(function (data, status, headers, config) {
				var message = "Error: " + data;
				alert(message);
			});
		}
	};

	$scope.cancel = function () {
		$modalInstance.dismiss('cancel');
	};
	$scope.addQuantity = function (name, grams) {
		var qt =
		{
			Name: name,
			Grams: grams
		};
		$scope.food.QuantityTypes.push(qt);
		this.newQuantityName = '';
		this.newQuantityGrams = '';
	};
};

function listAddCtrl($scope, $http, $routeParams) {
	var id = $routeParams.listId;
	if (id) {
		$http.get('/Home/GetListById?id=' + id).success(function (data) {
			$scope.list = data;
		}).error(function (data, status, headers, config) {
			alert("Error couldn't get list with Id" + id);
		});
	}
	else {
		$scope.list = { Name: '', Items: [], Id: 0 };
	}
	$scope.saved = true;

	$http.get('/Home/GetAllRecipesWithIngredients').success(function (data) {
		$scope.recipes = data;
	}).error(function (data, status, headers, config) {
		alert("error");
	});

	$scope.addIngredientsForRecipe = function (index) {
		var recipeIngredients = $scope.recipes[index];
		for (var i = 0; i < recipeIngredients.Ingredients.length; i++) {
			$scope.addIngredient(index, i);
		}
	};

	$scope.addIngredient = function (recipeIndex, itemIndex) {
		var ingredient = $scope.recipes[recipeIndex].Ingredients[itemIndex];
		var updated = $scope.updateExisting(ingredient);
		if (!updated) {
			var item = {Quantity: ingredient.Quantity,
				QuantityType: ingredient.QuantityType,
				QuantityTypeId: ingredient.QuantityTypeId,
				Name: ingredient.Food.Name,
				FoodId: ingredient.FoodId,
				ShoppingListId: $scope.list.Id
			};
			$scope.addItemFromRecipe(item);
		}
	};

	$scope.updateExisting = function(ingredient) {
		var updated = false;
		for (var i = 0; i < $scope.list.Items.length; i++) {
			var itemToCheck = $scope.list.Items[i];
			if (itemToCheck.FoodId == ingredient.FoodId && itemToCheck.QuantityTypeId == ingredient.QuantityTypeId) {
				updated = true;
				$scope.saved = false;
				itemToCheck.ShoppingListId = $scope.list.Id;
				itemToCheck.Quantity += ingredient.Quantity;
				$http.post('/Home/InsertOrUpdateShoppingListItem', itemToCheck).success(function(data) {
					if (!data.Success) {
						itemToCheck.Quantity -= ingredient.Quantity;
						alert(data.Message);
					}
					$scope.saved = true;
				}).error(function(data, status, headers, config) {
					var message = "Error: " + data;
					alert(message);
					$scope.saved = true;
				});
			}
		}
		return updated;
	};

	$scope.addItemFromRecipe = function (item) {
		$http.post('/Home/InsertOrUpdateShoppingListItem', item).success(function (data) {
			if (data.Success) {
				$scope.list.Id = data.ShoppingListId;
				item.Id = data.ShoppingListItemId;
				$scope.list.Items.push(item);
			}
			else {
				alert(data.Message);
			}
			$scope.saved = true;
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
			$scope.saved = true;
		});
	};

	$scope.addItem = function () {
		$scope.saved = false;
		var item = {
			Name: $scope.foodSearch,
			Quantity: $scope.quantity,
			QuantityType: $scope.quantityType,
			ShoppingListId: $scope.list.Id
		};
		if ($scope.currentSelectedFood)
			item.FoodId = $scope.currentSelectedFood.Id;
		if ($scope.quantityType) {
			item.QuantityTypeId = $scope.quantityType.Id;
		}

		var updated = $scope.updateExisting(item);

		if (!updated) {
			$http.post('/Home/InsertOrUpdateShoppingListItem', item).success(function (data) {
				if (data.Success) {
					$scope.foodSearch = '';
					$scope.quantity = '';
					$scope.ingredientQuantityTypes = '';
					$scope.quantityType = '';
					$scope.currentSelectedFood = '';
					$scope.list.Id = data.ShoppingListId;
					item.Id = data.ShoppingListItemId;
					$scope.list.Items.push(item);
				}
				else {
					alert(data.Message);
				}
				$scope.saved = true;
			}).error(function (data, status, headers, config) {
				var message = "Error: " + data;
				alert(message);
				$scope.saved = true;
			});
		}
	};

	$scope.insertOrUpdateList = function () {
		$scope.saved = false;
		var list = { Name: $scope.list.Name, Id: $scope.list.Id };
		$http.post('/Home/InsertOrUpdateShoppingList', list).success(function (data) {
			if (data.Success)
				$scope.list.Id = data.ShoppingListId;
			else {
				alert(data.Message);
			}
			$scope.saved = true;
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
			$scope.saved = true;
		});
	};

	$scope.insertOrUpdateItem = function () {

	};

	$scope.foods = function (foodQuery) {
		if (foodQuery !== null && foodQuery.length > 2) {
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

	$scope.deleteItem = function (index) {
		console.log(index);
		$scope.saved = false;
		$http.post('/Home/DeleteShoppingListItem/' + $scope.list.Items[index].Id).success(function (data) {
			$scope.saved = true;
			if (data.Success) {
				$scope.list.Items.splice(index, 1);
			} else {
				alert(data);
			}
		}).error(function(data, status, headers, config) {
			$scope.saved = true;
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.finishedSorting = function (index, newOrder) {
		console.log('finished, ' + index + ', ' + newOrder);
	};
}

function listDetailCtrl($scope, $http, $routeParams, $location) {
	$scope.currentRole = '';
	$scope.orderProp = 'Group.Description';
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

	$scope.delete = function() {
		if (confirm("Are you sure you want to delete this list?")) {
			$http.post('/Home/DeleteList/' + $scope.list.Id).success(function(data) {
				if (data.Success)
					$location.path('/lists');
				else {
					alert(data.Message);
				}
			}).error(function (data, status, headers, config) {
				alert("error");
			});
		}
	};

	$scope.createGroupHeader = function(description) {
		var showHeader = (description != $scope.currentRole);
		$scope.currentRole = description;
		return showHeader;
	};
}

function recipeCopyCtrl($scope, $http) {
	$scope.recipe = { Name: '', Description: '', CaloriesPerServing: '', Ingredients: [], Instructions: [], Id: 0 };

	$scope.scrapePage = function () {
		$http.get('/Home/Scrape?url=' + $scope.url).success(function (data) {
			$scope.rawRecipe = data;
			if (data.Instructions)
				$scope.recipe.Instructions = data.Instructions;
			if (data.Name)
				$scope.recipe.Name = data.Name;
			if (data.Servings)
				$scope.recipe.Servings = data.Servings;
			if (data.Description)
				$scope.recipe.Description = data.Description;
			if (data.Url)
				$scope.recipe.Url = data.Url;
			$scope.saveRecipe();
		});
	};

	$scope.foods = function (foodQuery) {
		if (foodQuery !== null && foodQuery.length > 2) {
			return $http.get('/Home/GetFoods?filter=' + foodQuery).then(function (response) {
				return response.data;
			});
		} else {
			return [];
		}
	};
	
	$scope.foodSelected = function (item, currentIngredient) {
		//$scope.recipe.Ingredients[index].currentSelectedFood = item;
		//ingredient.
		$http.get('/Home/GetFoodQuantityTypes?id=' + item.Id).success(function (data) {
			currentIngredient.currentSelectedFood = item;
			currentIngredient.currentSelectedFood.QuantityTypes = data;
			currentIngredient.quantityType = data[0];
		});

		//var currentIngredient = $scope.recipe.Ingredients[index];
		//currentIngredient.currentSelectedFood = item;
		//$http.get('/Home/GetFoodQuantityTypes?id=' + item.Id).success(function (data) {
		//	currentIngredient.currentSelectedFood.QuantityTypes = data;
		//	currentIngredient.quantityType = currentIngredient.currentSelectedFood.QuantityTypes[0];
		//});
	};

	$scope.saveRecipe = function () {
		$http.post('/Home/InsertOrUpdateRecipe', $scope.recipe).success(function (data) {
			if (data.Success) {
				if ($scope.recipe.Id === 0)
					$scope.recipe.Id = data.RecipeId;
				for (var i = 0; i < $scope.recipe.Instructions.length; i++) {
					var text = $scope.recipe.Instructions[i];
					var instruction = { Id: 0, Text: text, RecipeId: $scope.recipe.Id };
					$scope.insertOrUpdateInstruction(instruction);
				}
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.insertOrUpdateInstruction = function (instruction) {
		$http.post('/Home/InsertOrUpdateInstruction', instruction).success(function (data) {
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
		});
	};

	$scope.saveIngredient = function (ingredientInput) {
		if(!ingredientInput.Id)
			ingredientInput.Id = 0;
		var ingredient = {
			Food: ingredientInput.foodSearch,
			Quantity: ingredientInput.quantity,
			QuantityType: ingredientInput.quantityType,
			FoodId: ingredientInput.currentSelectedFood.Id,
			QuantityTypeId: ingredientInput.quantityType.Id,
			RecipeId: $scope.recipe.Id,
			Notes: ingredientInput.newIngredientNotes,
			Id: ingredientInput.Id
		};
		$http.post('/Home/InsertOrUpdateIngredient', ingredient).success(function (data) {
			if (data.Success) {
				ingredient.Id = data.IngredientId;
			} else {
				alert(data);
			}
		}).error(function (data, status, headers, config) {
			var message = "Error: " + data;
			alert(message);
		});
	};
}