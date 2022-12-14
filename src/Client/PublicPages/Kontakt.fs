namespace PublicPages

open System

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared
open SharedTypes

open Records.Client
open HtmlFeliz.Layout
open Auxiliaries.Client.SpaceChecker

module Kontakt =

    type Model =
        {
            KontaktValues: GetKontaktValues
            KontaktInputValues: GetKontaktValues
            ErrorMsg: string
            Id: int
        }

    type Msg =   
        | AskServerForKontaktValues 
        | GetKontaktValues of GetKontaktValues    

    let private sendDeserialisedKontaktValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let init id : Model * Cmd<Msg> =
       
        let model =        
            {
                KontaktValues = GetKontaktValues.Default        
                KontaktInputValues = GetKontaktValues.Default
                ErrorMsg = String.Empty
                Id = id
            }
        model, Cmd.ofMsg AskServerForKontaktValues

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        
        match msg with       
            | AskServerForKontaktValues ->
                 let loadEvent = SharedDeserialisedKontaktValues.create model.KontaktInputValues
                 let cmd = Cmd.OfAsync.perform sendDeserialisedKontaktValuesApi.sendDeserialisedKontaktValues loadEvent GetKontaktValues
                 model, cmd            
            | GetKontaktValues value -> { model with KontaktValues =
                                                        {
                                                            V001 = value.V001; V002 = value.V002; V003 = value.V003;
                                                            V004 = value.V004; V005 = value.V005; V006 = value.V006;
                                                            V007 = value.V007; Msgs = value.Msgs 
                                                        }
                                                     ErrorMsg = sprintf "%s %s %s" value.Msgs.Msg1 value.Msgs.Msg2 value.Msgs.Msg3
                                        }, Cmd.none    
 
    let view (model: Model) (dispatch: Msg -> unit) links =
    
        let kontaktRecord =
           {
               Home = prop.className "normal"
               Sluzby = prop.className "normal"
               Cenik = prop.className "normal"
               Nenajdete = prop.className "normal"
               Kontakt = prop.className "current"
           }

        let kontaktHtml =

            javaScriptMessageBox model.ErrorMsg
    
            Html.div [
                prop.id "templatemo_content"
                prop.children [
                    Html.h2 [
                        prop.id 16
                        prop.children [
                            Html.text "Kontakt na ambulanci nutri??n?? terapie"
                        ]
                    ]
                    Html.div [
                        Html.p [
                            prop.id 17
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V001
                            ]
                        ]
                        Html.p [
                            prop.id 18
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V002
                            ]
                        ]
                        Html.p [
                            prop.id 19
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V003                    
                            ]
                        ]
                        Html.p [
                            prop.id 20
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V004                
                            ]
                        ]
                        Html.p [
                            prop.id 21
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V005                            
                            ]
                        ]
                        Html.p [
                            prop.id 21
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V006                            
                            ]
                        ]
                        Html.p [
                            prop.id 21
                            prop.style
                               [
                                 style.padding(0)
                               ]
                            prop.children [
                               Html.text model.KontaktValues.V007                            
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "body2"
                        prop.children [
                            Html.p [
                                prop.className "editable"
                                prop.id 24
                                prop.style
                                    [
                                      style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "-----------------------------------------------------------------------------------------"
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 25
                                prop.style
                                    [
                                      style.padding(0)
                                    ]
                                prop.children [
                                     Html.text "I??: 02021820"
                                     Html.br []
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 26
                                prop.style
                                    [
                                        style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "Fyzick?? osoba zapsan?? v ??ivnostensk??m rejst????ku od 10.10.2012"
                                    Html.br []
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 27
                                prop.style
                                    [
                                        style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "P??edm??t podnik??n??: V??roba, obchod a slu??by neuveden?? v p????loh??ch 1 a?? 3 ??ivnostensk??ho z??kona"       
                                    Html.br []
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 28
                                prop.style
                                    [
                                      style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "Vznik opr??vn??n?? dne 21.08.2013"
                                    Html.br []
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "cleaner_h50"
                    ]
                    Html.div [
                        Html.img [
                            prop.src "/Content/images/Telefon.png"
                            prop.width 340
                            prop.height 279
                            prop.alt "telefon"
                        ]
                    ]
                ]
            ]     
    
        layout <| kontaktHtml <| kontaktRecord <| links