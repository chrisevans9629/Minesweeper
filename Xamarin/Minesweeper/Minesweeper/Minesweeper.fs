﻿// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Minesweeper

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open System
open FFImageLoading.Forms

module App = 
    open SkiaSharp
    open SkiaSharp.Views.Forms
    type GridSize = {
        Size:float
        Zoom:float
        XTran:float
        YTran:float
        PanXStart:float
        PanYStart:float}
    type Model = 
        { 
            Game: Minesweeper.MinesweeperBase
            Flag: bool 
            Size:GridSize}
    type Msg = 
        | ToggleFlag
        | Flag of BaseCell
        | Tap of BaseCell
        | Reset
        | UpdateRow of Nullable<int>
        | UpdateColumn of Nullable<int>
        | UpdateBombs of int
        | SizeChanged of GridSize
        
    let game = MinesweeperBase()
    let config = MinesweeperConfig()
    config.Rows <- System.Nullable(20)
    config.Columns <- System.Nullable(20)
    config.BombCount <- 25
    game.Setup(config)
    let init() = {Game=game;Flag=false;Size={Size=50.;Zoom=1.;XTran=0.;YTran=0.;PanXStart=0.;PanYStart=0.}}, Cmd.none
     
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
        | SizeChanged g -> {model with Size=g}, Cmd.none
            
    let bombimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.bomb.png"))
    let flagimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.flag.png"))
    let dirtimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.dirt.jpg"))
    let dugimg = Source(EmbeddedResourceImageSource.FromResource("Minesweeper.dug.jpg"))
    let isInt p = 
        let mutable t = 0
        System.Int32.TryParse(p, &t)
    let parse a =
        let mutable t = 0
        System.Int32.TryParse(a, &t) |> ignore
        t
    let entry label text textChanged =
        View.StackLayout(
                orientation=StackOrientation.Vertical,
                children=[ 
                    View.Label(text=label)
                    View.Entry(
                        placeholder=label,
                        text=text,
                        textChanged = debounce 250 (fun e -> e.NewTextValue |> textChanged))])
    let MIN_SCALE = 1.
    let MAX_SCALE = 8.
    let OVERSHOOT = 0.15
    let StartX, StartY = 0.0,0.0
    let StartScale = 0.0

    let view (model: Model) dispatch =
        
        let handlePinch (p:PinchGestureUpdatedEventArgs) =
            if p.Status = GestureStatus.Started then
                dispatch (SizeChanged({model.Size with Zoom=model.Size.Zoom*p.Scale; XTran= p.ScaleOrigin.X;YTran=p.ScaleOrigin.Y}))
            if p.Status = GestureStatus.Running then
                dispatch (SizeChanged({model.Size with Zoom= model.Size.Zoom*p.Scale}))

        let handlePan (p:PanUpdatedEventArgs) =
            if p.StatusType = GestureStatus.Started then
                dispatch (SizeChanged({model.Size with PanXStart=model.Size.XTran;PanYStart=model.Size.YTran}))
            if p.StatusType = GestureStatus.Running then
                dispatch (SizeChanged({model.Size with XTran=model.Size.XTran+model.Size.PanXStart;YTran=model.Size.YTran+model.Size.PanYStart}))
                
        let skiaSharpGrid =
            View.SKCanvasView(enableTouchEvents=true,
                invalidate=true,
                height=1000.,
                width=1000.,
                paintSurface=(fun arg -> 
                    let canvas = arg.Surface.Canvas
                    canvas.Clear()

                    let columns = float32(model.Game.Columns)
                    let rows = float32(model.Game.Rows)

                    let cWidth = float32(arg.Info.Width)
                    let cHeight = float32(arg.Info.Height)

                    let size = Math.Min( float32(model.Size.Size), Math.Min(cWidth/columns,cHeight/rows))

                    let rowSizeHalf = rows * size/2.f
                    let columnSizeHalf = columns * size/2.f

                    let xOffset, yOffset = cWidth/2.f-columnSizeHalf, cHeight/2.f-rowSizeHalf


                    model.Game.Grid.SetDimensions(size,xOffset,yOffset)
                    use paint = new SKPaint()
                    paint.TextAlign <- SKTextAlign.Center

                    for t in model.Game.Cells do
                        paint.TextSize <- t.Width / 1.5f
                        
                        paint.Color <- if t.ShowEmpty || t.ShowValue then SKColors.Green else SKColors.LightGreen
                        paint.IsStroke <- false
                        canvas.DrawRect(t.X,t.Y,t.Width,t.Width, paint)
                        
                        paint.IsStroke <- true
                        paint.Color <- SKColors.Black
                        canvas.DrawRect(t.X,t.Y,t.Width,t.Width, paint)
                        paint.IsStroke <- false
                        if t.ShowValue || t.ShowFlag then
                            canvas.DrawText(t.DisplayValue(),SKPoint(t.X + (t.Width/2.f),t.Y + (t.Width/1.5f)), paint)
                    ),
                touch=(fun a -> 
                    if a.ActionType = SKTouchAction.Released then
                        let x = a.Location.X
                        let y = a.Location.Y
                        for t in model.Game.Cells do
                            if t.Hit(x,y) then if model.Flag then dispatch (Flag t) else dispatch (Tap t)
                    ))
                    .Scale(model.Size.Zoom)
                    .AnchorX(model.Size.XTran)
                    .AnchorY(model.Size.YTran)
                    //.GestureRecognizers([View.PinchGestureRecognizer(handlePinch);View.PanGestureRecognizer(handlePan)])
                        
                    
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
                orientation=StackOrientation.Vertical,
                spacing=10.,
                children=[
                    View.Label(text= sprintf "Bombs: %d" model.Game.Config.BombCount,verticalTextAlignment=TextAlignment.Center).Column(0)
                    View.Label(text= sprintf "Score: %d" model.Game.Score,verticalTextAlignment=TextAlignment.Center).Column(1)
                    View.Label(text="Flag:", verticalTextAlignment=TextAlignment.Center)
                    View.CheckBox(isChecked=model.Flag, checkedChanged=(fun a -> dispatch ToggleFlag)).Padding(Thickness(10.0)).HorizontalOptions(LayoutOptions.Center).Column(2)
                    entry "Rows" (model.Game.Config.Rows.ToString()) rows
                    entry "Columns" (model.Game.Config.Columns.ToString()) columns
                    entry "Bombs" (model.Game.Config.BombCount.ToString()) bombs
                    View.Button(text="Reset", command=(fun () -> dispatch Reset)).Padding(Thickness(10.0)).HorizontalOptions(LayoutOptions.Center).Column(3)
                    View.Label(text="Size:",verticalTextAlignment=TextAlignment.Center)
                    View.Slider(value=model.Size.Zoom,verticalOptions=LayoutOptions.CenterAndExpand, minimumMaximum=(0.5,4.), width=200., valueChanged=(fun args -> dispatch (SizeChanged ({model.Size with Zoom= args.NewValue}))))
                    ])

        View.MasterDetailPage(title="Minesweeper",masterBehavior=MasterBehavior.Popover,master=View.ContentPage(content=header,title="Settings"),
            detail=View.ContentPage(
                content=(if model.Game.GameEnd then 
                            endView 
                        else
                            View.ScrollView(
                                content=skiaSharpGrid,
                                orientation=ScrollOrientation.Both)
                                )).Title("Minesweeper"))
                            
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
    do runner.EnableLiveUpdate()
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

