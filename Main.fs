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
    | GenerateRandomNumber of AsyncMsg<double>

let random = System.Random()

let init () =
    { Count = 0; Random = Deferred.HasNotStartedYet }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none

    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

    | GenerateRandomNumber AsyncMsg.Started ->
        let generateRandom () =
            async {
                do! Async.Sleep 1000
                let randomNumber = random.NextDouble()
                return randomNumber
            }

        let command =
            Cmd.OfAsyncImmediate.perform generateRandom () (AsyncMsg.Finished >> GenerateRandomNumber)

        { state with Random = Deferred.InProgress }, command

    | GenerateRandomNumber (AsyncMsg.Finished randomNumber) -> { state with Random = Deferred.Resolved randomNumber }, Cmd.none

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
                prop.onClick (fun _ -> dispatch (GenerateRandomNumber AsyncMsg.Started))
                prop.text "Random Number"
            ]

            Html.h1 state.Count

            match state.Random with
            | Deferred.HasNotStartedYet -> Html.none
            | Deferred.InProgress -> Html.h1 "Please wait..."
            | Deferred.Resolved randomNumber -> Html.h1 randomNumber
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withTrace Tracers.console
|> Program.withConsoleTrace
#endif
|> Program.run
