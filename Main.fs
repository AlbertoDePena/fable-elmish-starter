module App

open Elmish
open Elmish.React
open Feliz

type State =
    { Count: int
      Random: Deferred<double> }

type Msg =
    | Increment
    | Decrement
    | GenerateRandomNumber of MsgEvent<double>

let random = System.Random()

let init () =
    { Count = 0; Random = HasNotStartedYet }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none

    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

    | GenerateRandomNumber Started ->
        let generateRandom =
            async {
                do! Async.Sleep 1000
                let randomNumber = random.NextDouble()
                return GenerateRandomNumber(Finished randomNumber)
            }

        { state with Random = InProgress }, Cmd.fromAsync generateRandom

    | GenerateRandomNumber (Finished randomNumber) -> { state with Random = Resolved randomNumber }, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    let buttonStyle =
        "border border-blue-600 text-gray-200 rounded px-5 py-5 mx-2 bg-blue-500"

    Html.div [
        prop.className "container mx-auto py-4 px-4"
        prop.children [
            Html.button [
                prop.className buttonStyle
                prop.onClick (fun _ -> dispatch Increment)
                prop.text "Increment"
            ]

            Html.button [
                prop.className buttonStyle
                prop.onClick (fun _ -> dispatch Decrement)
                prop.text "Decrement"
            ]

            Html.button [
                prop.className buttonStyle
                prop.onClick (fun _ -> dispatch (GenerateRandomNumber Started))
                prop.text "Random Number"
            ]

            Html.h1 state.Count

            match state.Random with
            | HasNotStartedYet -> Html.none
            | InProgress -> Html.h1 "Please wait..."
            | Resolved randomNumber -> Html.h1 randomNumber
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withTrace Tracers.console
#endif
|> Program.run
