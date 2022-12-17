[<RequireQualifiedAccess>]
module Main

open Elmish
open Elmish.React
open Elmish.Navigation

open ADP.Fable

Program.mkProgram Application.init Application.update Application.render
|> Program.toNavigable (UrlParser.parseHash Router.parseUrl) Application.updateUrl
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withTrace Tracers.console
|> Program.withConsoleTrace
#endif
|> Program.run