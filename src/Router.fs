[<RequireQualifiedAccess>]
module Router

open Elmish
open Elmish.UrlParser

[<RequireQualifiedAccess>]
type Route =
    | Home
    | Blog of int

let toHash (route: Route) =
    match route with
    | Route.Home -> "#home"
    | Route.Blog id -> sprintf "#blog/%i" id

/// https://elmish.github.io/urlParser/docs/parsing.html
let parseRoute: Parser<Route -> Route, Route> =
    // combinator that takes nothing
    let matchNothing = UrlParser.top
    // combinator for a static string we expect to find in the URL
    let matchString = UrlParser.s
    // combinator to extract a string
    let parseString = UrlParser.str
    // combinator to attempt to parse and Int32
    let parseInt32 = UrlParser.i32

    UrlParser.oneOf [
        (matchString "blog" </> parseInt32) |> UrlParser.map (fun blogId -> Route.Blog blogId)
        (matchString "home") |> UrlParser.map Route.Home
        matchNothing |> UrlParser.map Route.Home
    ]
