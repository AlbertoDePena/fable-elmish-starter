[<RequireQualifiedAccess>]
module Main

open Elmish
open Elmish.React
open Elmish.Navigation

Program.mkProgram Application.init Application.update Application.render
|> Program.toNavigable (UrlParser.parseHash Router.parseUrl) Application.updateUrl
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withTrace Tracers.console
|> Program.withConsoleTrace
#endif
|> Program.run