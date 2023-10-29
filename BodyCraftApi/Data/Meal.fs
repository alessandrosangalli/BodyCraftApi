namespace BodyCraftApi.Data

open System

type Meal = { Food: Food[]; Time: TimeSpan }

type MealToInsert =
    | Meal
    | FoodWithId
