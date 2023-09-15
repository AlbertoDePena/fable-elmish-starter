[<RequireQualifiedAccess>]
module Main

open Fable.Core.JsInterop

open Elmish
open Elmish.React
open Elmish.Navigation

importAll "bulma/css/bulma.min.css"

Program.mkProgram Application.init Application.update Application.render
|> Program.toNavigable (UrlParser.parseHash Application.parseUrl) Application.updateUrl
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
//|> Program.withConsoleTrace
|> Program.withTrace Tracers.console
#endif
|> Program.run
