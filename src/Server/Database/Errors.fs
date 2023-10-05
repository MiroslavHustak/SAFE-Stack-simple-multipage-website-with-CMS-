namespace Auxiliaries.Errors

open System
open ErrorTypes.Server
open PatternBuilders.Server.PatternBuilders
open Auxiliaries.Connections.Connection
open TransLayerSend.Server.TransLayerSend
open SharedTypes

module Errors =

    let internal insertOrUpdateError insertOrUpdate =     
        
        let cenikValuesDtoSendDefault = cenikValuesTransferLayerSend CenikValuesDomain.Default
        match insertOrUpdate getConnection closeConnection cenikValuesDtoSendDefault with
        | Ok _    -> InsertOrUpdateError1
        | Error _ -> InsertOrUpdateError2

    let internal errorMsgBoxIU insertOrUpdate cond =
    
        let du =
            match insertOrUpdate with
            | Ok _    -> NoError
            | Error _ -> InsertOrUpdateError
                
        //just testing active patterns... :-)
        let (|Cond1|Cond2|Cond3|) value =
        
            MyPatternBuilder    
                {    
                    let! _ = (<>) value NoError, Cond1
                    let! _ = (=) value NoError, Cond2                          
                    return Cond3
                }
                
        match du with
        | Cond2 when cond = true  -> FirstRunError 
        | Cond2 when cond = false -> InsertOrUpdateError
        | Cond1                   -> NoError
        | Cond3 | _               -> du

        |> function 
            | FirstRunError       -> "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při načítání hodnot z databáze."
            | InsertOrUpdateError -> "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k chybě při načítání hodnot z databáze." 
            | NoError             -> String.Empty

    let internal errorMsgBoxS err =
        match err with
        | InsertOrUpdateError1 -> CenikValuesDomain.Default, "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při ověřování existující databáze."
        | InsertOrUpdateError2 -> CenikValuesDomain.Default, "Došlo k chybě při načítání hodnot z databáze a dosazování defaultních hodnot. Zobrazované hodnoty mohou být chybné."
        | ReadingDbError       -> CenikValuesDomain.Default, "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty místo chybných hodnot."
        | ConnectionError      -> CenikValuesDomain.Default, "Chyba připojení k databázi. Dosazeny defaultní hodnoty místo chybných hodnot."