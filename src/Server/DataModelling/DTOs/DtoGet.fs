namespace DtoGet.Server

open System

module DtoGet =

    type MessagesDtoGet =
        {
            Msg1: string
            Msg2: string
            Msg3: string
            Msg4: string
            Msg5: string
            Msg6: string
        }
       
    type CenikValuesDtoGet =
        {
            IdDtoGet: int option; ValueStateDtoGet: string option;
            V001DtoGet: string option; V002DtoGet: string option; V003DtoGet: string option;
            V004DtoGet: string option; V005DtoGet: string option; V006DtoGet: string option;
            V007DtoGet: string option; V008DtoGet: string option; V009DtoGet: string option;
            MsgsDtoGet: MessagesDtoGet option
        }

    // Defined but currently unused; retained for potential future requirements or updates.     
    type KontaktValuesDtoGet =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; Msgs: MessagesDtoGet
        }

    type LinkAndLinkNameValuesDtoGet  =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V001n: string; V002n: string; V003n: string;
            V004n: string; V005n: string; V006n: string;
            Msgs: MessagesDtoGet
        }

   