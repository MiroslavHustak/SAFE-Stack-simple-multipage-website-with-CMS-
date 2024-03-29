namespace SharedTypes

open System

//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions

[<RequireQualifiedAccess>]
module SharedApi = 

    //[<Struct>]
    //type AccessToken = AccessToken of string

    [<Struct>]
    type Username = Username of string //See Isaac Abraham page 272 onwards

    [<Struct>]
    type Password = Password of string

    [<Struct>]
    type LoginProblems =
        {
            line1: string
            line2: string
        }    

    [<Struct>]
    type User =
        {
            Username: Username
            //AccessToken: AccessToken
        }         
                 
    //https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions
    [<Struct>]
    type LoginResult =
        | UsernameOrPasswordIncorrect of UsernameOrPasswordIncorrect: LoginProblems
        | LoggedIn of LoggedIn: User

