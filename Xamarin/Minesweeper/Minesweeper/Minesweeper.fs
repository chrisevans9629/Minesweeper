// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Minesweeper

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open System
open FFImageLoading.Forms

module App = 
    type Model = 
        { 
            Game: Minesweeper.MinesweeperBase
            Flag: bool }
    type Msg = 
        | ToggleFlag
        | Flag of BaseCell
        | Tap of BaseCell
        | Reset
        | UpdateRow of Nullable<int>
        | UpdateColumn of Nullable<int>
        | UpdateBombs of int
        
    let game = MinesweeperBase()
    let config = MinesweeperConfig()
    config.Rows <- System.Nullable(10)
    config.Columns <- System.Nullable(10)
    game.Setup(config)
    let init() = {Game=game;Flag=false}, Cmd.none
    let update msg model =
        match msg with
        | Flag f -> 
            model.Game.ClickOnCell(f,true) |> ignore
            model, Cmd.none
        | Tap f -> 
            model.Game.ClickOnCell(f, false) |> ignore
            model, Cmd.none
        | Reset ->
            model.Game.Reset()
            model, Cmd.none
        | ToggleFlag -> { model with Flag = model.Flag <> true }, Cmd.none
        | UpdateBombs b -> 
            model.Game.Config.BombCount <- b
            model, Cmd.none
        | UpdateRow b ->
            model.Game.Config.Rows <- b
            model, Cmd.none
        | UpdateColumn b ->
            model.Game.Config.Columns <- b
            model, Cmd.none
            
    let bombimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.bomb.png"))
    let flagimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.flag.png"))
    let dirtimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.dirt.jpg"))
    let dugimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.dug.jpg"))
    let isInt p = 
        let mutable t = 0
        System.Int32.TryParse(p, &t)
    let parse a =
        System.Int32.Parse(a)
    let entry label text textChanged =
        View.StackLayout(
                orientation=StackOrientation.Vertical,
                children=[
                    View.Label(text=label)
                    View.Entry(
                        placeholder=label,
                        text=text,
                        textChanged = debounce 250 (fun e -> e.NewTextValue |> textChanged))])
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
        let rows p = dispatch (UpdateRow (Nullable(parse(p))))
        let columns p = dispatch (UpdateColumn (Nullable(parse(p))))
        let bombs p = dispatch (UpdateBombs (parse(p)))
        let header =
            View.StackLayout(
                orientation=StackOrientation.Horizontal,
                children=[
                    View.Label(text= sprintf "%d Bombs" model.Game.Config.BombCount).FontSize(Named NamedSize.Large).Column(0)
                    View.Label(text= sprintf "Score: %d" model.Game.Score).FontSize(Named NamedSize.Large).Column(1)
                    View.Label(text="Flag:")
                    View.CheckBox(isChecked=model.Flag, checkedChanged=(fun a -> dispatch ToggleFlag)).Padding(Thickness(10.0)).HorizontalOptions(LayoutOptions.Center).Column(2)
                    entry "Rows" (model.Game.Config.Rows.ToString()) rows
                    entry "Columns" (model.Game.Config.Columns.ToString()) columns
                    entry "Bombs" (model.Game.Config.BombCount.ToString()) bombs
                    View.Button(text="Reset", command=(fun () -> dispatch Reset)).Padding(Thickness(10.0)).HorizontalOptions(LayoutOptions.Center).Column(3)
                    ])
        let cell (r:BaseCell) =
            View.Grid(
                children=[
                    View.CachedImage(
                        source = (if r.ShowBomb then 
                                    bombimg 
                                  else if r.ShowFlag then 
                                    flagimg 
                                  else if r.ShowValue || r.ShowEmpty then 
                                    dugimg 
                                  else dirtimg),
                        horizontalOptions=LayoutOptions.CenterAndExpand,
                        verticalOptions=LayoutOptions.CenterAndExpand,
                        aspect=Aspect.AspectFit
                        )//.WidthRequest(30.0).HeightRequest(30.0)
                    View.Label(
                        isVisible=r.ShowValue,
                        text=if r.ShowValue then r.Value.ToString() 
                             else " ")
                             //.BackgroundColor(if r.ShowEmpty || r.ShowValue then Color.LightGreen else Color.LightBlue)
                             //.ForegroundColor(Color.Black)
                             .HorizontalTextAlignment(TextAlignment.Center)
                             .VerticalTextAlignment(TextAlignment.Center)
                             .FontSize(Named NamedSize.Large)
                             .WidthRequest(30.0).HeightRequest(30.0)
                        ])
                
        let mineSweeperGrid =
            View.Grid(
                rowdefs=[for i in 1 .. model.Game.Rows -> Absolute 50.0], 
                coldefs=[for i in 1..model.Game.Columns -> Absolute 50.0],
                children=([
                    for r in model.Game.Cells -> 
                        (cell r)
                            .Row(r.Row)
                            .Column(r.Column)
                            .GestureRecognizers([ View.TapGestureRecognizer(command=(fun () -> dispatch (if model.Flag then Flag r else Tap r)))
                                ]) ])).Spacing(0.)
        

        View.ContentPage(
            content=View.StackLayout(
                children=[
                    header
                    if model.Game.GameEnd then 
                        endView 
                    else
                        View.ScrollView(
                            content=mineSweeperGrid,
                            verticalScrollBarVisibility=ScrollBarVisibility.Always,
                            horizontalScrollBarVisibility=ScrollBarVisibility.Always)])).Title("Mine Sweeper")
                            
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
    //let modelId = "model"
    //override __.OnSleep() = 
    //
    //    let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
    //    Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)
    //
    //    app.Properties.[modelId] <- json
    //
    //override __.OnResume() = 
    //    Console.WriteLine "OnResume: checking for model in app.Properties"
    //    try 
    //        match app.Properties.TryGetValue modelId with
    //        | true, (:? string as json) -> 
    //
    //            Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
    //            let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)
    //
    //            Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
    //            runner.SetCurrentModel (model, Cmd.none)
    //
    //        | _ -> ()
    //    with ex -> 
    //        App.program.onError("Error while restoring model found in app.Properties", ex)
    //
    //override this.OnStart() = 
    //    Console.WriteLine "OnStart: using same logic as OnResume()"
    //    this.OnResume()
    //

