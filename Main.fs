[<RequireQualifiedAccess>]
module Main

open Elmish
open Elmish.React

Program.mkProgram Application.init Application.update Application.render
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withTrace Tracers.console
|> Program.withConsoleTrace
#endif
|> Program.run
