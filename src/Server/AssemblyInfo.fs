namespace Server

module AssemblyInfo =

    open System.Runtime.CompilerServices

    [<assembly: InternalsVisibleTo("Shared")>]
    [<assembly: InternalsVisibleTo("Client")>]
    [<assembly: InternalsVisibleTo("Server.Tests")>]
    do ()



