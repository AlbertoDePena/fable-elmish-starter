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

let parseRoute: Parser<Route -> Route, Route> =
    UrlParser.oneOf [ // Auth Routes
        UrlParser.map (fun blogId -> Route.Blog blogId) (UrlParser.s "blog" </> UrlParser.i32)
        UrlParser.map Route.Home (UrlParser.s "home")
        UrlParser.map Route.Home UrlParser.top
    ]