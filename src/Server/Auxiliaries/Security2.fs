// Author: Nathan B. Evans
// Twitter: @nbevans
// Blog: http://nbevans.wordpress.com/2014/03/13/pbkdf2-hash-function-for-fsharp/
// License: MIT

namespace Auxiliaries.Server

open System
open System.Security.Cryptography

module Security2 = 

    let private subkeyLength = 32
    let private saltSize = 16

    /// Hashes a password by a specified number of iterations using the PBKDF2 crypto function.
    let private hash password iterations =

        //failwith "Simulated exception hash"
        use algo = new Rfc2898DeriveBytes(password, saltSize, iterations)
        let salt = algo.Salt
        let bytes = algo.GetBytes(subkeyLength)
    
        let iters =
            match BitConverter.IsLittleEndian with
            | true  -> BitConverter.GetBytes(iterations)
            | false -> BitConverter.GetBytes(iterations) |> Array.rev
                
        let parts = Array.zeroCreate<byte> 54
        Buffer.BlockCopy(salt, 0, parts, 1, saltSize)
        Buffer.BlockCopy(bytes, 0, parts, 17, subkeyLength)
        Buffer.BlockCopy(iters, 0, parts, 50, sizeof<int>)

        Convert.ToBase64String(parts)

    /// Hashes a password using 10,000 iterations of the PBKDF2 crypto function.
    let internal fastHash password = hash password 10000

    /// Hashes a password using 100,000 iterations of the PBKDF2 crypto function.
    let internal strongHash password = hash password 100000

    /// Hashes a password using 300,000 iterations of the PBKDF2 crypto function.
    let internal uberHash password = hash password 300000

    /// Verifies a PBKDF2 hashed password with a candidate password.
    /// Returns true if the candidate password is correct.
    /// The hashed password must have been originally generated by one of the hash functions within this module.
    let internal verify hashedPassword (password:string) =

        //failwith "Simulated exception verify"
        let parts = Convert.FromBase64String(hashedPassword)

        match parts.Length <> 54 || parts.[0] <> byte 0 with
        | true  -> false
        | false ->
                   let salt = Array.zeroCreate<byte> saltSize
                   Buffer.BlockCopy(parts, 1, salt, 0, saltSize)

                   let bytes = Array.zeroCreate<byte> subkeyLength
                   Buffer.BlockCopy(parts, 17, bytes, 0, subkeyLength)

                   let iters = Array.zeroCreate<byte> sizeof<int>
                   Buffer.BlockCopy(parts, 50, iters, 0, sizeof<int>)

                   let iters =
                       match BitConverter.IsLittleEndian with
                       | true  -> iters
                       | false -> iters |> Array.rev

                   let iterations = BitConverter.ToInt32(iters, 0)
                   use algo = new Rfc2898DeriveBytes(password, salt, iterations)
                   let challengeBytes = algo.GetBytes(32)
                             
                   let x = Seq.compareWith (fun a b -> match a = b with true -> 0 | false -> 1) bytes challengeBytes 

                   match x with
                   | v when v = 0 -> true
                   | _            -> false
     
            

