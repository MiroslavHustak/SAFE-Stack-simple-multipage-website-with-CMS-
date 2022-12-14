namespace Server

open System
open System.IO
open System.Data.SqlClient

open Saturn
open Dapper.FSharp
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open DbAccess.Sql       //uncomment to test plain SQL (and, at the same time, comment out "open DbAccess.Dapper")
//open DbAccess.Dapper  //uncomment to test Dapper.FSharp (and, at the same time, comment out "open DbAccess.Sql")

open DiscriminatedUnions.Server

open Auxiliary.Server.Security2
open Auxiliaries.Server.Connection
open Auxiliaries.Server.CopyingFiles
open Auxiliaries.Server.ROP_Functions
open Auxiliaries.Server.Serialisation
open Auxiliaries.Server.Deserialisation

module Server = 

    let private (>>=) condition nextFunc = 
        match condition with
        | false -> SharedApi.UsernameOrPasswordIncorrect  
        | true  -> nextFunc()

    [<Struct>]
    type private MyPatternBuilder = MyPatternBuilder with            
        member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc
        member _.Using x = x
        member _.Return x = x

    let private strContainsOnlySpace str = str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space
    let private isValidLogin inputUsrString inputPswString = not (strContainsOnlySpace inputUsrString || strContainsOnlySpace inputPswString)
    let private isValidCenik param = ()   //TODO validation upon request from the user 
    let private isValidKontakt param = () //TODO validation upon request from the user 
    let private isValidLink param = ()    //TODO validation upon request from the user

    let private pswHash() = //to be used only once before bundling             
       
        let usr = uberHash "" //delete username before bundling
        let psw = uberHash "" //delete password before bundling
        let mySeq = seq { usr; psw }
        
        use sw = new StreamWriter(Path.GetFullPath("uberHash.txt")) 
                |> Option.ofObj
                |> function
                    | Some value -> value
                    | None       -> //TODO some action 
                                    new StreamWriter(Path.GetFullPath("")) 
        mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) 

    let private verifyLogin (login: LoginInfo) =   // LoginInfo -> Async<LoginResult>>

        (*
        MyPatternBuilder    
            {
                let result = 
                match File.Exists(Path.GetFullPath("uberHash.txt")) with
                | false ->  Seq.empty                       
                | true  ->                        
                           match File.ReadAllLines("uberHash.txt") |> Option.ofObj with
                           | Some value -> (value |> Seq.ofArray) 
                           | None       -> Seq.empty  

                let! _ = isValidLogin login.Username login.Password
                let! _ = verify (result |> Seq.head) login.Username && verify (result |> Seq.last) login.Password               
                return SharedApi.LoggedIn { Username = login.Username; AccessToken = SharedApi.AccessToken "Dummy" }
            } 
        *)

        //containing redundant code for learning purposes
        MyPatternBuilder    
            {
                let result = 
                    match File.Exists(Path.GetFullPath("uberHash.txt")) with
                    | false ->  Seq.empty //No need of any action, Seq.empty will do its job here                                
                    | true  -> //StreamReader refused to work here, thar is why File.ReadAllLines was used                              
                               match File.ReadAllLines("uberHash.txt") |> Option.ofObj with
                               | Some value -> (value |> Seq.ofArray) 
                               | None       -> Seq.empty  //No need of any action, Seq.empty will do its job here

                let! _ = isValidLogin login.Username login.Password
                let securityTokenFile = Path.GetFullPath("securityToken.txt")
                                        |> Option.ofObj
                                        |> function
                                            | Some value -> value
                                            | None       -> //TODO some action only in case you decide to use saved accessToken
                                                            String.Empty           
           
                let! _ = verify (result |> Seq.head) login.Username && verify (result |> Seq.last) login.Password 

                //TODO trywith only in case you decide to use the saved accessToken
                let result =                
                    let accessToken = string <| System.Guid.NewGuid() //encodeJwt securityToken //TODO only in case you decide to use saved accessToken
                    //********************************************************************************
                    let mySeq = seq { login.Username; accessToken }
                    use sw = new StreamWriter(Path.GetFullPath(securityTokenFile)) 
                             |> Option.ofObj
                             |> function
                                 | Some value -> value
                                 | None       -> //TODO some action only in case you decide to use accessToken
                                                 new StreamWriter(Path.GetFullPath(securityTokenFile)) 
                    mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) 
                    //code with saved accessToken (originally a workaround) left for potential exploitation in the future

                    //*********************************************************************************************
                    SharedApi.LoggedIn { Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
                return result
            }
    
      //TODO validation upon request from the user 
    let private verifyCenikValues (cenikValues: GetCenikValues) =
        match isValidCenik () with
        | () -> Success ()        
        //| _  -> Failure ()

     //TODO validation upon request from the user 
    let private verifyKontaktValues (kontaktValues: GetKontaktValues) =
       match isValidKontakt () with
       | () -> Success ()        
       //| _  -> Failure ()

    //TODO validation upon request from the user 
    let private verifyLinkAndLinkNameValues (linkValues: GetLinkAndLinkNameValues) =
       match isValidLink () with
       | () -> Success ()        
       //| _  -> Failure ()

    let IGetApi exn =
        {
            login =
                fun login -> async { return (verifyLogin login) }          
         (*
          getSecurityTokenFile = //TODO try with
              fun getSecurityTokenFile -> async { return File.Exists(Path.GetFullPath("securityToken.txt")) }             
     
          getSecurityToken =
              fun getSecurityToken ->  //TODO try with
                  async
                      {       
                          match File.Exists(Path.GetFullPath("securityToken.txt")) with
                          | false -> //TODO error + some action
                                     return Seq.empty  
                          | true  -> //StreamReader refused to work here, thar is why File.ReadAllLines was used                              
                                     match File.ReadAllLines("securityToken.txt") |> Option.ofObj with
                                     | Some value -> return (value |> Seq.ofArray) 
                                     | None       -> return Seq.empty  //TODO error + some action                        
                      }        
      
          deleteSecurityTokenFile =
              fun deleteSecurityTokenFile ->  //TODO try with
                  async
                      {
                          File.Delete(Path.GetFullPath("securityToken.txt"))
                          return ()
                      }   
          *)
            getCenikValues =  
                fun getCenikValues ->
                    async
                        {                   
                            let getNewCenikValues: GetCenikValues =                        
                                match verifyCenikValues getCenikValues with                
                                | Success () ->
                                              let dbNewCenikValues = { getCenikValues with Id = 2; ValueState = "new" }

                                              //************* plain SQL or Dapper.FSharp ********************                                                                         
                                              let exnSql = insertOrUpdate dbNewCenikValues
                                         
                                              { dbNewCenikValues with Msgs = { Messages.Default with Msg1 = exnSql } }
                                                                           
                                | Failure () -> GetCenikValues.Default
                          return getNewCenikValues
                      }

            sendOldCenikValues = 
                fun _ -> 
                    async
                        {                           
                            //************* plain SQL or Dapper.FSharp ********************                     
                            let IdNew = 2
                            let IdOld = 3
                                                                    
                            let (dbGetNewCenikValues, exnSql2) = selectValues IdNew                                         
                            let exnSql = insertOrUpdate { dbGetNewCenikValues with Id = IdOld; ValueState = "old" }//eqv of the aforementioned copying 
                            let (dbSendOldCenikValues, exnSql3) = selectValues IdOld

                            return { dbSendOldCenikValues with Msgs = { Messages.Default with Msg1 = exnSql; Msg2 = exnSql2; Msg3 = exnSql3 } }
                        }

            sendDeserialisedCenikValues = 
               fun _ ->
                   async
                       {                          
                           //************* plain SQL or Dapper.FSharp ********************
                           let IdNew = 2
                    
                           let (dbSendCenikValues, exnSql1) = selectValues IdNew

                           return { dbSendCenikValues with Msgs = { Messages.Default with Msg1 = exnSql1; Msg2 = exn } }
                       }

             //************* from here downwards Json/XML ********************   
            getKontaktValues =
                fun getKontaktValues ->
                    async
                        {
                            let getNewKontaktValues: GetKontaktValues = 
                                match verifyKontaktValues getKontaktValues with
                                | Success () ->                                  
                                               let serializeNow x =
                                                  //failwith "Simulated exception10" 
                                                  serialize getKontaktValues "jsonKontaktValues.xml"                            
                                               let exnJson = (serializeNow, (fun x -> ()), "Zadan?? hodnoty nebyly ulo??eny, neb do??lo k t??to chyb??: Error10") |||> tryWith |> deconstructor1
                                               { getKontaktValues with Msgs = { Messages.Default with Msg1 = exnJson } }                                   
                                | Failure () -> GetKontaktValues.Default    
                            return getNewKontaktValues
                        }

            sendOldKontaktValues =
                fun _ ->
                    async
                        {
                            let copyFilesNow x =
                                //failwith "Simulated exception11" 
                                copyFiles 
                                <| "jsonKontaktValues.xml"
                                <| "jsonKontaktValuesBackUp.xml"
                            let exnJson1 = (copyFilesNow, (fun x -> ()), "Byly dosazeny p??edchoz?? hodnoty, neb do??lo k t??to chyb??: Error11") |||> tryWith |> deconstructor1
                    
                            let sendOldKontaktValuesNow x =
                                //failwith "Simulated exception12" 
                                deserialize "jsonKontaktValuesBackUp.xml"
                            let (sendOldKontaktValues, exnJson2) = (sendOldKontaktValuesNow, (fun x -> ()), "Byly dosazeny defaultn?? hodnoty, neb do??lo k t??to chyb??: Error12") |||> tryWith |> deconstructor2 GetKontaktValues.Default
                            return { sendOldKontaktValues with Msgs = { Messages.Default with Msg1 = exnJson1; Msg2 = exnJson2 } }                 
                        } 

            sendDeserialisedKontaktValues =
                fun _ ->
                    async
                        {                  
                            let sendKontaktValuesNow x =
                                //failwith "Simulated exception13" 
                                deserialize "jsonKontaktValues.xml" 
                            let (sendKontaktValues, exnJson1) = (sendKontaktValuesNow, (fun x -> ()), "Byly dosazeny defaultn?? hodnoty, neb do??lo k t??to chyb??: Error13") |||> tryWith |> deconstructor2 GetKontaktValues.Default
                            return { sendKontaktValues with Msgs = { Messages.Default with Msg1 = exnJson1 } }                   
                        }

            getLinkAndLinkNameValues =
               fun getLinkAndLinkNameValues ->
                   async
                      {
                         let getNewLinkAndLinkNameValues: GetLinkAndLinkNameValues = 
                             match verifyLinkAndLinkNameValues getLinkAndLinkNameValues with
                             | Success () ->
                                            let serializeNow x =
                                                //failwith "Simulated exception14" 
                                                serialize getLinkAndLinkNameValues "jsonLinkAndLinkNameValues.xml"                             
                                            let exnJson = (serializeNow, (fun x -> ()), "Zadan?? hodnoty nebyly ulo??eny, neb do??lo k t??to chyb??: Error14") |||> tryWith |> deconstructor1
                                            { getLinkAndLinkNameValues with Msgs = { Messages.Default with Msg1 = exnJson } }     
                             | Failure () -> GetLinkAndLinkNameValues.Default                        
                        return getNewLinkAndLinkNameValues
                      }
           
            sendOldLinkAndLinkNameValues =
                fun _ ->
                   async
                       {
                           let copyFilesNow x =
                               //failwith "Simulated exception15" 
                               copyFiles 
                               <| "jsonLinkAndLinkNameValues.xml"
                               <| "jsonLinkAndLinkNameValuesBackUp.xml"
                           let exnJson1 = (copyFilesNow, (fun x -> ()), "Byly dosazeny p??edchoz?? hodnoty, neb do??lo k t??to chyb??: Error15") |||> tryWith |> deconstructor1
                      
                           let sendOldLinkAndLinkNameValuesNow x =
                               //failwith "Simulated exception16" 
                               deserialize "jsonLinkAndLinkNameValuesBackUp.xml" 
                           let (sendOldLinkAndLinkNameValues, exnJson2) = (sendOldLinkAndLinkNameValuesNow, (fun x -> ()), "Byly dosazeny defaultn?? hodnoty, neb do??lo k t??to chyb??: Error16") |||> tryWith |> deconstructor2 GetLinkAndLinkNameValues.Default
                           return { sendOldLinkAndLinkNameValues with Msgs = { Messages.Default with Msg1 = exnJson1; Msg2 = exnJson2 } }  
                       } 

            sendDeserialisedLinkAndLinkNameValues =
               fun _ ->
                   async
                       {
                           let sendLinkAndLinkNameValuesNow x =
                               //failwith "Simulated exception17" 
                               deserialize "jsonLinkandLinkNameValues.xml"  
                           let (sendLinkAndLinkNameValues, exnJson1) = (sendLinkAndLinkNameValuesNow, (fun x -> ()), "Byly dosazeny defaultn?? hodnoty, neb do??lo k t??to chyb??: Error17") |||> tryWith |> deconstructor2 GetLinkAndLinkNameValues.Default
                           return { sendLinkAndLinkNameValues with Msgs = { Messages.Default with Msg1 = exnJson1 } }  
                       }
        }

    let webApp exn =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue (IGetApi exn)
        |> Remoting.buildHttpHandler

    let app =
        let exnSql = insertOrUpdate { GetCenikValues.Default with Msgs = { Messages.Default with Msg1 = "First run" } }
        application
            {
                use_router (webApp exnSql)
                memory_cache
                use_static "public"
                use_gzip
            }

    [<EntryPoint>]
    let main _ =   
        Dapper.FSharp.OptionTypes.register()
        //pswHash()
        run app    
        0