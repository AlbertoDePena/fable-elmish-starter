module App

open Elmish
open Elmish.React
open Feliz

type State = { Count: int }

type Msg =
    | Increment
    | Decrement

let init () = { Count = 0 }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none

    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    let buttonStyle =
        "border border-blue-600 text-gray-200 rounded px-5 py-5 mx-2 bg-blue-500"

    Html.div
        [ prop.className "container mx-auto py-4 px-4"
          prop.children
              [ Html.button
                  [ prop.className buttonStyle
                    prop.onClick (fun _ -> dispatch Increment)
                    prop.text "Increment" ]

                Html.button
                    [ prop.className buttonStyle
                      prop.onClick (fun _ -> dispatch Decrement)
                      prop.text "Decrement" ]

                Html.h1 state.Count ] ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run
