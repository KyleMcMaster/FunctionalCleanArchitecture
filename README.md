# FP for OO Devs lunch-n-learn 07/19

Disclaimer: We're all learning here, even me! So there may be concepts, terminology, etc I don't know! 

# Goals
- Refactor away from nulls
- Refactor away from _throwing_ exceptions
- Making failures explicit
- Improving the readability of code
- Having fun!

Although not a goal specifically, refactoring to functions as the unit of programming versus classes is a natural consequence of the above goals in FP (although C# fights this).

## Introduce CSharpFunctionalExtensions
  - Introduce Vladimir Khorikov, DDD, TDD, and Unit Testing expert
  - [Vladimir Khorikov Pluralsight](https://www.pluralsight.com/authors/vladimir-khorikov)
  - Unit Testing Book
  - It's a library similar to Ardalis.Result but expanded for a wider variety of use cases and FP support

## Monads (simplified)
  - A structure for representing a value or operation that can be combined with functional operations without breaking the contract of the monad itself
  - [wiki](https://en.wikipedia.org/wiki/Monad_(functional_programming))
  - Result (compare to [Ardalis.Result](https://github.com/ardalis/Result/blob/0f54000ed05addaaadd973f28cc08bf131af788f/sample/Ardalis.Result.Sample.Core/Services/WeatherService.cs#L84))
  - Maybe
  - [Other C# Monads we already use](https://ericlippert.com/category/monads/page/2/)

## Railway Oriented Programming
  - Result<T, E> T value, E error
  - Projects.GetById for a simple introduction, Maybe and Result
  - Project.Update endpoint
  - My favorite feature of this library
  - Enables developers to define recipes and tell stories using concise method chaining operations
  - Focus on making failures explicit, avoiding exceptions, and staying inside the higher-order function mindset

## Gotchas
  - Are we using pure functions? Does Dependency injection break this?
    - Show simple example with Project.GetById, no currying/partial application needed
    - partial application/currying in update name
  - How to deal with Domain events?
    - Project.CreateToDoItem example
  - This style produces a lot of methods which at first seems bad but really isn't ... because functions are first-class citizens in this paradigm 
    - How to deal with them? organize in static classes similar to extensions, group by feature, use generics, etc, nothing has to change there 
  - Lots of noise with type parameters, due to many overloads and well.... the C# type system not being as strong at inference as say, F#
  - Any others??


## Notes about the demo
  - Absolutely abusing exceptions, better practice would be to model failures just like successful outcomes using something like a SmartEnum
  - In OOP/FP hybrid architectures, not everything has to be immutable, support for encapsulation even with immutability
    - In C#, classes are the unit for grouping data and behavior, whether in separate or single classes
  - Newer C# features allow for friendlier functional expressions
    - Pattern matching with switches, example of generic error handler

## Takeaways
  - Other examples we'd like to see?
  - Functional Architecture demo? CQRS?
  - F# lunch-n-learn?
  - opinions?


## Randomness
[F# goodness](https://github.com/NitroDevs/FShopOnWeb/blob/main/src/Microsoft.eShopWeb.Web/Home/Home.Page.fs)
[Currying and Uncurrying](https://medium.com/@adambene/currying-in-javascript-es6-540d2ad09400)
[Principles of Functional Programming](https://dev.to/jamesrweb/principles-of-functional-programming-4b7c)
[Functional programming principles](https://medium.com/@kumbhar.pradnya/functional-programming-principles-6f59bc6764ff)

## Resources

[Title](https://github.com/vkhorikov/CSharpFunctionalExtensions)
[Title](https://www.pluralsight.com/authors/vladimir-khorikov)
[Title](https://github.com/ardalis/CleanArchitecture/blob/main/src/Clean.Architecture.Web/Endpoints/ProjectEndpoints/GetById.cs)
[Title](https://en.wikipedia.org/wiki/Monad_(functional_programming))
[Title](https://ericlippert.com/category/monads/page/2/)
[Title](https://www.pluralsight.com/courses/functional-architecture-fsharp)
[Title](https://github.com/ardalis/Result/blob/main/sample/Ardalis.Result.Sample.Core/Services/WeatherService.cs#L84)