namespace PublicPages

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared

open Records.Client
open HtmlFeliz.Layout
open Helpers.Client.Helper

module Cenik = 

    type Model =
        {
            CenikValues: CenikValuesShared
            CenikInputValues: CenikValuesShared
            ErrorMsg: string
            Id: int
        }

    type Msg =   
        | AskServerForCenikValues 
        | GetCenikValues of CenikValuesShared    

    let private getDeserialisedCenikValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let internal init id : Model * Cmd<Msg> =

        let model =
           {
               CenikValues = SharedCenikValues.cenikValuesDomainDefault     
               CenikInputValues = SharedCenikValues.cenikValuesDomainDefault
               ErrorMsg = String.Empty
               Id = id
           }
        model, Cmd.ofMsg AskServerForCenikValues

    let internal update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
        
        match msg with       
        | AskServerForCenikValues
            -> 
             let loadEvent = SharedDeserialisedCenikValues.transformLayer model.CenikInputValues
             let cmd = Cmd.OfAsync.perform getDeserialisedCenikValuesApi.getDeserialisedCenikValues loadEvent GetCenikValues
             model, cmd
                
        | GetCenikValues value
            ->
             {
                 model with
                     CenikValues =
                         {
                             Id = value.Id; ValueState = value.ValueState
                             V001 = value.V001; V002 = value.V002; V003 = value.V003
                             V004 = value.V004; V005 = value.V005; V006 = value.V006
                             V007 = value.V007; V008 = value.V008; V009 = value.V009
                             Msgs = value.Msgs
                         }
                     ErrorMsg = sprintf "%s %s %s" value.Msgs.Msg1 value.Msgs.Msg2 value.Msgs.Msg3
             }, Cmd.none    
 
    let internal view (model : Model) (dispatch : Msg -> unit) links =
    
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
                            Html.text "Ceník služeb"                   
                        ]
                    ]
                    Html.ul [
                        Html.li [
                            prop.id 28
                            prop.children [
                                Html.text "Vstupní vyšetření a zhodnocení nutričního stavu pacienta, propočty a zhodnocení zaslaného jídelníčku k vyplnění"                       
                                Html.text (sprintf " %s " (String.replicate 36 "."))
                                Html.text model.CenikValues.V001
                                Html.text ",- Kč"
                            ]
                        ]
                        Html.li [
                            prop.id 29
                            prop.children [
                                Html.text "Asistovaný nákup s nutričním terapeutem (60 min)"                      
                                Html.text (sprintf " %s " (String.replicate 24 "."))
                                Html.text model.CenikValues.V002
                                Html.text ",- Kč"
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
                                Html.text "Redukční balíček (individuální redukční plán, konzultace nutričním terapeutem (45 min), redukční dieta, propočty a zhodnocení jídelníčku, pitný režim, pohybový režim, edukační materiály)"
                                Html.text (sprintf " %s " (String.replicate 44 "."))
                                Html.text model.CenikValues.V003
                                Html.text ",- Kč"
                            ]
                        ]
                        Html.li [
                            prop.id 32
                            prop.children [
                                Html.text "Kontrolní konzultace"                        
                                Html.text (sprintf " %s " (String.replicate 68 "."))
                                Html.text model.CenikValues.V004
                                Html.text ",- Kč"
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
                                Html.text "Edukace diety nutričním terapeutem (cca 60 min)"                        
                                Html.text (sprintf " %s " (String.replicate 25 "."))
                                Html.text model.CenikValues.V008
                                Html.text ",- Kč"
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
                            Html.text "Onemocnění žaludku, žlučníku, slivnivky břišní, racionální strava, dna, onemocnění střev, onemocnění ledvin, celiakie, antikolaguační léčba, anorexie, chudokrevnost a jiné"
                        ]
                    ]
                    Html.ul [
                        Html.li [
                            prop.id 39
                            prop.children [
                                Html.text "Edukace dané diety nutričním terapeutem (cca 60 min)"                       
                                Html.text (sprintf " %s " (String.replicate 16 "."))
                                Html.text model.CenikValues.V009
                                Html.text ",- Kč"
                            ]
                        ]
                    ]
                ]
            ]

        layout <| cenikHtml <| cenikRecord <| links