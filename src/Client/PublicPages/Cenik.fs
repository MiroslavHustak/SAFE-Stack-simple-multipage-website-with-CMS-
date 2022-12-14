namespace PublicPages

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared
open SharedTypes

open Records.Client
open HtmlFeliz.Layout
open Auxiliaries.Client.SpaceChecker

module Cenik = 

    type Model =
        {
            CenikValues: GetCenikValues
            CenikInputValues: GetCenikValues
            ErrorMsg: string
            Id: int
        }

    type Msg =   
        | AskServerForCenikValues 
        | GetCenikValues of GetCenikValues    

    let private sendDeserialisedCenikValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let init id : Model * Cmd<Msg> =

        let model =
          {
              CenikValues = GetCenikValues.Default       
              CenikInputValues = GetCenikValues.Default
              ErrorMsg = String.Empty
              Id = id
          }
        model, Cmd.ofMsg AskServerForCenikValues

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        
        match msg with       
            | AskServerForCenikValues -> 
                 let loadEvent = SharedDeserialisedCenikValues.create model.CenikInputValues
                 let cmd = Cmd.OfAsync.perform sendDeserialisedCenikValuesApi.sendDeserialisedCenikValues loadEvent GetCenikValues
                 model, cmd            
            | GetCenikValues value -> { model with CenikValues =
                                                      {
                                                          Id = value.Id; ValueState = value.ValueState;
                                                          V001 = value.V001; V002 = value.V002; V003 = value.V003;
                                                          V004 = value.V004; V005 = value.V005; V006 = value.V006;
                                                          V007 = value.V007; V008 = value.V008; V009 = value.V009;
                                                          Msgs = value.Msgs
                                                      }
                                                   ErrorMsg = sprintf "%s %s %s" value.Msgs.Msg1 value.Msgs.Msg2 value.Msgs.Msg3
                                      }, Cmd.none    
 
    let view (model: Model) (dispatch: Msg -> unit) links =
    
        let cenikRecord =
           {
               Home = prop.className "normal"
               Sluzby = prop.className "normal"
               Cenik = prop.className "current"
               Nenajdete = prop.className "normal"
               Kontakt = prop.className "normal"
           }

        let cenikHtml =

            javaScriptMessageBox model.ErrorMsg

            Html.div [
                prop.method "get"
                prop.id "templatemo_content"
                prop.children [
                    Html.h1 [
                        prop.id 27
                        prop.children [
                            Html.text "Cen??k slu??eb"                   
                        ]
                    ]
                    Html.ul [
                        Html.li [
                            prop.id 28
                            prop.children [
                                Html.text "Vstupn?? vy??et??en?? a zhodnocen?? nutri??n??ho stavu pacienta, propo??ty a zhodnocen?? zaslan??ho j??deln????ku k vypln??n??"                       
                                Html.text " .................................... "
                                Html.text model.CenikValues.V001
                                Html.text ",- K??"
                            ]
                        ]
                        Html.li [
                            prop.id 29
                            prop.children [
                                Html.text "Asistovan?? n??kup s nutri??n??m terapeutem (60 min)"                      
                                Html.text " ........................ "
                                Html.text model.CenikValues.V002
                                Html.text ",- K??"
                            ]
                        ]
                    ]           
                    Html.br[]
                    Html.h3 [
                        prop.id 30
                        prop.text "Obezitologie"
                    ]
                    Html.ul [
                        Html.li [
                            prop.id 31
                            prop.children [
                                Html.text "Reduk??n?? bal????ek (individu??ln?? reduk??n?? pl??n, konzultace nutri??n??m terapeutem (45 min), reduk??n?? dieta, propo??ty a zhodnocen?? j??deln????ku, pitn?? re??im, pohybov?? re??im, eduka??n?? materi??ly)"
                                Html.text " ............................................ "
                                Html.text model.CenikValues.V003
                                Html.text ",- K??"
                            ]
                        ]
                        Html.li [
                            prop.id 32
                            prop.children [
                                Html.text "Kontroln?? konzultace"                        
                                Html.text " .................................................................... "
                                Html.text model.CenikValues.V004
                                Html.text ",- K??"
                            ]
                        ]
                        Html.li [
                            prop.id 33
                            prop.children [
                                Html.text "Sestaven?? j??deln??ho l??stku na t??den"                       
                                Html.text " ................................................ "
                                Html.text model.CenikValues.V005
                                Html.text ",- K??"
                            ]
                        ]
                        Html.li [
                            prop.id 34
                            prop.children [
                                Html.text "Sestaven?? j??deln??ho l??stku na 2 t??dny"                     
                                Html.text " ............................................. "
                                Html.text model.CenikValues.V006
                                Html.text ",- K??"
                            ]
                        ]
                        Html.li [
                            prop.id 35
                            prop.children [
                                Html.text "Sestaven?? j??deln??ho l??stku na 3 t??dny"                       
                                Html.text " ............................................. "
                                Html.text model.CenikValues.V007
                                Html.text ",- K??"
                            ]
                        ]
                    ]
                    Html.br[]
                    Html.h3 [
                        prop.id 36
                        prop.text "Diabetologie"
                    ]
                    Html.ul [
                        Html.li [
                            prop.id 37
                            prop.children [
                                Html.text "Edukace diety nutri??n??m terapeutem (cca 60 min)"                        
                                Html.text " ......................... "
                                Html.text model.CenikValues.V008
                                Html.text ",- K??"
                            ]
                        ]
                    ]
                    Html.br[]
                    Html.h3 [
                        prop.id 38
                        prop.text "Dietologie"
                    ]
                    Html.p [
                        prop.id 40
                        prop.style
                            [
                              style.fontStyle.italic
                              style.marginLeft(14)
                            ]
                        prop.children [
                            Html.text "Onemocn??n?? ??aludku, ??lu??n??ku, slivnivky b??i??n??, racion??ln?? strava, dna, onemocn??n?? st??ev, onemocn??n?? ledvin, celiakie, antikolagua??n?? l????ba, anorexie, chudokrevnost a jin??"
                        ]
                    ]
                    Html.ul [
                        Html.li [
                            prop.id 39
                            prop.children [
                                Html.text "Edukace dan?? diety nutri??n??m terapeutem (cca 60 min)"                       
                                Html.text " ................ "
                                Html.text model.CenikValues.V009
                                Html.text ",- K??"
                            ]
                        ]
                    ]
                ]
            ]

        layout <| cenikHtml <| cenikRecord <| links