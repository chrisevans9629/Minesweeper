// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Minesweeper

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open System

module App = 
    //type Model = 
    //  { Count : int
    //    Step : int
    //    TimerOn: bool }
    //
    //type Msg = 
    //    | Increment 
    //    | Decrement 
    //    | Reset
    //    | SetStep of int
    //    | TimerToggled of bool
    //    | TimedTick
    //
    //let initModel = { Count = 0; Step = 1; TimerOn=false }
    //
    //let init () = initModel, Cmd.none
    //
    //let timerCmd =
    //    async { do! Async.Sleep 200
    //            return TimedTick }
    //    |> Cmd.ofAsyncMsg
    //
    //let update msg model =
    //    match msg with
    //    | Increment -> { model with Count = model.Count + model.Step }, Cmd.none
    //    | Decrement -> { model with Count = model.Count - model.Step }, Cmd.none
    //    | Reset -> init ()
    //    | SetStep n -> { model with Step = n }, Cmd.none
    //    | TimerToggled on -> { model with TimerOn = on }, (if on then timerCmd else Cmd.none)
    //    | TimedTick -> 
    //        if model.TimerOn then 
    //            { model with Count = model.Count + model.Step }, timerCmd
    //        else 
    //            model, Cmd.none
    //
    //let view (model: Model) dispatch =
    //    View.ContentPage(
    //      content = View.StackLayout(padding = Thickness 20.0, verticalOptions = LayoutOptions.Center,
    //        children = [ 
    //            View.Label(text = sprintf "%d" model.Count, horizontalOptions = LayoutOptions.Center, width=200.0, horizontalTextAlignment=TextAlignment.Center)
    //            View.Button(text = "Increment", command = (fun () -> dispatch Increment), horizontalOptions = LayoutOptions.Center)
    //            View.Button(text = "Decrement", command = (fun () -> dispatch Decrement), horizontalOptions = LayoutOptions.Center)
    //            View.Label(text = "Timer", horizontalOptions = LayoutOptions.Center)
    //            View.Switch(isToggled = model.TimerOn, toggled = (fun on -> dispatch (TimerToggled on.Value)), horizontalOptions = LayoutOptions.Center)
    //            View.Slider(minimumMaximum = (0.0, 10.0), value = double model.Step, valueChanged = (fun args -> dispatch (SetStep (int (args.NewValue + 0.5)))), horizontalOptions = LayoutOptions.FillAndExpand)
    //            View.Label(text = sprintf "Step size: %d" model.Step, horizontalOptions = LayoutOptions.Center) 
    //            View.Button(text = "Reset", horizontalOptions = LayoutOptions.Center, command = (fun () -> dispatch Reset), commandCanExecute = (model <> initModel))
    //        ]))
    type Model = 
        { 
            Game: Minesweeper.MinesweeperBase
            Flag: bool }
    type Msg = 
        | ToggleFlag
        | Flag of BaseCell
        | Tap of BaseCell
        | Reset
    let game = MinesweeperBase()
    let config = MinesweeperConfig()
    config.Rows <- System.Nullable(10)
    config.Columns <- System.Nullable(10)
    game.Setup(config)
    let init() = {Game=game;Flag=false}, Cmd.none
    let update msg model =
        match msg with
        | Flag f -> 
            model.Game.ClickOnCell(f, f.Flag <> true) |> ignore
            model, Cmd.none
        | Tap f -> 
            model.Game.ClickOnCell(f, false) |> ignore
            model, Cmd.none
        | Reset ->
            model.Game.Reset()
            model, Cmd.none
        | ToggleFlag -> { model with Flag = model.Flag <> true }, Cmd.none

    let bombimg = Source(ImageSource.FromResource("Minesweeper.bomb.png"))
    let flagimg = Source(ImageSource.FromResource("Minesweeper.flag.png"))
    let view (model: Model) dispatch =
        let endView =
            View.StackLayout(children=[
                View.Label(text=if model.Game.Win then "Congrats! You Won!" else "You lost")
                View.Label(text=sprintf "Your score was %d" model.Game.Score)
                View.Button(text="Reset", command=(fun () -> dispatch Reset))])
                .HorizontalOptions(LayoutOptions.Center)
                .VerticalOptions(LayoutOptions.Center)
                .Margin(Thickness(10.0))
                .BackgroundColor(Color.White)
                


        let header =
            View.Grid(
                coldefs=[Star;Star;Star;Star],
                children=[
                    View.Label(text= sprintf "%d Bombs" model.Game.Config.BombCount).FontSize(Named NamedSize.Large).Column(0)
                    View.Label(text= sprintf "Score: %d" model.Game.Score).FontSize(Named NamedSize.Large).Column(1)
                    View.Button(text=sprintf "Flag %b" model.Flag, command=(fun () -> dispatch ToggleFlag)).Padding(Thickness(10.0)).HorizontalOptions(LayoutOptions.Center).Column(2)
                    View.Button(text="Reset", command=(fun () -> dispatch Reset)).Padding(Thickness(10.0)).HorizontalOptions(LayoutOptions.Center).Column(3)
                    ])
        let cell (r:BaseCell) =
            if r.ShowBomb then
                View.Image(
                    source = bombimg,
                    horizontalOptions=LayoutOptions.Center,
                    verticalOptions=LayoutOptions.Center,
                    aspect=Aspect.AspectFit
                    ).WidthRequest(30.0).HeightRequest(30.0)
            else if r.ShowFlag then
                View.Image(
                    source = flagimg,
                    horizontalOptions=LayoutOptions.Center,
                    verticalOptions=LayoutOptions.Center,
                    aspect=Aspect.AspectFit
                    ).WidthRequest(30.0).HeightRequest(30.0)
            else
                View.Label(
                    text=if r.ShowEmpty then "0" 
                         else if r.ShowValue then r.Value.ToString() 
                         else if r.ShowBomb then "x" 
                         else if r.ShowFlag then "F" 
                         else " ")
                         .BackgroundColor(Color.LightBlue)
                         .ForegroundColor(Color.Black)
                         .HorizontalTextAlignment(TextAlignment.Center)
                         .VerticalTextAlignment(TextAlignment.Center)
                         .FontSize(Named NamedSize.Large)
                         .WidthRequest(30.0).HeightRequest(30.0)
        let mineSweeperGrid =
            View.Grid(
                rowdefs=[for i in 1 .. model.Game.Rows -> Dimension.Absolute 50.0], 
                coldefs=[for i in 1..model.Game.Columns -> Absolute 50.0],
                children=([
                    for r in model.Game.Cells -> (cell r).Row(r.Row).Column(r.Column).GestureRecognizers([
                             View.TapGestureRecognizer(command=(fun () -> dispatch (if model.Flag then Flag r else Tap r)))
                             ]) ] |> 
                        List.append (
                            if model.Game.GameEnd then 
                                [endView.RowSpan(model.Game.Rows).ColumnSpan(model.Game.Columns).Padding(Thickness(20.0))] 
                            else []))).Spacing(2.0)
        

        View.ContentPage(
            content=View.StackLayout(children=[header;View.ScrollView(content=mineSweeperGrid)])).Title("Mine Sweeper")
                            
    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()


