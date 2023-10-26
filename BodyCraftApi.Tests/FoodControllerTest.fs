namespace BodyCraftApi.Tests

open NUnit.Framework
open BodyCraftApi.Repositories
open BodyCraftApi.Controllers

[<TestFixture>]
type FoodControllerTest() =

    [<Test>]
    member this.TestMethodPassing() =
        let expected = "1"
        let foodRepository = FoodRepository()
        let foodController = FoodController(foodRepository)

        let actual = foodController.Get()


        Assert.AreEqual(expected, actual)
