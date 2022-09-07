[<RequireQualifiedAccess>]
module Router

open Elmish
open Elmish.React
open Elmish.Navigation
open Elmish.UrlParser

let toHash (url: Application.Url) =
    match url with
    | Application.Url.Home -> "#home"
    | Application.Url.Blog id -> $"#blog/{id}"

let parseUrl: Parser<Application.Url -> Application.Url, Application.Url> =
    oneOf [ // Auth Routes
        map (fun domainId -> Application.Url.Blog domainId) (s "blog" </> i32)
        map Application.Url.Home (s "home")        
        map Application.Url.Home top
    ]